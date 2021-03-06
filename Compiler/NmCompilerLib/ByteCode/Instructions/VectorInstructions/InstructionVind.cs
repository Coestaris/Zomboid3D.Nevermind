using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.Instructions.VectorInstructions
{
    internal class InstructionVind : Instruction
    {
        public Variable Index;

        public override List<byte> Serialize() => Index.Serialize().ToList();

        public override InstructionType Type => InstructionType.Vind;

        public override string InstructionName => "vind";

        public override string SourceValue() => ToSourceValue(Index.ToSourceValue());

        public override bool UsesVariable(int index) => Index.Index == index;

        public override List<Variable> FetchUsedVariables(int index) => InnerFetch(index, Index);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex)
        {
            if (Index.Index == oldIndex) Index = Index.Clone(newIndex);
        }

        public InstructionVind(Variable index, Function func, ByteCode byteCode,
            int label) : base(func, byteCode, label)
        {
            Index = index;
        }
    }
}