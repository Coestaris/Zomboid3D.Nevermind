namespace Nevermind.Compiler
{
    public enum CompileErrorType
    {
        UnknownToken,
        WrongCodeStructure,
        EmptyFile,
        UnknownLexeme,
        LexemeWithoutRequiredBlock
    }
}