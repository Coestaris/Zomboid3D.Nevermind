using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
using Nevermind.Compiler;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.LexemeParsing.Lexemes;
using Nevermind.Compiler.LexemeParsing.Lexemes.Expressions;

namespace Nevermind.ByteCode
{
    internal class FunctionInstruction
    {
        public readonly List<Instruction> Instructions;
        public readonly Function Function;

        public FunctionInstruction(Function function)
        {
            Function = function;
            Instructions = new List<Instruction>();
        }
    }

    internal class NumberedVariable
    {
        public readonly int Index;
        public readonly Variable Variable;

        public NumberedVariable(int index, Variable variable)
        {
            Index = index;
            Variable = variable;
        }
    }

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

        public void Proceed()
        {
            Instructions = new List<FunctionInstruction>();
            foreach (var function in Program.Functions)
            {
                var labelIndex = 0;
                var instructionSet = new FunctionInstruction(function);

                var localVarIndex = 0;
                var locals = function.LocalVariables.Select(p => new NumberedVariable(localVarIndex++, p)).ToList();

                foreach (var local in locals)
                {
                    instructionSet.Instructions.Add(new InstructionLocal(local, function, this, labelIndex++));
                }

                var registerInstructions = new List<InstructionReg>();
                var regCount = 0;
                foreach (var lexeme in function.RawLexeme.ChildLexemes)
                {
                    localVarIndex = function.LocalVariables.Count;
                    
                    if (lexeme.Type == LexemeType.Var)
                    {
                        var list = ((VarLexeme)lexeme).Expression.ToList();

                        if (list == null)
                        {
                            if (((VarLexeme)lexeme).Expression.Root.SubTokens.Count == 1)
                            {
                                ExpressionToken token = ((VarLexeme)lexeme).Expression.Root.SubTokens[0];
                                Variable src = ExpressionLineItem.GetVariable(function, this, token.CodeToken);

                                if (token.UnaryOperators.Count != 0 && token.UnaryOperators[0] != null)
                                {
                                    var unaryRes = token.UnaryOperators[0].UnaryFunc(new OperatorOperands(function, this, labelIndex++, src));
                                    if (unaryRes.Error != null)
                                        throw new ParseException(token.CodeToken, unaryRes.Error.ErrorType);

                                    var lastInstruction = (UnaryArithmeticIntsruction)unaryRes.Instruction;
                                    var actualResult = locals.Find(p => p.Index == ((VarLexeme)lexeme).Index).Variable;

                                    if (!unaryRes.ResultType.Equals(actualResult.Type))
                                        throw new ParseException(lexeme.Tokens[0], CompileErrorType.IncompatibleTypes);

                                    lastInstruction.Result = actualResult;
                                    instructionSet.Instructions.Add(unaryRes.Instruction);
                                }
                                else
                                {
                                    instructionSet.Instructions.Add(
                                        new InstructionLdi(src, locals.Find(p => p.Index == ((VarLexeme)lexeme).Index).Variable,
                                            function, this, labelIndex++));
                                }
                            }
                            else throw new ParseException(lexeme.Tokens[0], CompileErrorType.WrongOperandList);
                        }
                        else
                        {
                            Console.WriteLine(string.Join("\n", list));

                            var res = ExpressionLineItem.GetInstructions(function, this, ref localVarIndex, list, out var registers);
                            regCount = Math.Max(regCount, registers.Count);

                            if(registerInstructions.Count < regCount)
                                registerInstructions.AddRange(
                                        registers.
                                            Skip(registerInstructions.Count).
                                            Take(registers.Count - registerInstructions.Count - 1).
                                            Select(p => new InstructionReg(new NumberedVariable(p.Index, p), function, this, labelIndex++)));

                            res.ForEach(p => p.Label = labelIndex++);

                            instructionSet.Instructions.AddRange(res);

                            var lastInstruction = (ArithmeticIntsruction)instructionSet.Instructions.Last();
                            var actualResult = locals.Find(p => p.Index == ((VarLexeme)lexeme).Index).Variable;

                            if (!lastInstruction.Result.Type.Equals(actualResult.Type))
                                throw new ParseException(lexeme.Tokens[0], CompileErrorType.IncompatibleTypes);

                            lastInstruction.Result = actualResult;
                        }
                    }
                }

                instructionSet.Instructions.InsertRange(function.LocalVariables.Count, registerInstructions);
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