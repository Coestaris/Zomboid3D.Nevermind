using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class IfLexeme : Lexeme
    {
        public IfLexeme(List<Token> tokens) : base(tokens, LexemeType.If)
        {
        }

    }
}