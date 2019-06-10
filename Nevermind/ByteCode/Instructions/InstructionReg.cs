using System.Collections.Generic;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionReg : Instruction
    {
        public readonly NumberedVariable Variable;

        public InstructionReg(NumberedVariable var, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
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