using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class FunctionLexeme : Lexeme
    {
        public FunctionLexeme(List<Token> tokens) : base(tokens, LexemeType.Function)
        {
        }
    }
}