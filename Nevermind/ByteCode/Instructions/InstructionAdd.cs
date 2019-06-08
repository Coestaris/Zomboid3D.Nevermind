using System.Collections.Generic;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionAdd : Instruction
    {
        public override string InstructionName => "add";

        public override int ParameterCount => 3;

        public override string SourceValue()
        {
            throw new System.NotImplementedException();
        }

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public InstructionAdd(Function func, ByteCode byteCode, int label) : base(func, byteCode, label) { }
    }
}