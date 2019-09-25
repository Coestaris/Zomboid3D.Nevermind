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
            { TypeID.String,   0x4 },
            { TypeID.Array,    0x5 }
        };

        public static readonly Dictionary<InstructionType, UInt16> InstructionDict =
            new Dictionary<InstructionType, ushort>
            {
                { InstructionType.Ret,     0x1 },
                { InstructionType.Push,    0x2 },
                { InstructionType.Pop,     0x3 },
                { InstructionType.Ldi,     0x4 },
                { InstructionType.Jmp,     0x5 },
                { InstructionType.Call,    0x6 },
                { InstructionType.BrEq,    0x7 },
                { InstructionType.Cast,    0x8 },
                { InstructionType.Vget,    0x9 },
                { InstructionType.Vset,    0xA },
                { InstructionType.Syscall, 0xB },
                { InstructionType.Vect,    0xC },
                { InstructionType.Vind,    0xD },
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

        public static readonly Dictionary<string, UInt16> SyscallTypes = new Dictionary<string, ushort>
        {
            //io
            { "io_print_i", 0x1 },
            { "io_print_f", 0x2 },
            { "io_print_s", 0x3 },
            { "io_print",   0x4 },
            { "io_fprint",  0x5 },

            //sys
            { "sys_get_time", 0x100 },
            { "sys_get_gpc",  0x101 },
            { "sys_get_pc",   0x102 },
            { "sys_get_fc",   0x103 },

            //math
            { "m_acos",   0x200 },
            { "m_asin",   0x201 },
            { "m_atan",   0x202 },
            { "m_atan2",  0x203 },
            { "m_ceil",   0x204 },
            { "m_cos",    0x205 },
            { "m_cosh",   0x206 },
            { "m_exp",    0x207 },
            { "m_fabs",   0x208 },
            { "m_floor",  0x209 },
            { "m_fmod",   0x20A },
            { "m_log",    0x20B },
            { "m_log10",  0x20C },
            { "m_log2",   0x20D },
            { "m_pow",    0x20E },
            { "m_sin",    0x20F },
            { "m_sinh",   0x210 },
            { "m_sqrt",   0x211 },
            { "m_tan",    0x212 },
            { "m_tanh",   0x213 },

            { "m_acosf",  0x214 },
            { "m_asinf",  0x215 },
            { "m_atanf",  0x216 },
            { "m_atan2f", 0x217 },
            { "m_ceilf",  0x218 },
            { "m_cosf",   0x219 },
            { "m_coshf",  0x21A },
            { "m_expf",   0x21B },
            { "m_fabsf",  0x21C },
            { "m_floorf", 0x21D },
            { "m_fmodf",  0x21E },
            { "m_logf",   0x21F },
            { "m_log10f", 0x220 },
            { "m_log2f",  0x221 },
            { "m_powf",   0x222 },
            { "m_sinf",   0x223 },
            { "m_sinhf",  0x224 },
            { "m_sqrtf",  0x225 },
            { "m_tanf",   0x226 },
            { "m_tanhf",  0x227 },

            { "m_acosl",  0x228 },
            { "m_asinl",  0x229 },
            { "m_atanl",  0x22A },
            { "m_atan2l", 0x22B },
            { "m_ceill",  0x22C },
            { "m_cosl",   0x22D },
            { "m_coshl",  0x22E },
            { "m_expl",   0x22F },
            { "m_fabsl",  0x230 },
            { "m_floorl", 0x231 },
            { "m_fmodl",  0x232 },
            { "m_logl",   0x233 },
            { "m_log10l", 0x234 },
            { "m_log2l",  0x235 },
            { "m_powl",   0x236 },
            { "m_sinl",   0x237 },
            { "m_sinhl",  0x238 },
            { "m_sqrtl",  0x239 },
            { "m_tanl",   0x23A },
            { "m_tanhl",  0x23B },

            //array
            { "array_resize",  0x300 },
            { "array_free",    0x301 },
            { "array_print",   0x302 },
        };
    }
}
