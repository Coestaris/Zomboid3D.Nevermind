using Nevermind.ByteCode.Functions;
using System.Collections.Generic;
using Nevermind.ByteCode.NMB;

namespace Nevermind.ByteCode.Instructions.ArithmeticInstructions
{
    internal class UnaryArithmeticInstruction : ArithmeticInstruction
    {
        public readonly Variable Operand;

        public readonly UnaryArithmeticInstructionType AType;

        public override string InstructionName => AType.ToString().ToLower();

        public override int ParameterCount => 2;

        public override string SourceValue() => ToSourceValue(Result?.Index ?? -1, Operand.ToSourceValue());

        public override List<byte> Serialize() => ToBytes(
            Chunk.Int32ToBytes(Result.Index),
                Operand.Serialize()
        );

        public override InstructionType Type => InstructionType._Unary;

        public override bool UsesVariable(int index) =>
            base.UsesVariable(index) || Operand.Index == index;

        public override List<Variable> FetchUsedVariables(int index) =>
            InnerFetch(index, Result, Operand);

        public UnaryArithmeticInstruction(UnaryArithmeticInstructionType type, Variable res, Variable a, Function func, ByteCode byteCode, int label) : base(res, func, byteCode, label)
        {
            AType = type;
            Operand = a;
        }
    }
}
