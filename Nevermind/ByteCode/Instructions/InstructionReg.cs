using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.InternalClasses;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionReg : Instruction
    {
        public readonly NumeratedVariable Variable;

        public InstructionReg(NumeratedVariable var, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Variable = var;
        }

        public override string InstructionName => "reg";
        public override int ParameterCount => 2;
        public override string SourceValue() =>
            ToSourceValue(Variable.Index, ByteCode.Header.GetTypeIndex(Variable.Variable.Type));

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }
    }
}