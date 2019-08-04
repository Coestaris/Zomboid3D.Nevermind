namespace Nevermind.Compiler.LexemeParsing
{
    internal enum LexemeType
    {
        Block,

        Import,
        Module,
        Var,
        If,
        Attribute,
        Function,
        Expression,
        Return,
        While,
        Else,

        Unknown
    }
}