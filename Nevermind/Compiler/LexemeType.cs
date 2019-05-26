namespace Nevermind.Compiler
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