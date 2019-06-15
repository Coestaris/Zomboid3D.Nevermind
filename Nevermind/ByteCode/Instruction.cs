using System.Collections.Generic;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode
{
    internal abstract class Instruction
    {
        public int Label;
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
            return $"   _{Function.Name + Label,-6}:  {SourceValue()}";
        }

        protected string ToSourceValue(params object[] objects)
        {
            return $"{InstructionName,-6} {string.Join(", ", objects)}";
        }

        protected string ToFunctionLabel(int index)
        {
            return $"_{Function.Name}{index}";
        }
    }
}