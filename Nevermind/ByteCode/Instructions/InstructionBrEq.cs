using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;
using Nevermind.ByteCode.Types;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionBrEq : InstructionJmp
    {
        public readonly Variable Variable;

        public override List<byte> Serialize() => ToBytes(
            Variable.Serialize(),
            Chunk.Int32ToBytes(Index)
        );

        public override InstructionType Type => InstructionType.BrEq;

        public override string InstructionName => "breq";
        public override int ParameterCount => 2;
        public override string SourceValue() =>
            ToSourceValue(Variable.ToSourceValue(), ToFunctionLabel(Index));

        public InstructionBrEq(Variable variable, int index, Function func, ByteCode byteCode, int label) : base(index, func, byteCode, label)
        {
            if (variable.Type.ID != TypeID.Float && variable.Type.ID != TypeID.Integer)
                throw new ParseException(CompileErrorType.IncompatibleTypes, variable.Token);

            Variable = variable;
        }
    }
}