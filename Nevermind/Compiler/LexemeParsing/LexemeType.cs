namespace Nevermind.Compiler.LexemeParsing
{
    internal enum LexemeType
    {
        Block,

        Import,
        Var,
        If,
        FunctionCall,
        Function,
        Expression,

        Unknown,
    }
}