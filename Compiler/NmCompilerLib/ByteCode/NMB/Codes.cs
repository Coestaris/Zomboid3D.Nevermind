using System;
using System.Collections.Generic;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.Types;

namespace Nevermind.ByteCode.NMB
{
    internal static class Codes
    {
        public const UInt16 CurrentNMVersion = 1050;

        public static byte[] NMBSignature = {(byte) 'N', (byte) 'M', (byte) 'B'};

        public static readonly Dictionary<TypeID, UInt16> TypeIdDict = new Dictionary<TypeID, ushort>
        {
            { TypeID.Integer,  0x1 },
            { TypeID.UInteger, 0x2 },
            { TypeID.Float,    0x3 },
            { TypeID.String,   0x4 }
        };

        public static readonly Dictionary<InstructionType, UInt16> InstructionDict =
            new Dictionary<InstructionType, ushort>
            {
                { InstructionType.Ret,  0x1 },
                { InstructionType.Push, 0x2 },
                { InstructionType.Pop,  0x3 },
                { InstructionType.Ldi,  0x4 },
                { InstructionType.Jmp,  0x5 },
                { InstructionType.Call, 0x6 },
                { InstructionType.BrEq, 0x7 },
                { InstructionType.Cast, 0x8 }
            };

        public static readonly Dictionary<BinaryArithmeticInstructionType, UInt16> ABInstructionDict =
            new Dictionary<BinaryArithmeticInstructionType, ushort>()
            {
                { BinaryArithmeticInstructionType.A_Add,       0x64 },
                { BinaryArithmeticInstructionType.A_Sub,       0x65 },
                { BinaryArithmeticInstructionType.A_Mul,       0x66 },
                { BinaryArithmeticInstructionType.A_Div,       0x67 },
                { BinaryArithmeticInstructionType.A_lseq,      0x68 },
                { BinaryArithmeticInstructionType.A_ls,        0x69 },
                { BinaryArithmeticInstructionType.A_gr,        0x6A },
                { BinaryArithmeticInstructionType.A_greq,      0x6B },
                { BinaryArithmeticInstructionType.A_neq,       0x6C },
                { BinaryArithmeticInstructionType.A_eq,        0x6D },
                { BinaryArithmeticInstructionType.A_EDiv,      0x6E },
                { BinaryArithmeticInstructionType.A_LAnd,      0x6F },
                { BinaryArithmeticInstructionType.A_LOr,       0x70 },
                { BinaryArithmeticInstructionType.A_And,       0x71 },
                { BinaryArithmeticInstructionType.A_Xor,       0x72 },
                { BinaryArithmeticInstructionType.A_Or,        0x73 },
                { BinaryArithmeticInstructionType.A_lsh,       0x74 },
                { BinaryArithmeticInstructionType.A_rlh,       0x75 },
                { BinaryArithmeticInstructionType.A_SetAdd,    0x76 },
                { BinaryArithmeticInstructionType.A_SetSub,    0x77 },
                { BinaryArithmeticInstructionType.A_SetMul,    0x78 },
                { BinaryArithmeticInstructionType.A_SetDiv,    0x79 },
                { BinaryArithmeticInstructionType.A_SetEDiv,   0x7A },
                { BinaryArithmeticInstructionType.A_SetAnd,    0x7B },
                { BinaryArithmeticInstructionType.A_SetXor,    0x7C },
                { BinaryArithmeticInstructionType.A_SetOr,     0x7D },
                { BinaryArithmeticInstructionType.A_Set,       0x7E }
            };

        public static readonly Dictionary<UnaryArithmeticInstructionType, UInt16> AUInstructionDict =
            new Dictionary<UnaryArithmeticInstructionType, ushort>()
            {
                { UnaryArithmeticInstructionType.A_Neg,    0x100 },
                { UnaryArithmeticInstructionType.A_Not,    0x101 },
                { UnaryArithmeticInstructionType.A_BNeg,   0x102 },
            };
    }
}
