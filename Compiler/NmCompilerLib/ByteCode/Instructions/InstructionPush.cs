using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionPush : Instruction
    {
        public Variable Variable;

        public override List<byte> Serialize() => ToBytes(
            Variable.Serialize()
            );

        public override InstructionType Type => InstructionType.Push;

        public override string InstructionName => "push";

        public override int ParameterCount => 0;

        public override string SourceValue() => ToSourceValue(Variable.ToSourceValue());

        public override bool UsesVariable(int index) => Variable.Index == index;

        public override List<Variable> FetchUsedVariables(int index) => InnerFetch(index, Variable);

        public InstructionPush(Variable variable, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Variable = variable;
        }
    }
}