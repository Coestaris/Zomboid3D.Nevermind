using System;

namespace Nevermind.Compiler
{
    [Flags]
    internal enum TokenType
    {
        ImportKeyword = 1,
        VarKeyword = 2,
        IfKeyword = 4,
        FunctionKeyword = 8,

        Identifier = 16,
        Number = 32,
        FloatNumber = 64,

        Quote = 128,
        Semicolon = 256,
        Colon = 512,

        BracketOpen = 1024,
        BracketClosed = 2048,

        EqualSign = 4096,
        PlusSign = 8192,
        MultiplySign = 16384,

        BraceOpened = 32768,
        BraceClosed = 65536,

        ComplexToken = 131072,
    }

    internal static class TokenTypeExtensions
    {
        public static bool HasFlagFast(this TokenType value, TokenType flag)
        {
            return (value & flag) != 0;
        }
    }
}