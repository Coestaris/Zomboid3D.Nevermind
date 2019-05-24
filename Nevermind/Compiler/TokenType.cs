namespace Nevermind.Compiler
{
    internal enum TokenType
    {
        ImportKeyword,
        VarKeyword,
        IfKeyword,

        Identifier,
        Number,
        FloatNumber,

        Quote,
        Semicolon,
        Colon,

        BracketOpen,
        BracketClosed,

        EqualSign,
        PlusSign,
        MultiplySign,

        BraceOpened,
        BraceClosed
    }
}