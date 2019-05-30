namespace Nevermind.Compiler
{
    public enum CompileErrorType
    {
        UnknownToken,
        WrongCodeStructure,
        EmptyFile,
        UnknownLexeme,
        LexemeWithoutRequiredBlock,
        UnableToOpenFile,
        UnknownOperator,
        WrongExpresionStructure,
        WrongTokenInExpression,
        OperatorWithoutOperand,
        MultipleOperators,
        UnexpectedToken
    }
}