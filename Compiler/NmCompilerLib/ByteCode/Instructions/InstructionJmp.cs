using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionJmp : Instruction
    {
        public int Index;

        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(Index)
        );

        public override InstructionType Type => InstructionType.Jmp;

        public override string InstructionName => "jmp";

        public override int ParameterCount => 0;

        public override string SourceValue() => ToSourceValue(ToFunctionLabel(Index));

        public override bool UsesVariable(int index) => false;

        public override List<Variable> FetchUsedVariables(int index) => InnerFetch(index, null);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex) { }

        public InstructionJmp(int index, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Index = index;
        }
    }
}