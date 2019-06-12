using System;

namespace Nevermind.Compiler
{
    internal class ParseException : Exception
    {
        private readonly Token _token;
        private readonly CompileErrorType _errorType;

        public ParseException(CompileError error)
        {
            _token = error.Token;
            _errorType = error.ErrorType;
        }

        public ParseException(Token token, CompileErrorType errorType)
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