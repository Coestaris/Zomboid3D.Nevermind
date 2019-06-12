using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.InternalClasses;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionLocal : Instruction
    {
        private readonly NumeratedVariable _variable;

        public InstructionLocal(NumeratedVariable var, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            _variable = var;
        }

        public override string InstructionName => "local";
        public override int ParameterCount => 2;
        public override string SourceValue() =>
            ToSourceValue(_variable.Index, ByteCode.Header.GetTypeIndex(_variable.Variable.Type));

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }
    }
}