namespace Nevermind.Compiler.LexemeParsing
{
    internal enum LexemeType
    {
        Block,

        Import,
        Module,
        Var,
        If,
        Function,
        Expression,

        Unknown,
        Return,
    }
}