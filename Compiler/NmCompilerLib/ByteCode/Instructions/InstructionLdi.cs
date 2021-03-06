using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionLdi : ArithmeticInstruction
    {
        public Variable Src;

        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(Result.Index),
            Src.Serialize()
        );

        public override InstructionType Type => InstructionType.Ldi;

        public override string InstructionName => "ldi";

        public override string SourceValue() =>
            ToSourceValue(Result.ToSourceValue(), Src.ToSourceValue());

        public override bool UsesVariable(int index) => base.UsesVariable(index) || Src.Index == index;

        public override List<Variable> FetchUsedVariables(int index) =>
            InnerFetch(index, Result, Src);

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex)
        {
            base.ReplaceRegisterUsage(oldIndex, newIndex);
            if (Src.Index == oldIndex) Src = Src.Clone(newIndex);
        }

        public InstructionLdi(Variable src, Variable dst, Function func, ByteCode byteCode, int label) : base(dst, func, byteCode, label)
        {
            if (dst.Type.ID != src.Type.ID)
                throw new CompileException(CompileErrorType.IncompatibleTypes, src.Token);

            if (dst.Type.GetBase() < src.Type.GetBase())
                throw new CompileException(CompileErrorType.IncompatibleTypeBases, src.Token);

            Src = src;
        }
    }
}