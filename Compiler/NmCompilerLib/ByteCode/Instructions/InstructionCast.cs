using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;
using Nevermind.ByteCode.Types;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionCast : Instruction
    {
        public Variable Dest;
        public Variable Source;

        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(Dest.Index),
            Chunk.Int32ToBytes(Source.Index)
        );

        public override InstructionType Type => InstructionType.Cast;

        public override string InstructionName => "cast";

        public override int ParameterCount => 0;

        public override string SourceValue() => ToSourceValue(
            Dest.ToSourceValue(), Source.ToSourceValue() + $" (to {ByteCode.Header.GetTypeIndex(Dest.Type)} = {Dest.Type.ID}:{Dest.Type.GetBase()})");

        public InstructionCast(Variable dest, Variable source, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Dest = dest;
            Source = source;
        }
    }
}