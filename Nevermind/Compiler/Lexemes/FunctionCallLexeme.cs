using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class FunctionCallLexeme : Lexeme
    {
        public FunctionCallLexeme(List<Token> tokens) : base(tokens, LexemeType.FunctionCall)
        {
        }

    }
}