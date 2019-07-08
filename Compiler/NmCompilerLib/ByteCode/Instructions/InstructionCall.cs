using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionCall: Instruction
    {
        public Function DestFunc;

        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(DestFunc.ModuleIndex),
            Chunk.Int32ToBytes(DestFunc.Index)
        );

        public override InstructionType Type => InstructionType.Call;

        public override string InstructionName => "call";

        public override int ParameterCount => 0;

        public override string SourceValue() => ToSourceValue(
            $"{DestFunc.ModuleIndex}({(DestFunc.ModuleIndex != -1 ? DestFunc.Program.Module.Name : "self")})",
            $"{DestFunc.Index}({DestFunc.Name})");

        public override bool UsesVariable(int index) => false;

        public override List<Variable> FetchUsedVariables(int index) => InnerFetch(index, null);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex) { }

        public InstructionCall(Function destFunc, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            DestFunc = destFunc;
        }
    }
}