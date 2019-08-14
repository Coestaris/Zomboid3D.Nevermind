using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionPushI : Instruction
    {
        public int Value;

        public override List<byte> Serialize() => ToBytes(
                Chunk.Int32ToBytes(Value)
            );

        public override InstructionType Type => InstructionType.Push;

        public override string InstructionName => "push";

        public override int ParameterCount => 0;

        public override string SourceValue() => ToSourceValue(Value);

        public override bool UsesVariable(int index) => false;

        public override List<Variable> FetchUsedVariables(int index) => null;

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex) {}

        public InstructionPushI(int value, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Value = value;
        }
    }
}