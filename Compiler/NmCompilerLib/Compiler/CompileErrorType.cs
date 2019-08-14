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
        UnknownTypeName,
        ExpectedNumericOperands,
        WrongResultType,
        UndefinedReference,
        WrongOperandList,
        IncompatibleTypes,
        IncompatibleTypeBases,
        ElseWithoutCondition,
        WrongAssignmentOperation,
        ReturnInVoidFunction,
        WrongParameterCount,
        UnexpectedCommaOperator,
        OutOfBoundsConstant,
        ExpressionIsNotVariable,
        WrongUsageOfVoidFunc,
        UnknownModuleName,
        InnerCompileException,
        NotModuleImport,
        PrivateExportFunction,
        ModuleFunctionCall,
        RecursiveImport,
        ExpectedArrayType,
        ExpectedIntegerIndex,
        WrongIndicesCount,
        UnknownAttributeType,
        UnknownSyscallType,
        WrongAttributeParameterCount,
        WrongSyscallAppendType,
        UnknownCountMode,
        WrongCountValue,
        WrongParameterIndexFormat,
        UnknownVariableTypeFormat,
        UnknownVariableBaseFormat,
        WrongResrtictOptions
    }
}