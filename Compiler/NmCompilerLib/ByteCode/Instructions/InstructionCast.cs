using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.NMB;
using Nevermind.ByteCode.Types;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionCast : ArithmeticInstruction
    {
        public Variable Source;

        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(Result.Index),
            Source.Serialize()
        );

        public override InstructionType Type => InstructionType.Cast;

        public override string InstructionName => "cast";

        public override string SourceValue() => ToSourceValue(
            Result.ToSourceValue(), Source.ToSourceValue() + $" (to {ByteCode.Header.GetTypeIndex(Result.Type)} = {Result.Type.ID}:{Result.Type.GetBase()})");

        public override bool UsesVariable(int index) => base.UsesVariable(index) || Source.Index == index;

        public override List<Variable> FetchUsedVariables(int index) => InnerFetch(index, Result, Source);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex)
        {
            base.ReplaceRegisterUsage(oldIndex, newIndex);
            if (Source.Index == oldIndex) Source = Source.Clone(newIndex);
        }

        public InstructionCast(Variable dest, Variable source, Function func, ByteCode byteCode, int label) : base(dest, func, byteCode, label)
        {
            Source = source;
        }
    }
}