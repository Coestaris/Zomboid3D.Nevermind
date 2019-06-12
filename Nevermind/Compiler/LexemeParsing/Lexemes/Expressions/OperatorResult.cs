using Nevermind.ByteCode;
using Nevermind.ByteCode.Types;

namespace Nevermind.Compiler.LexemeParsing.Lexemes.Expressions
{
    internal class OperatorResult
    {
        public readonly CompileError Error;
        public readonly Instruction Instruction;
        public readonly Type ResultType;

        public OperatorResult(CompileError error)
        {
            Error = error;
            Instruction = null;
        }

        public OperatorResult(Instruction instruction, Type resultType)
        {
            Error = null;
            Instruction = instruction;
            ResultType = resultType;
        }
    }
}