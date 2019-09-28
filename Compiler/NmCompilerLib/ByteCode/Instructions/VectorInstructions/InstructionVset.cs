using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;

namespace Nevermind.ByteCode.Instructions.VectorInstructions
{
    internal class InstructionVset : Instruction
    {
        internal Variable _array;
        internal List<Variable> _indices;

        public Variable Src;
        public bool Expanded;

        public override List<byte> Serialize() => Src.Serialize().ToList();

        public override InstructionType Type => InstructionType.Vset;

        public override string InstructionName => "vset";

        public override string SourceValue() => ToSourceValue(Src.ToSourceValue());

        public override bool UsesVariable(int index) => Src.Index == index;

        public override List<Variable> FetchUsedVariables(int index) => InnerFetch(index, Src);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex)
        {
            if (Src.Index == oldIndex) Src = Src.Clone(newIndex);
        }

        public InstructionVset(Variable array, Variable src, List<Variable> indices, Function func, ByteCode byteCode,
            int label) : base(func, byteCode, label)
        {
            Src = src;

            _indices = indices;
            _array = array;
        }
    }
}