using System;

namespace Nevermind.Compiler
{
    internal class CompileException : Exception
    {
        private readonly Token _token;
        private readonly Token _declarationToken;
        private readonly CompileErrorType _errorType;

        public CompileException(CompileError error)
        {
            _token = error.Token;
            _errorType = error.ErrorType;
        }

        public CompileException(CompileErrorType errorType, Token token = null, Token declarationToken = null)
        {
            _token = token;
            _errorType = errorType;
            _declarationToken = declarationToken;
        }

        public CompileError ToError()
        {
            return new CompileError(_errorType, _token, _declarationToken);
        }
    }
}