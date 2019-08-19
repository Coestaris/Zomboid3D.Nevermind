using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionRet : Instruction
    {
        public override List<byte> Serialize() => ToBytes();

        public override string InstructionName => "ret";

        public override InstructionType Type => InstructionType.Ret;

        public override string SourceValue() => ToSourceValue();

        public override bool UsesVariable(int index) => false;

        public override List<Variable> FetchUsedVariables(int index) => InnerFetch(index, null);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex) { }

        public InstructionRet(Function func, ByteCode byteCode, int label) : base(func, byteCode, label) { }
    }
}