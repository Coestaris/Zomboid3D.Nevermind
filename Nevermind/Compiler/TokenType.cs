using System;
using System.Collections.Generic;

namespace Nevermind.Compiler
{
    [Flags]
    internal enum TokenType : ulong
    {
        ImportKeyword         = (ulong)(1UL <<  0),
        VarKeyword            = (ulong)(1UL <<  1),
        IfKeyword             = (ulong)(1UL <<  2),
        FunctionKeyword       = (ulong)(1UL <<  3),

        Identifier            = (ulong)(1UL <<  4),
        Number                = (ulong)(1UL <<  5),

        FloatNumber           = (ulong)(1UL <<  6),
        Quote                 = (ulong)(1UL <<  7),
        Semicolon             = (ulong)(1UL <<  8),
        Colon                 = (ulong)(1UL <<  9),

        BracketOpen           = (ulong)(1UL << 10),
        BracketClosed         = (ulong)(1UL << 11),

        EqualSign             = (ulong)(1UL << 12),
        PlusSign              = (ulong)(1UL << 13),
        MultiplySign          = (ulong)(1UL << 14),

        BraceOpened           = (ulong)(1UL << 15),
        BraceClosed           = (ulong)(1UL << 16),

        MinusSign             = (ulong)(1UL << 17),
        DivideSign            = (ulong)(1UL << 18),
        GreaterSign           = (ulong)(1UL << 19),
        LessThanSign          = (ulong)(1UL << 20),
        Tilda                 = (ulong)(1UL << 21),
        AmpersandSign         = (ulong)(1UL << 22),
        OrSign                = (ulong)(1UL << 23),
        CircumflexSign        = (ulong)(1UL << 24),
        PercentSign           = (ulong)(1UL << 25),
        QuestingSign          = (ulong)(1UL << 26),
        ExclamationMark       = (ulong)(1UL << 27),

        ComaSign              = (ulong)(1UL << 28),

        ModuleKeyword         = (ulong)(1UL << 29),
        PrivateKeyword        = (ulong)(1UL << 30),
        PublicKeyword         = (ulong)(1UL << 31),
        InitializationKeyword = (ulong)(1UL << 32),
        FinalizationKeyword   = (ulong)(1UL << 33),
        EntrypointKeyword     = (ulong)(1UL << 34),
        ReturnKeyword         = (ulong)(1UL << 35),

        StringToken           = (ulong)(1UL << 36),

        ComplexToken          = (ulong)(1UL << 37)
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
                case TokenType.StringToken:
                    return stringValue != null ? $"<string:{stringValue}>" : "<string>";
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

                case TokenType.ModuleKeyword:
                    return "module";
                case TokenType.PrivateKeyword:
                    return "private";
                case TokenType.PublicKeyword:
                    return "public";
                case TokenType.InitializationKeyword:
                    return "initialization";
                case TokenType.FinalizationKeyword:
                    return "finalization";
                case TokenType.EntrypointKeyword:
                    return "entrypoint";
                case TokenType.ReturnKeyword:
                    return "return";

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}