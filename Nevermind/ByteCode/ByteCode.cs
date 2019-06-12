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
                    Variable src = ExpressionLineItem.GetVariable(function, this, token.CodeToken);

                    if (token.UnaryOperators.Count != 0 && token.UnaryOperators[0] != null)
                    {
                        var unaryRes = token.UnaryOperators[0].UnaryFunc(new OperatorOperands(function, this, labelIndex++, src));
                        if (unaryRes.Error != null)
                            throw new ParseException(token.CodeToken, unaryRes.Error.ErrorType);

                        if (storeResultTo != null)
                        {
                            var lastInstruction = (UnaryArithmeticIntsruction)unaryRes.Instruction;
                            if (!unaryRes.ResultType.Equals(storeResultTo.Type))
                                throw new ParseException(lexeme.Tokens[0], CompileErrorType.IncompatibleTypes);
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
                            instructionSet.Instructions.Add(new InstructionLdi(src, storeResultTo, function, this, labelIndex++));
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
                else throw new ParseException(lexeme.Tokens[0], CompileErrorType.WrongOperandList);
            }
            else
            {
                Console.WriteLine(string.Join("\n", list));

                var res = ExpressionLineItem.GetInstructions(function, this, ref localVarIndex, list, out var registers);

                if ((res.Last() is BinaryArithmeticIntsruction) && ((BinaryArithmeticIntsruction)res.Last()).CanBeSimplified())
                {
                    res[res.Count - 1] = ((BinaryArithmeticIntsruction)res.Last()).Simplify();
                    registers.RemoveAt(registers.Count - 1);
                }

                regCount = Math.Max(regCount, registers.Count);
                if (registerInstructions.Count < regCount)
                    registerInstructions.AddRange(
                            registers.
                                Skip(registerInstructions.Count).
                                Take(registers.Count - registerInstructions.Count - (storeResultTo != null ? 1 : 0)).
                                Select(p => new InstructionReg(new NumeratedVariable(p.Index, p), function, this, labelIndexCopy++)));

                res.ForEach(p => p.Label = labelIndexCopy++);
                instructionSet.Instructions.AddRange(res);

                if (storeResultTo != null)
                {
                    var lastInstruction = (ArithmeticIntsruction)instructionSet.Instructions.Last();
                    if (!lastInstruction.Result.Type.Equals(storeResultTo.Type))
                        throw new ParseException(lexeme.Tokens[0], CompileErrorType.IncompatibleTypes);
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
                localVarIndex = function.LocalVariables.Count;

                switch (lexeme.Type)
                {
                    case LexemeType.Block:
                        break;
                    case LexemeType.Var:
                        {
                            //Calculate expression
                            var expression = ((VarLexeme)lexeme).Expression;
                            var storeResult = locals.Find(p => p.Index == ((VarLexeme)lexeme).Index).Variable;

                            ExpressionToList(expression, lexeme, function, out var variable, ref labelIndex, ref localVarIndex, ref regCount,
                                registerInstructions, instructionSet, locals, storeResult);
                        }
                        break;
                    case LexemeType.If:
                        {
                            var ifLexeme = lexeme as IfLexeme;

                            //Calculate expression
                            var expression = ((IfLexeme)lexeme).Expression;
                            ExpressionToList(expression, lexeme, function, out var variable, ref labelIndex, ref localVarIndex, ref regCount,
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
                            ExpressionToList((ExpressionLexeme)lexeme, lexeme, function, out var variable, ref labelIndex, ref localVarIndex, ref regCount,
                                registerInstructions, instructionSet, locals, null);
                        }
                        break;
                    case LexemeType.Return:
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

                foreach (var local in locals)
                {
                    instructionSet.Instructions.Add(new InstructionLocal(local, function, this, labelIndex++));
                }

                var registerInstructions = new List<InstructionReg>();
                var regCount = 0;

                GetInstructionList(function.RawLexeme, function, ref localVarIndex, ref regCount, ref labelIndex,
                    registerInstructions, instructionSet, locals);

                instructionSet.Instructions.InsertRange(function.LocalVariables.Count, registerInstructions); //.FindAll(p => p.Variable.Index >= localVarIndex - 1));
                instructionSet.Instructions.Add(new InstructionRet(function, this, labelIndex++));
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
                sb.AppendFormat("\n.{0}\n", instruction.Function.Name);
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