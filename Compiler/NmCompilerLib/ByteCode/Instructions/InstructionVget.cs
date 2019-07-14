using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionVget : ArithmeticInstruction
    {
        public Variable Array;
        public Variable Index;

        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(Result.Index),
            Chunk.Int32ToBytes(Array.Index),
            Index.Serialize()
        );

        public override InstructionType Type => InstructionType.Vget;

        public override string InstructionName => "vget";

        public override int ParameterCount => 0;

        public override string SourceValue() => ToSourceValue(
            Result.ToSourceValue(),
            Array.ToSourceValue(),
            Index.ToSourceValue());

        public override bool UsesVariable(int index) =>
            base.UsesVariable(index) || Array.Index == index || Index.Index == index;

        public override List<Variable> FetchUsedVariables(int index) =>
            InnerFetch(index, Result, Array, Index);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex)
        {
            base.ReplaceRegisterUsage(oldIndex, newIndex);
            if (Array.Index == oldIndex) Array = Array.Clone(newIndex);
            if (Index.Index == oldIndex) Index = Index.Clone(newIndex);
        }

        public InstructionVget(Variable result, Variable array, Variable index, Function func, ByteCode byteCode,
            int label) : base(result, func, byteCode, label)
        {
            Array = array;
            Index = index;
        }
    }
}