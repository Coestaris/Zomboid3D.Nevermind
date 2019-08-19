using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.Instructions.VectorInstructions
{
    internal class InstructionVect : Instruction
    {
        public Variable Array;

        public override List<byte> Serialize() =>
            Array.Serialize().ToList();

        public override InstructionType Type => InstructionType.Vect;

        public override string InstructionName => "vect";

        public override string SourceValue() => ToSourceValue(Array.ToSourceValue());

        public override bool UsesVariable(int index) => Array.Index == index;

        public override List<Variable> FetchUsedVariables(int index) => InnerFetch(index, Array);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex)
        {
            if (Array.Index == oldIndex) Array = Array.Clone(newIndex);
        }

        public InstructionVect(Variable array, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Array = array;
        }
    }
}