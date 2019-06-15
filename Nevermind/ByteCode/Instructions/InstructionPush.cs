using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionPush : Instruction
    {
        public Variable Variable;

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string InstructionName => "push";
        public override int ParameterCount => 0;
        public override string SourceValue() => ToSourceValue(Variable.Index);

        public InstructionPush(Variable variable, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Variable = variable;
        }
    }
}