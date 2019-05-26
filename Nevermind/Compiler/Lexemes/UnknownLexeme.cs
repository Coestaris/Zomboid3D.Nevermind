using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class UnknownLexeme : Lexeme
    {
        public UnknownLexeme(List<Token> tokens) : base(tokens, LexemeType.Unknown, false)
        {
        }
    }
}