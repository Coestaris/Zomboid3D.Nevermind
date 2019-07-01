using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionPop : ArithmeticInstruction
    {
        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(Result.Index)
        );

        public override InstructionType Type => InstructionType.Pop;

        public override string InstructionName => "pop";
        public override int ParameterCount => 0;
        public override string SourceValue() => ToSourceValue(Result.ToSourceValue());

        public InstructionPop(Variable result, Function func, ByteCode byteCode, int label) : base(result, func, byteCode, label) { }
    }
}