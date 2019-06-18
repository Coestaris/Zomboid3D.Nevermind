using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
using Nevermind.ByteCode.Instructions.ArithmeticIntsructions;
using Nevermind.ByteCode.InternalClasses;
using Nevermind.Compiler;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.LexemeParsing.Lexemes;
using Nevermind.Compiler.LexemeParsing.Lexemes.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nevermind.ByteCode
{
    public class ByteCode
    {
        public readonly NmProgram Program;
        internal readonly ByteCodeHeader Header;
        internal List<FunctionInstruction> Instructions;

        public ByteCode(NmProgram program)
        {
            Program = program;
            Header = new ByteCodeHeader(program);
        }

        internal void ExpressionToList(
            ExpressionLexeme expression, Lexeme lexeme, Function function, out Variable resultVar,
            ref int labelIndex, ref int localVarIndex, ref int regCount, List<InstructionReg> registerInstructions,
            FunctionInstruction instructionSet, List<NumeratedVariable> locals, Variable storeResultTo)

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
                            throw new ParseException(unaryRes.Error.ErrorType, token.CodeToken);

                        if (storeResultTo != null)
                        {
                            var lastInstruction = (UnaryArithmeticIntsruction)unaryRes.Instruction;
                            if (!unaryRes.ResultType.Equals(storeResultTo.Type))
                                throw new ParseException(CompileErrorType.IncompatibleTypes, lexeme.Tokens[0]);
                            lastInstruction.Result = storeResultTo;
                            resultVar = storeResultTo;
                        }
                        else
                        {
                            resultVar = (unaryRes.Instruction as UnaryArithmeticIntsruction).Result;
                        }

                        instructionSet.Instructions.Add(unaryRes.Instruction);

                    }
                    else
                    {
                        if (storeResultTo != null)
                        {
                            instructionSet.Instructions.Add(new InstructionLdi(src, storeResultTo, function, this, labelIndexCopy++));
                            resultVar = storeResultTo;
                        }
                        else
                        {

                            /*Variable dest = null;

                            if (regCount == 0)
                            {
                                new Variable(src.Type, "__reg", function.Scope, null, localVarIndex++);
                                registerInstructions.Add(new InstructionReg(new NumberedVariable(dest.Index, dest), function, this, labelIndex++));
                                regCount = 1;
                            }
                            else
                            {
                                dest = registerInstructions[0].Variable.Variable;
                            }

                            instructionSet.Instructions.Add(new InstructionLdi(src, dest, function, this, labelIndex++));
                            resultVar = (instructionSet.Instructions.Last() as InstructionLdi).Dest;*/
                            resultVar = src;
                        }
                    }
                }
                else throw new ParseException(CompileErrorType.WrongOperandList, lexeme.Tokens[0]);
            }
            else
            {
                Console.WriteLine(string.Join("\n", list));

                List<Variable> registers;
                var res = ExpressionLineItem.GetInstructions(function, this, ref localVarIndex, list, out registers, locals);

                if ((res.Last() is BinaryArithmeticIntsruction) && ((BinaryArithmeticIntsruction)res.Last()).CanBeSimplified())
                {
                    var last = (BinaryArithmeticIntsruction)res.Last();

                    if (last.Type == BinaryArithmeticIntsructionType.A_Set && res.Count != 1)
                    {
                        res.RemoveAt(res.Count - 1);
                        (res[res.Count - 1] as ArithmeticIntsruction).Result = last.Operand1;
                    }
                    else
                    {
                        res[res.Count - 1] = last.Simplify();
                    }
                    registers.RemoveAt(registers.Count - 1);
                }

                regCount = Math.Max(regCount, registers.Count);
                if (registerInstructions.Count < regCount)
                    registerInstructions.AddRange(
                            registers.
                                Skip(registerInstructions.Count).
                                Take(registers.Count - registerInstructions.Count - (storeResultTo != null ? 1 : 0)).
                                Select(p => new InstructionReg(new NumeratedVariable(p.Index, p), function, this, -1)));

                res.ForEach(p => p.Label = labelIndexCopy++);
                instructionSet.Instructions.AddRange(res);

                if (storeResultTo != null)
                {
                    var lastInstruction = (ArithmeticIntsruction)instructionSet.Instructions.Last();
                    if (!lastInstruction.Result.Type.Equals(storeResultTo.Type))
                        throw new ParseException(CompileErrorType.IncompatibleTypes, lexeme.Tokens[0]);
                    lastInstruction.Result = storeResultTo;
                    resultVar = storeResultTo;
                }
                else
                {
                    var last = instructionSet.Instructions.Last();
                    resultVar = (last as ArithmeticIntsruction).Result;
                }
            }

            labelIndex = labelIndexCopy;

        }

        private void GetInstructionList(Lexeme rootLexeme, Function function, ref int localVarIndex, ref int regCount, ref int labelIndex,
            List<InstructionReg> registerInstructions, FunctionInstruction instructionSet, List<NumeratedVariable> locals)
        {
            foreach (var lexeme in rootLexeme.ChildLexemes)
            {
                localVarIndex = locals.Count;

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
                                registerInstructions, instructionSet, locals, storeResult);
                        }
                        break;
                    case LexemeType.If:
                        {
                            var ifLexeme = lexeme as IfLexeme;

                            //Calculate expression
                            var expression = ((IfLexeme)lexeme).Expression;
                            Variable variable;
                            ExpressionToList(expression, lexeme, function, out variable, ref labelIndex, ref localVarIndex, ref regCount,
                                registerInstructions, instructionSet, locals, null);

                            instructionSet.Instructions.Add(new InstructionBrEq(variable, -1, function, this, labelIndex++));
                            var eq = instructionSet.Instructions.Last() as InstructionBrEq;

                            //Proceed block
                            GetInstructionList(ifLexeme.Block, function, ref localVarIndex, ref regCount, ref labelIndex,
                                registerInstructions, instructionSet, locals);

                            if (ifLexeme.ElseLexeme != null)
                            {
                                instructionSet.Instructions.Add(new InstructionJmp(-1, function, this, labelIndex++));
                                var jmp = instructionSet.Instructions.Last() as InstructionJmp;

                                eq.Index = labelIndex;

                                //Proceed else block
                                GetInstructionList(ifLexeme.ElseLexeme.Block, function, ref localVarIndex, ref regCount, ref labelIndex,
                                    registerInstructions, instructionSet, locals);

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
                                registerInstructions, instructionSet, locals, null);
                        }
                        break;
                    case LexemeType.Return:
                        {
                            //Calculate expression
                            Variable variable;
                            ExpressionToList(((ReturnLexeme)lexeme).Expression, lexeme, function, out variable, ref labelIndex, ref localVarIndex, ref regCount,
                                registerInstructions, instructionSet, locals, null);

                            if (!variable.Type.Equals(function.ReturnType))
                                throw new ParseException(CompileErrorType.IncompatibleTypes, lexeme.Tokens[1]);

                            instructionSet.Instructions.Add(new InstructionPush(variable, function, this, labelIndex++));
                        }
                        break;
                }
            }
        }

        public void Proceed()
        {
            Instructions = new List<FunctionInstruction>();
            foreach (var function in Program.Functions)
            {
                var labelIndex = 0;
                var instructionSet = new FunctionInstruction(function);

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
                    instructionSet.Instructions.Add(new InstructionLocal(local, function, this, labelIndex++));
                }

                if (function.Parameters.Count != 0)
                {
                    foreach (var param in locals.Skip(function.LocalVariables.Count))
                        instructionSet.Instructions.Add(new InstructionPop(param.Variable, function, this, labelIndex++));
                }

                var registerInstructions = new List<InstructionReg>();
                var regCount = 0;

                GetInstructionList(function.RawLexeme, function, ref localVarIndex, ref regCount, ref labelIndex,
                    registerInstructions, instructionSet, locals);

                int counter = 0;
                registerInstructions.ForEach(p => p.Label = locals.Count + counter++);

                instructionSet.Instructions.InsertRange(locals.Count, registerInstructions); //.FindAll(p => p.Variable.Index >= localVarIndex - 1));
                instructionSet.Instructions.Add(new InstructionRet(function, this, labelIndex++));

                for (int i = locals.Count + registerInstructions.Count; i < instructionSet.Instructions.Count; i++)
                {
                    instructionSet.Instructions[i].Label += registerInstructions.Count;
                    if (instructionSet.Instructions[i] is InstructionJmp)
                        (instructionSet.Instructions[i] as InstructionJmp).Index += registerInstructions.Count;
                }
                Instructions.Add(instructionSet);
            }
        }

        public string ToSource()
        {
            var sb = new StringBuilder();
            sb.Append(Header.ToSource());
            sb.AppendLine();
            foreach (var instruction in Instructions)
            {
                sb.AppendFormat("\n.{0} ({1})\n", instruction.Function.Name, instruction.Function.Index);
                sb.AppendLine(string.Join("\n", instruction.Instructions.Select(p => p.ToSource()).ToList()));
            }

            return sb.ToString();
        }

        public byte[] ToBinary()
        {
            return Header.ToBinary();
        }
    }
}