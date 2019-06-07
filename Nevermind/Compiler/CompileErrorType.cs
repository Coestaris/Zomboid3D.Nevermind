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
        UnexpectedToken,
        WrongIdentifierFormat,
        UnknownEscapeSymbol,
        WrongEscapeCodeFormat,
        ExpectedEscapeSymbol,
        WrongModuleNameFormat,
        MultipleInitializationFunctions,
        WrongFunctionNameFormat,
        WrongFunctionParameterNameFormat,
        MultipleFinalizationFunctions,
        MultipleEntrypointFunctions,
        NoEntrypointFunction,
        NoFinalizationFunction,
        NoInitializationFunction,
        MultipleFunctionsWithSameName,
        ModuleWithEntrypointFunction,
        VariableRedeclaration,
        UnexpectedLexeme,
        UnknownTypeName
    }
}