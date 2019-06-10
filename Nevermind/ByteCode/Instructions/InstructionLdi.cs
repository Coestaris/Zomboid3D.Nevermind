using System.Collections.Generic;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionLdi : Instruction
    {
        public readonly Variable Dest;
        public readonly Variable Src;

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string InstructionName => "ldi";
        public override int ParameterCount => 2;
        public override string SourceValue() =>
            ToSourceValue(Dest.Index, Src.ToSourceValue());

        public InstructionLdi(Variable src, Variable dst, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Dest = dst;
            Src = src;
        }
    }
}