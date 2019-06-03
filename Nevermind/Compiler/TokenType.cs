using System;
using System.Collections.Generic;

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

        MinusSign = 262144,
        DivideSign = 524288,
        GreaterSign = 1048576,
        LessThanSign = 2097152,
        Tilda = 4194304,
        AmpersandSign = 8388608,
        OrSign = 16777216,
        CircumflexSign = 33554432,
        PercentSign = 67108864,
        QuestingSign = 134217728,
        ExclamationMark = 268435456,

        ComplexToken = 131072,
        ComaSign = 536870912
    }

    internal static class TokenTypeExtensions
    {
        public static bool HasFlagFast(this TokenType value, TokenType flag)
        {
            return (value & flag) != 0;
        }

        public static IEnumerable<TokenType> GetFlags(this TokenType value)
        {
            foreach (Enum v in Enum.GetValues(typeof(TokenType)))
                if (value.HasFlag(v))
                    yield return (TokenType)v;
        }

        public static string ToSource(this TokenType type, string stringValue = null)
        {
            switch (type)
            {
                case TokenType.ImportKeyword:
                    return "import";
                case TokenType.VarKeyword:
                    return "var";
                case TokenType.IfKeyword:
                    return "if";
                case TokenType.FunctionKeyword:
                    return "function";
                case TokenType.Identifier:
                    return stringValue != null ? $"<identifier:{stringValue}>" : "<identifier>";
                case TokenType.Number:
                    return stringValue != null ? $"<number:{stringValue}>" : "<number>";
                case TokenType.FloatNumber:
                    return stringValue != null ? $"<float:{stringValue}>" : "<float>";
                case TokenType.Quote:
                    return "\"";
                case TokenType.Semicolon:
                    return ";";
                case TokenType.Colon:
                    return ":";
                case TokenType.BracketOpen:
                    return "(";
                case TokenType.BracketClosed:
                    return ")";
                case TokenType.EqualSign:
                    return "=";
                case TokenType.PlusSign:
                    return "+";
                case TokenType.MultiplySign:
                    return "*";
                case TokenType.BraceOpened:
                    return "{";
                case TokenType.BraceClosed:
                    return "}";

                case TokenType.MinusSign:
                    return "-";
                case TokenType.DivideSign:
                    return "/";
                case TokenType.GreaterSign:
                    return ">";
                case TokenType.LessThanSign:
                    return "<";
                case TokenType.Tilda:
                    return "~";
                case TokenType.AmpersandSign:
                    return "&";
                case TokenType.OrSign:
                    return "|";
                case TokenType.CircumflexSign:
                    return "^";
                case TokenType.PercentSign:
                    return "%";
                case TokenType.QuestingSign:
                    return "?";
                case TokenType.ExclamationMark:
                    return "!";
                case TokenType.ComaSign:
                    return ",";

                case TokenType.ComplexToken:
                    return "<complex>";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}