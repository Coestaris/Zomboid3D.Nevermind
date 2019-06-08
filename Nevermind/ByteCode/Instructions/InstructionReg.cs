using System.Collections.Generic;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionReg : Instruction
    {
        public override string InstructionName => "reg";
        public override int ParameterCount => 2;
        public override string SourceValue()
        {
            throw new System.NotImplementedException();
        }

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public InstructionReg(Function func, ByteCode byteCode, int label) : base(func, byteCode, label) { }
    }
}