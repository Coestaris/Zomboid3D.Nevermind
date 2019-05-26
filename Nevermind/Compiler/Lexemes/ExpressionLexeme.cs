using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class ExpressionLexeme : Lexeme
    {
        public ExpressionLexeme(List<Token> tokens) : base(tokens, LexemeType.Expression)
        {
        }
    }
}