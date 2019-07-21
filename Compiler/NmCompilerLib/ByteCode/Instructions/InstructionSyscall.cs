using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionSyscall : Instruction
    {
        public int Index;

        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(Index)
        );

        public override InstructionType Type => InstructionType.Syscall;

        public override string InstructionName => "syscall";

        public override int ParameterCount => 0;

        public override string SourceValue() => ToSourceValue($"{Index} ({Codes.SyscallTypes.FirstOrDefault(p => p.Value == Index).Key})");

        public override bool UsesVariable(int index) => false;

        public override List<Variable> FetchUsedVariables(int index) => InnerFetch(index, null);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex) { }

        public InstructionSyscall(int index, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Index = index;
        }
    }
}