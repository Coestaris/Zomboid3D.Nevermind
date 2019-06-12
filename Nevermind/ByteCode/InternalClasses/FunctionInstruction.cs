using Nevermind.ByteCode.Functions;
using System.Collections.Generic;

namespace Nevermind.ByteCode.InternalClasses
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
}