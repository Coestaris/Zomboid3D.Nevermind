using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.InternalClasses;
using Nevermind.Compiler;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.LexemeParsing.Lexemes;
using Nevermind.Compiler.LexemeParsing.Lexemes.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler.Semantics;
using Nevermind.Compiler.Semantics.Attributes;
using Type = Nevermind.ByteCode.Types.Type;

namespace Nevermind.ByteCode
{
    public class ByteCode
    {
        public readonly NmProgram Program;
        internal readonly ByteCodeHeader Header;
        internal List<FunctionInstructions> Instructions;

        public ByteCode(NmProgram program)
        {
            Program = program;
            Header = new ByteCodeHeader(program);
        }

        private void ExpressionToList(
            ExpressionLexeme expression, Lexeme lexeme, Function function, out Variable resultVar,
            ref int labelIndex, ref int localVarIndex, ref int regCount, List<Variable> outRegisters,
            FunctionInstructions instructionsSet, List<NumeratedVariable> locals, Variable storeResultTo)
        {
            ExpressionToList_GetList(expression, lexeme, function, out resultVar, ref labelIndex,
                ref localVarIndex, ref regCount, outRegisters, instructionsSet, locals, storeResultTo);

            ExpressionToList_FixIndexerAssignment(expression, lexeme, function, ref labelIndex,
                ref localVarIndex, ref regCount, outRegisters, instructionsSet, locals, storeResultTo);
        }

        private void ExpressionToList_FixIndexerAssignment(
            ExpressionLexeme expression, Lexeme lexeme, Function function,
            ref int labelIndex, ref int localVarIndex, ref int regCount, List<Variable> outRegisters,
            FunctionInstructions instructionsSet, List<NumeratedVariable> locals, Variable storeResultTo)
        {
            //find assignment to array elements and replace it by vset and remove first vget
            for (int i = 0; i < instructionsSet.Instructions.Count; i++)
            {
                Variable result = null;
                var simplified = false;
                var set = false;

                if (instructionsSet.Instructions[i] is ArithmeticInstruction &&
                    !(instructionsSet.Instructions[i] is InstructionVget))
                {
                    if(instructionsSet.Instructions[i] is BinaryArithmeticInstruction &&
                            (instructionsSet.Instructions[i] as BinaryArithmeticInstruction).CanBeSimplified())
                    {
                        var instruction = ((BinaryArithmeticInstruction) instructionsSet.Instructions[i]);

                        result = instruction.Operand1;
                        simplified = true;

                        if (instruction.AType == BinaryArithmeticInstructionType.A_Set)
                            set = true;
                    }
                    else
                        result = ((ArithmeticInstruction) instructionsSet.Instructions[i]).Result;
                }
                else
                {
                    continue;
                }

                InstructionVset vset;

                if (result.VariableType == VariableType.ArrayItem)
                {
                    if (!simplified)
                    {
                        var newReg = new Variable(result.Type, "__arrayReg", function.Scope,
                            result.Token, localVarIndex++, VariableType.Variable);

                        vset = new InstructionVset(
                            result.Array, newReg, result.ArrayItem, function, this, labelIndex++);

                        outRegisters.Add(newReg);

                        instructionsSet.Instructions.Insert(i + 1, vset);

                        var globalIndex = 0;
                        var deleted = instructionsSet.Instructions.RemoveAll(p =>
                        {
                            var vget = p as InstructionVget;

                            if (vget == null) return false;

                            var used = false;
                            for (var j = globalIndex + 1; j < instructionsSet.Instructions.Count; j++)
                            {
                                var count = instructionsSet.Instructions[j].FetchUsedVariables(vget.Result.Index).Count;
                                count -= (instructionsSet.Instructions[j] as ArithmeticInstruction)?.Result.Index ==
                                         vget.Result.Index
                                    ? 1
                                    : 0;

                                if (count > 0)
                                {
                                    used = true;
                                    break;
                                }
                            }

                            globalIndex++;

                            return vget.Result.Index == result.Index && !used;
                        });

                        (instructionsSet.Instructions[i - deleted] as ArithmeticInstruction).Result = newReg;
                    }
                    else
                    {
                        if (!set)
                        {
                            instructionsSet.Instructions[i] =
                                ((BinaryArithmeticInstruction) instructionsSet.Instructions[i]).Simplify();

                            vset = new InstructionVset(
                                result.Array, (instructionsSet.Instructions[i] as ArithmeticInstruction).Result,
                                    result.ArrayItem, function, this, labelIndex++);

                            instructionsSet.Instructions.Insert(i + 1, vset);
                        }
                        else
                        {
                            vset = new InstructionVset(
                                result.Array, result, result.ArrayItem, function, this, labelIndex++);

                            instructionsSet.Instructions.Insert(i + 1, vset);
                            instructionsSet.Instructions.RemoveAt(i);

                            instructionsSet.Instructions.RemoveAll(p =>
                            {
                                var vget = p as InstructionVget;
                                return vget != null && vget.Result.Index == result.Index;
                            });
                        }
                    }


                }
            }
        }

        private void ExpressionToList_GetList(
            ExpressionLexeme expression, Lexeme lexeme, Function function, out Variable resultVar,
            ref int labelIndex, ref int localVarIndex, ref int regCount, List<Variable> outRegisters,
            FunctionInstructions instructionsSet, List<NumeratedVariable> locals, Variable storeResultTo)
        {

            var list = expression.ToList();
            var labelIndexCopy = labelIndex;

            if (list == null)
            {
                if (expression.Root.SubTokens.Count == 1)
                {
                    ExpressionToken token = expression.Root.SubTokens[0];
                    Variable src = ExpressionLineItem.GetVariable(locals, this, token.CodeToken);

                    if (token.UnaryOperators.Count != 0 && token.UnaryOperators[0] != null)
                    {
                        var unaryRes = token.UnaryOperators[0].UnaryFunc(new OperatorOperands(function, this, labelIndexCopy++, src));
                        if (unaryRes.Error != null)
                            throw new CompileException(unaryRes.Error.ErrorType, token.CodeToken);

                        instructionsSet.Instructions.Add(unaryRes.Instruction);

                        if (storeResultTo != null)
                        {
                            var lastInstruction = (UnaryArithmeticInstruction)unaryRes.Instruction;
                            if (unaryRes.ResultType != storeResultTo.Type)
                            {
                                //but can we cast?
                                if(!Type.CanCastAssignment(storeResultTo.Type, unaryRes.ResultType))
                                    throw new CompileException(CompileErrorType.IncompatibleTypes, lexeme.Tokens[0]);

                                var castedVar = new Variable(unaryRes.ResultType, "__unaryReg", function.Scope,
                                    null, localVarIndex++, VariableType.Variable);
                                instructionsSet.Instructions.Add(new InstructionCast(storeResultTo, castedVar, function, this,
                                    labelIndexCopy++));

                                lastInstruction.Result = castedVar;
                                outRegisters.Add(castedVar);
                            }
                            else
                            {
                                lastInstruction.Result = storeResultTo;
                            }

                            resultVar = storeResultTo;
                        }
                        else
                        {
                            resultVar = (unaryRes.Instruction as UnaryArithmeticInstruction).Result;
                        }
                    }
                    else
                    {
                        if (storeResultTo != null)
                        {
                            if (storeResultTo.Type != src.Type)
                            {
                                //Not equal, cast is needed.
                                if (!Type.CanCastAssignment(storeResultTo.Type, src.Type))
                                    throw new CompileException(CompileErrorType.IncompatibleTypes, lexeme.Tokens[0]);

                                instructionsSet.Instructions.Add(new InstructionCast(storeResultTo, src, function, this, labelIndexCopy++));
                                resultVar = storeResultTo;
                            }
                            else
                            {
                                instructionsSet.Instructions.Add(new InstructionLdi(src, storeResultTo, function, this,
                                    labelIndexCopy++));
                                resultVar = storeResultTo;
                            }
                        }
                        else
                        {

                            resultVar = src;
                        }
                    }
                }
                else throw new CompileException(CompileErrorType.WrongOperandList, lexeme.Tokens[0]);
            }
            else
            {
                 if(function.Program.Verbose)
                     Console.WriteLine(string.Join("\n", list));

                List<Variable> registers;
                var res = ExpressionLineItem.GetInstructions(function, this, ref localVarIndex, list, out registers, locals);

                if (
                    (res.Last() is BinaryArithmeticInstruction) &&
                    ((BinaryArithmeticInstruction)res.Last()).CanBeSimplified() &&
                    (res.Count < 2 || res[res.Count - 2].Type != InstructionType.Vget ||
                     (res.Last() as BinaryArithmeticInstruction).Operand1.VariableType != VariableType.ArrayItem))
                {
                    var last = (BinaryArithmeticInstruction)res.Last();

                    if (last.AType == BinaryArithmeticInstructionType.A_Set && res.Count != 1)
                    {
                        res.RemoveAt(res.Count - 1);
                        (res[res.Count - 1] as ArithmeticInstruction).Result = last.Operand1;
                    }
                    else
                    {
                        res[res.Count - 1] = last.Simplify();
                    }
                    registers.RemoveAt(registers.Count - 1);
                }

                regCount += registers.Count;
                outRegisters.AddRange(registers);

                res.ForEach(p => p.Label = labelIndexCopy++);
                instructionsSet.Instructions.AddRange(res);

                if (storeResultTo != null)
                {
                    if (!(instructionsSet.Instructions.Last() is ArithmeticInstruction))
                        throw new CompileException(CompileErrorType.ExpressionIsNotVariable, lexeme.Tokens[1]);

                    var lastInstruction = (ArithmeticInstruction) instructionsSet.Instructions.Last();
                    if (lastInstruction.Result.Type != storeResultTo.Type)
                    {
                        //but can we cast?
                        if (!Type.CanCastAssignment(storeResultTo.Type, lastInstruction.Result.Type))
                            throw new CompileException(CompileErrorType.IncompatibleTypes, lexeme.Tokens[0]);

                        instructionsSet.Instructions.Add(new InstructionCast(storeResultTo, lastInstruction.Result, function, this,
                            labelIndexCopy++));
                    }
                    else
                    {
                        lastInstruction.Result = storeResultTo;
                    }
                    resultVar = storeResultTo;

                }
                else
                {
                    var last = instructionsSet.Instructions.Last();

                    if (!(last is ArithmeticInstruction))
                        resultVar = null;
                    else
                        resultVar = (last as ArithmeticInstruction).Result;
                }
            }

            labelIndex = labelIndexCopy;
        }

        private void GetInstructionList(Lexeme rootLexeme, Function function, ref int localVarIndex, ref int regCount, ref int labelIndex,
            List<Variable> registers, FunctionInstructions instructionsSet, List<NumeratedVariable> locals)
        {
            foreach (var lexeme in rootLexeme.ChildLexemes)
            {
                //localVarIndex = locals.Count;

                switch (lexeme.Type)
                {
                    case LexemeType.Block:
                        break;
                    case LexemeType.Var:
                        {
                            //Calculate expression
                            var expression = ((VarLexeme)lexeme).Expression;

                            //declaration only
                            if (expression == null)
                                continue;

                            var storeResult = locals.Find(p => p.Index ==
                                                ((VarLexeme)lexeme).Index + Program.ProgramGlobals.Count).Variable;

                            Variable variable;
                            ExpressionToList(expression, lexeme, function, out variable, ref labelIndex, ref localVarIndex, ref regCount,
                                registers, instructionsSet, locals, storeResult);
                        }
                        break;
                    case LexemeType.If:
                        {
                            var ifLexeme = lexeme as IfLexeme;

                            //Calculate expression
                            var expression = ((IfLexeme)lexeme).Expression;
                            Variable variable;
                            ExpressionToList(expression, lexeme, function, out variable, ref labelIndex, ref localVarIndex, ref regCount,
                                registers, instructionsSet, locals, null);

                            if(variable == null)
                                throw new CompileException(CompileErrorType.ExpressionIsNotVariable, lexeme.Tokens[1]);

                            instructionsSet.Instructions.Add(new InstructionBrEq(variable, -1, function, this, labelIndex++));
                            var eq = instructionsSet.Instructions.Last() as InstructionBrEq;

                            //Proceed block
                            GetInstructionList(ifLexeme.Block, function, ref localVarIndex, ref regCount, ref labelIndex,
                                registers, instructionsSet, locals);

                            if (ifLexeme.ElseLexeme != null)
                            {
                                instructionsSet.Instructions.Add(new InstructionJmp(-1, function, this, labelIndex++));
                                var jmp = instructionsSet.Instructions.Last() as InstructionJmp;

                                eq.Index = labelIndex;

                                //Proceed else block
                                GetInstructionList(ifLexeme.ElseLexeme.Block, function, ref localVarIndex, ref regCount, ref labelIndex,
                                    registers, instructionsSet, locals);

                                jmp.Index = labelIndex;
                            }
                            else
                            {
                                eq.Index = labelIndex;
                            }
                        }
                        break;
                    case LexemeType.Expression:
                        {
                            //Calculate expression
                            Variable variable;
                            ExpressionToList((ExpressionLexeme)lexeme, lexeme, function, out variable, ref labelIndex, ref localVarIndex, ref regCount,
                                registers, instructionsSet, locals, null);
                        }
                        break;
                    case LexemeType.While:
                        {
                            var whileLexeme = lexeme as WhileLexeme;
                            int startIndex = labelIndex;

                            //Calculate expression
                            var expression = whileLexeme.Expression;
                            Variable variable;
                            ExpressionToList(expression, lexeme, function, out variable, ref labelIndex, ref localVarIndex, ref regCount,
                                registers, instructionsSet, locals, null);

                            if(variable == null)
                                throw new CompileException(CompileErrorType.ExpressionIsNotVariable, lexeme.Tokens[1]);

                            instructionsSet.Instructions.Add(new InstructionBrEq(variable, -1, function, this, labelIndex++));
                            var eq = instructionsSet.Instructions.Last() as InstructionBrEq;

                            //Proceed block
                            GetInstructionList(whileLexeme.Block, function, ref localVarIndex, ref regCount, ref labelIndex,
                                registers, instructionsSet, locals);

                            instructionsSet.Instructions.Add(
                                new InstructionJmp(startIndex, function, this, labelIndex++));

                            eq.Index = labelIndex;
                        }
                        break;
                    case LexemeType.Return:
                        {
                            //Calculate expression
                            Variable variable;
                            ExpressionToList(((ReturnLexeme)lexeme).Expression, lexeme, function, out variable, ref labelIndex, ref localVarIndex, ref regCount,
                                registers, instructionsSet, locals, null);

                            if (variable.Type != function.ReturnType)
                            {
                                //but can we cast?
                                if(!Type.CanCastAssignment(function.ReturnType, variable.Type))
                                    throw new CompileException(CompileErrorType.IncompatibleTypes, lexeme.Tokens[1]);

                                //casting
                                var castedVar = new Variable(function.ReturnType, "__castedReg", function.Scope,
                                    null, localVarIndex++, VariableType.Variable);
                                instructionsSet.Instructions.Add(new InstructionCast(castedVar, variable, function, this,
                                    labelIndex++));
                                variable = castedVar;
                                registers.Add(castedVar);
                            }

                            instructionsSet.Instructions.Add(new InstructionPush(variable, function, this, labelIndex++));
                        }
                        break;
                }
            }
        }

        private static List<FunctionInstructions> GetModuleFunctions(NmProgram program, bool init)
        {
            var result = new List<FunctionInstructions>();
            foreach (var import in program.Imports)
                result.AddRange(GetModuleFunctions(import.LinkedModule.Program, init));

            if (program.IsModule)
            {
                if (init)
                    result.Add(program.ByteCode.Instructions.Find(p =>
                        p.Function == program.Module.InitializationFunc));
                else
                    result.Add(program.ByteCode.Instructions.Find(p =>
                        p.Function == program.Module.FinalizationFunc));
            }

            return result;
        }

        private static List<Variable> GetModuleGlobals(NmProgram program)
        {
            var result = new List<Variable>();
            foreach (var import in program.Imports)
                result.AddRange(GetModuleGlobals(import.LinkedModule.Program));

            if (program.IsModule)
            {
               result.AddRange(program.ProgramGlobals);
               program.ProgramGlobals.ForEach(p => p.Name = $"@_{program.Module.Name}_{p.Name}");
            }

            return result;
        }

        public void Proceed()
        {
            //embed all module init and fin functions
            List<FunctionInstructions> inits = null;
            List<FunctionInstructions> fins  = null;
            if (Program.EntrypointFunction != null)
            {
                //embed module globals
                Program.ProgramGlobals.AddRange(GetModuleGlobals(Program));

                inits = GetModuleFunctions(Program, true);
                fins = GetModuleFunctions(Program, false);
                foreach (var function in inits)
                    Header.EmbedFunction(function, function.Function.Token);
                foreach (var function in fins)
                    Header.EmbedFunction(function, function.Function.Token);
            }

            Instructions = new List<FunctionInstructions>();
            foreach (var function in Program.Functions)
            {
                var labelIndex = 0;
                var instructionSet = new FunctionInstructions(function);

                //link inits
                /*if (function.Modifier == FunctionModifier.Entrypoint)
                    foreach (var initFunction in inits)
                        instructionSet.Instructions.Add(
                            new InstructionCall(initFunction.Function, function, this, labelIndex));*/

                var localVarIndex = Program.ProgramGlobals.Count;

                var locals = function.LocalVariables.Select(p => new NumeratedVariable(localVarIndex++, p)).ToList();

                if (function.Parameters.Count != 0)
                {
                    var parameters = function.Parameters.Select(p => new Variable(p.Type, p.Name, function.Scope, p.CodeToken, localVarIndex++, VariableType.Variable));
                    locals.AddRange(parameters.Select(p => new NumeratedVariable(p.Index, p)));
                }
                
                foreach (var local in locals)
                {
                    local.Variable.Index = local.Index;
                    //instructionSet.Instructions.Add(new InstructionLocal(local, function, this, labelIndex++));
                }

                if (function.Parameters.Count != 0)
                {
                    foreach (var param in locals.Skip(function.LocalVariables.Count))
                        instructionSet.Instructions.Add(new InstructionPop(param.Variable, function, this, labelIndex++));
                }

                var registers = new List<Variable>();
                var regCount = 0;

                //begin syscalls, variadic attributes
                foreach (var attribute in function.Attributes)
                {
                    switch (attribute.Type)
                    {
                        case AttributeType.Syscall:
                            if(!((SyscallAttribute)attribute).AppendToEnd)
                                instructionSet.Instructions.Add(new InstructionSyscall((attribute as SyscallAttribute).CallCode,
                                    function, this, labelIndex++));
                            break;

                        case AttributeType.Variadic:
                            function.IsVariadic = true;
                            break;
                    }
                }

                GetInstructionList(function.RawLexeme, function, ref localVarIndex, ref regCount, ref labelIndex,
                    registers, instructionSet, locals);

                //link fins
/*                if (function.Modifier == FunctionModifier.Entrypoint)
                    foreach (var initFunction in fins)
                        instructionSet.Instructions.Add(
                            new InstructionCall(initFunction.Function, function, this, labelIndex));*/

                //end syscalls
                foreach (var attribute in function.Attributes.FindAll(p => p is SyscallAttribute && ((SyscallAttribute) p).AppendToEnd))
                {
                    instructionSet.Instructions.Add(new InstructionSyscall((attribute as SyscallAttribute).CallCode,
                        function, this, labelIndex++));
                }

                instructionSet.Instructions.Add(new InstructionRet(function, this, labelIndex++));

                instructionSet.Registers = registers;
                instructionSet.Locals = locals.Select(p => p.Variable).ToList();
                Instructions.Add(instructionSet);
            }

            Instructions.AddRange(Header.EmbeddedFunctions);
            Program.Functions.AddRange(Header.EmbeddedFunctions.Select(p => p.Function));
        }

        public string ToSource()
        {
            var sb = new StringBuilder();
            sb.Append(Header.ToSource());
            sb.AppendLine();
            foreach (var instruction in Instructions)
            {
                sb.AppendFormat("\nFunction: {0} ({1})\n", instruction.Function.Name, instruction.Function.Index);
                if (instruction.Locals.Count != 0)
                {
                    sb.AppendFormat("Has {0} locals \n", instruction.Locals.Count);
                    foreach (var local in instruction.Locals)
                        sb.AppendFormat("  {0}. Type: {1}\n", local.Index, Header.GetTypeIndex(local.Type));
                }

                if (instruction.Registers.Count != 0)
                {
                    sb.AppendFormat("Has {0} registers\n", instruction.Registers.Count);
                    foreach (var register in instruction.Registers)
                        sb.AppendFormat("  {0}. Type: {1}\n", register.Index, Header.GetTypeIndex(register.Type));
                }

                sb.AppendLine("ASM: ");
                sb.AppendLine(string.Join("\n", instruction.Instructions.Select(p => p.ToSource()).ToList()));
            }

            return sb.ToString();
        }

        public byte[] Serialize()
        {
            var chunks = new List<Chunk>();
            chunks.Add(Header.GetHeaderChunk());
            chunks.Add(Header.GetTypesChunk());
            chunks.Add(Header.GetConstChunk());

            if(Program.ProgramGlobals.Count != 0)
                chunks.Add(Header.GetGlobalsChunk());

            if(Program.Metadata != null)
                chunks.Add(Program.Metadata.GetChunk());

            foreach (var instruction in Instructions)
            {
                chunks.Add(instruction.GetChunk(Header));
            }

            if(Program.SaveDebugInfo)
                chunks.Add(Header.GetDebugChunk());


            var buffer = new List<byte>();
            buffer.AddRange(Codes.NMBSignature);
            buffer.AddRange(Chunk.UInt16ToBytes((ushort)chunks.Count));
            foreach (var chunk in chunks)
                buffer.AddRange(chunk.Serialize());

            return buffer.ToArray();
        }

        public void SaveToFile(string fileName)
        {
            using (var f = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                var buffer = Serialize();
                f.Write(buffer, 0, buffer.Length);
            }
        }

        public void Optimize()
        {
            ReferenceOptimizer.Optimize(this);
        }
    }
}