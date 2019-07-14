using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionVset : Instruction
    {
        public Variable Array;
        public Variable Index;
        public Variable Source;

        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(Array.Index),
            Index.Serialize(),
            Source.Serialize()
        );

        public override InstructionType Type => InstructionType.Vset;

        public override string InstructionName => "vset";

        public override int ParameterCount => 0;

        public override string SourceValue() => ToSourceValue(
            Array.ToSourceValue(), Index.ToSourceValue(), Source.ToSourceValue());

        public override bool UsesVariable(int index) =>
            Array.Index == index || Index.Index == index || Source.Index == index;

        public override List<Variable> FetchUsedVariables(int index) =>
            InnerFetch(index, Array, Index, Source);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex)
        {
            if (Array.Index == oldIndex) Array = Array.Clone(newIndex);
            if (Index.Index == oldIndex) Index = Index.Clone(newIndex);
            if (Source.Index == oldIndex) Source = Source.Clone(newIndex);
        }

        public InstructionVset(Variable array, Variable index, Variable source, Function func, ByteCode byteCode,
            int label) : base(func, byteCode, label)
        {
            Array = array;
            Index = index;
            Source = source;
        }
    }
}