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
using System.Text;
using Nevermind.ByteCode.NMB;
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

        internal void ExpressionToList(
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

                if ((res.Last() is BinaryArithmeticInstruction) && ((BinaryArithmeticInstruction)res.Last()).CanBeSimplified())
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
                            var storeResult = locals.Find(p => p.Index == ((VarLexeme)lexeme).Index).Variable;

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

        public void Proceed()
        {
            Instructions = new List<FunctionInstructions>();
            foreach (var function in Program.Functions)
            {
                var labelIndex = 0;
                var instructionSet = new FunctionInstructions(function);

                var localVarIndex = 0;

                var locals = function.LocalVariables.Select(p => new NumeratedVariable(localVarIndex++, p)).ToList();

                if (function.Parameters.Count != 0)
                {
                    var parametrers = function.Parameters.Select(p => new Variable(p.Type, p.Name, function.Scope, p.CodeToken, localVarIndex++, VariableType.Variable));
                    locals.AddRange(parametrers.Select(p => new NumeratedVariable(p.Index, p)));
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

                GetInstructionList(function.RawLexeme, function, ref localVarIndex, ref regCount, ref labelIndex,
                    registers, instructionSet, locals);

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
                sb.AppendFormat("Has {0} locals \n", instruction.Locals.Count);
                foreach (var local in instruction.Locals)
                    sb.AppendFormat("  {0}. Type: {1}\n", local.Index, Header.GetTypeIndex(local.Type));

                sb.AppendFormat("Has {0} registers\n", instruction.Registers.Count);
                foreach (var register in instruction.Registers)
                    sb.AppendFormat("  {0}. Type: {1}\n", register.Index, Header.GetTypeIndex(register.Type));

                sb.AppendLine("ASM: ");
                sb.AppendLine(string.Join("\n", instruction.Instructions.Select(p => p.ToSource()).ToList()));
            }

            return sb.ToString();
        }

        public byte[] ToBinary()
        {
            var buffer = new List<byte>();

            buffer.AddRange(Codes.NMBSignature);
            var count = (ushort)(
                (Program.Metadata != null ? 1 : 0) + //Metadata chunk
                (Program.SaveDebugInfo    ? 1 : 0) + //Debug chunk
                3 +                                  //Header, Type, Const chunks
                Instructions.Count);                 //Function chunks

            buffer.AddRange(Chunk.UInt16ToBytes(count));
            buffer.AddRange(Header.GetHeaderChunk().ToBytes());
            buffer.AddRange(Header.GetTypesChunk().ToBytes());
            buffer.AddRange(Header.GetConstChunk().ToBytes());

            if(Program.Metadata != null)
                buffer.AddRange(Program.Metadata.GetChunk().ToBytes());

            foreach (var instruction in Instructions)
            {
                buffer.AddRange(instruction.GetChunk().ToBytes());
            }

            if(Program.SaveDebugInfo)
                buffer.AddRange(Header.GetDebugChunk().ToBytes());

            return buffer.ToArray();
        }

        public void SaveToFile(string fileName)
        {
            using (var f = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                var buffer = ToBinary();
                f.Write(buffer, 0, buffer.Length);
            }
        }

        public void Optimize()
        {
            ReferenceOptimizer.Optimize(this);
        }
    }
}