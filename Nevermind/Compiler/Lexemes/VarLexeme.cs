using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class VarLexeme : Lexeme
    {
        public VarLexeme(List<Token> tokens) : base(tokens, LexemeType.Var)
        {
        }
    }
}