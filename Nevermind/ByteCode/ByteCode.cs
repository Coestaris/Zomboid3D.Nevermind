using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.LexemeParsing.Lexemes;

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
            Proceed();
        }

        private void Proceed()
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

                Instructions.Add(instructionSet);

                foreach (var lexeme in function.RawLexeme.ChildLexemes)
                {
                    if (lexeme.Type == LexemeType.Var)
                    {
                        var list = ((VarLexeme) lexeme).Expression.ToList();
                        if(list != null)
                            Console.WriteLine(string.Join("\n", list));
                    }
                }
            }
        }

        public string ToSource()
        {
            var sb = new StringBuilder();
            sb.Append(Header.ToSource());
            sb.AppendLine();
            foreach (var instruction in Instructions)
            {
                sb.AppendFormat(".{0}\n", instruction.Function.Name);
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