using System.Collections.Generic;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionLdi : Instruction
    {
        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string InstructionName => "ldi";
        public override int ParameterCount => 2;
        public override string SourceValue()
        {
            throw new System.NotImplementedException();
        }

        public InstructionLdi(Function func, ByteCode byteCode, int label) : base(func, byteCode, label) { }
    }
}