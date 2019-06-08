using System.Collections.Generic;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode
{
    internal abstract class Instruction
    {
        public readonly int Label;
        public readonly ByteCode ByteCode;
        public readonly Function Function;

        public Instruction(Function func, ByteCode byteCode, int label)
        {
            Label = label;
            ByteCode = byteCode;
            Function = func;
        }

        public abstract List<byte> Serialize();

        public abstract string InstructionName { get; }
        public abstract int ParameterCount { get; }
        public abstract string SourceValue();

        public string ToSource()
        {
            return $"   _{Function.Name}{Label}: {SourceValue()}";
        }

        protected string ToSourceValue(params object[] objects)
        {
            return $"{InstructionName} {string.Join(", ", objects)}";
        }
    }
}