using System.Collections.Generic;
using Nevermind.ByteCode;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Types;

namespace Nevermind.Compiler.LexemeParsing.Lexemes.Expressions
{
    internal class OperatorResult
    {
        public readonly CompileError Error;
        public readonly Instruction Instruction;
        public readonly Type ResultType;
        internal readonly List<Variable> ResultTuple;

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

        public OperatorResult(List<Variable> resultTuple) : this(null, null)
        {
            ResultTuple = resultTuple;
        }
    }
}