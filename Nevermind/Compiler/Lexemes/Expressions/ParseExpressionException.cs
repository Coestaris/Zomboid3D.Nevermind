using System;

namespace Nevermind.Compiler.Lexemes.Expressions
{
    internal class ParseExpressionException : Exception
    {
        private readonly Token _token;
        private readonly CompileErrorType _errorType;

        public ParseExpressionException(Token token, CompileErrorType errorType)
        {
            _token = token;
            _errorType = errorType;
        }

        public CompileError ToError()
        {
            return new CompileError(_errorType, _token);
        }
    }
}