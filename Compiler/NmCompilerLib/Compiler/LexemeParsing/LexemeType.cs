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
        Else,

        Unknown
    }
}