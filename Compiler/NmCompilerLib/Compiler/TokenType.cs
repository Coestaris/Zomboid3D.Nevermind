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

        SquareBracketOpen     = (ulong)(1UL << 12),
        SquareBracketClosed   = (ulong)(1UL << 13),

        EqualSign             = (ulong)(1UL << 14),
        PlusSign              = (ulong)(1UL << 15),
        MultiplySign          = (ulong)(1UL << 16),

        BraceOpened           = (ulong)(1UL << 17),
        BraceClosed           = (ulong)(1UL << 18),

        MinusSign             = (ulong)(1UL << 19),
        DivideSign            = (ulong)(1UL << 20),
        GreaterSign           = (ulong)(1UL << 21),
        LessThanSign          = (ulong)(1UL << 22),
        Tilda                 = (ulong)(1UL << 23),
        AmpersandSign         = (ulong)(1UL << 24),
        OrSign                = (ulong)(1UL << 25),
        CircumflexSign        = (ulong)(1UL << 26),
        PercentSign           = (ulong)(1UL << 27),
        QuestingSign          = (ulong)(1UL << 28),
        ExclamationMark       = (ulong)(1UL << 29),

        ComaSign              = (ulong)(1UL << 30),

        ModuleKeyword         = (ulong)(1UL << 31),
        LibraryKeyword        = (ulong)(1UL << 32),
        PrivateKeyword        = (ulong)(1UL << 33),
        PublicKeyword         = (ulong)(1UL << 34),
        InitializationKeyword = (ulong)(1UL << 35),
        FinalizationKeyword   = (ulong)(1UL << 36),
        EntrypointKeyword     = (ulong)(1UL << 37),
        ReturnKeyword         = (ulong)(1UL << 38),
        ElseKeyword           = (ulong)(1UL << 39),
        WhileKeyword          = (ulong)(1UL << 40),
        ForKeyword            = (ulong)(1UL << 41),

        StringToken           = (ulong)(1UL << 42),

        ComplexToken          = (ulong)(1UL << 43)
    }

    internal static class TokenTypeExtensions
    {
        public static IEnumerable<TokenType> GetFlags(this TokenType value)
        {
            foreach (Enum v in Enum.GetValues(typeof(TokenType)))
                if (value.HasFlag(v))
                    yield return (TokenType)v;
        }

        private static Dictionary<TokenType, string> _tokenDict = new Dictionary<TokenType, string>()
        {
            { TokenType.ImportKeyword, "import" },
            { TokenType.WhileKeyword, "while" },
            { TokenType.ForKeyword, "for" },
            { TokenType.VarKeyword, "var" },
            { TokenType.IfKeyword, "if" },
            { TokenType.FunctionKeyword, "function" },
            { TokenType.Identifier, "<identifier{0}>" },
            { TokenType.Number, "<number{0}>" },
            { TokenType.FloatNumber, "<float{0}>" },
            { TokenType.StringToken, "<string{0}>" },
            { TokenType.Quote, "\"" },
            { TokenType.Semicolon, " }," },
            { TokenType.Colon, ":" },
            { TokenType.BracketOpen, "(" },
            { TokenType.BracketClosed, ")" },
            { TokenType.EqualSign, "=" },
            { TokenType.PlusSign, "+" },
            { TokenType.MultiplySign, "*" },
            { TokenType.BraceOpened, "{" },
            { TokenType.BraceClosed, "}" },
            { TokenType.SquareBracketOpen, "[" },
            { TokenType.SquareBracketClosed, "]" },
            { TokenType.MinusSign, "-" },
            { TokenType.DivideSign, "/" },
            { TokenType.GreaterSign, ">" },
            { TokenType.LessThanSign, "<" },
            { TokenType.Tilda, "~" },
            { TokenType.AmpersandSign, "&" },
            { TokenType.OrSign, "|" },
            { TokenType.CircumflexSign, "^" },
            { TokenType.PercentSign, "%" },
            { TokenType.QuestingSign, "?" },
            { TokenType.ExclamationMark, "!" },
            { TokenType.ComaSign, "," },
            { TokenType.ComplexToken, "<complex>" },
            { TokenType.ModuleKeyword, "module" },
            { TokenType.LibraryKeyword, "library" },
            { TokenType.ElseKeyword, "else" },
            { TokenType.PrivateKeyword, "private" },
            { TokenType.PublicKeyword, "public" },
            { TokenType.InitializationKeyword, "initialization" },
            { TokenType.FinalizationKeyword, "finalization" },
            { TokenType.EntrypointKeyword, "entrypoint" },
            { TokenType.ReturnKeyword, "return" },
        };

        public static string ToSource(this TokenType type, string stringValue = null)
        {
            var item = _tokenDict[type];
            if (item.Contains("{")) return string.Format(item, ":" + stringValue ?? "--" );
            else return item;
        }
    }
}