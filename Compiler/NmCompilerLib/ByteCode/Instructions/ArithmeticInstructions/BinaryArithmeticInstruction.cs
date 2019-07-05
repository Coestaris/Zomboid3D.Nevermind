using Nevermind.ByteCode.Functions;
using System;
using System.Collections.Generic;
using Nevermind.ByteCode.NMB;

namespace Nevermind.ByteCode.Instructions.ArithmeticInstructions
{

    internal class BinaryArithmeticInstruction : ArithmeticInstruction
    {
        public readonly Variable Operand1;
        public readonly Variable Operand2;

        public readonly BinaryArithmeticInstructionType AType;

        public override string InstructionName => AType.ToString().ToLower();

        public override int ParameterCount => 3;

        public override string SourceValue() =>
              ToSourceValue(Result?.Index ?? -1,
                  Operand1.ToSourceValue(),
                  Operand2.ToSourceValue());

        public override List<byte> Serialize() => ToBytes(
                Chunk.Int32ToBytes(Result.Index),
                Operand1.Serialize(),
                Operand2.Serialize()
            );

        public override bool UsesVariable(int index) =>
            base.UsesVariable(index) || Operand1.Index == index || Operand2.Index == index;

        public BinaryArithmeticInstruction(BinaryArithmeticInstructionType type, Variable res, Variable a, Variable b, Function func, ByteCode byteCode, int label) : base(res, func, byteCode, label)
        {
            AType = type;
            Operand1 = a;
            Operand2 = b;
        }

        public bool CanBeSimplified()
        {
            return AType >= BinaryArithmeticInstructionType.A_SetAdd &&
                   AType <= BinaryArithmeticInstructionType.A_Set;
        }

        public override InstructionType Type => InstructionType._Binary;

        public Instruction Simplify()
        {
            switch (AType)
            {
                case BinaryArithmeticInstructionType.A_SetAdd:
                    return new BinaryArithmeticInstruction(BinaryArithmeticInstructionType.A_Add, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticInstructionType.A_SetSub:
                    return new BinaryArithmeticInstruction(BinaryArithmeticInstructionType.A_Sub, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticInstructionType.A_SetMul:
                    return new BinaryArithmeticInstruction(BinaryArithmeticInstructionType.A_Mul, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticInstructionType.A_SetDiv:
                    return new BinaryArithmeticInstruction(BinaryArithmeticInstructionType.A_Div, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticInstructionType.A_SetEDiv:
                    return new BinaryArithmeticInstruction(BinaryArithmeticInstructionType.A_EDiv, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticInstructionType.A_SetAnd:
                    return new BinaryArithmeticInstruction(BinaryArithmeticInstructionType.A_And, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticInstructionType.A_SetXor:
                    return new BinaryArithmeticInstruction(BinaryArithmeticInstructionType.A_Xor, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticInstructionType.A_SetOr:
                    return new BinaryArithmeticInstruction(BinaryArithmeticInstructionType.A_Or, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticInstructionType.A_Set:
                    return new InstructionLdi(Operand2, Operand1, Function, ByteCode, Label);
            }
            return null;
        }
    }
}
