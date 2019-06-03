namespace Nevermind.Compiler.LexemeParsing
{
    internal enum LexemeType
    {
        Block,

        Import,
        Module,
        Var,
        If,
        FunctionCall,
        Function,
        Expression,

        Unknown,
    }
}