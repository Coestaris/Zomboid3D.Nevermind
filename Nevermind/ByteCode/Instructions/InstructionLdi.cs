using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionLdi : Instruction
    {
        public readonly Variable Dest;
        public readonly Variable Src;

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string InstructionName => "ldi";
        public override int ParameterCount => 2;
        public override string SourceValue() =>
            ToSourceValue(Dest.Index, Src.ToSourceValue());

        public InstructionLdi(Variable src, Variable dst, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            if (dst.Type.ID != src.Type.ID)
                throw new ParseException(src.Token, CompileErrorType.IncompatibleTypes);

            if (dst.Type.GetBase() < src.Type.GetBase())
                throw new ParseException(src.Token, CompileErrorType.IncompatibleTypeBases);

            Dest = dst;
            Src = src;
        }
    }
}