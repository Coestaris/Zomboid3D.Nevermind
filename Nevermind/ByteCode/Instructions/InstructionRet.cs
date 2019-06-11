using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionRet : Instruction
    {
        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string InstructionName => "ret";
        public override int ParameterCount => 0;
        public override string SourceValue() => ToSourceValue();

        public InstructionRet(Function func, ByteCode byteCode, int label) : base(func, byteCode, label) { }
    }
}