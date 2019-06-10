using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class ReturnLexeme : Lexeme
    {
        public ExpressionLexeme Expression;

        public ReturnLexeme(List<Token> tokens) : base(tokens, LexemeType.Return, false)
        {
            Expression = new ExpressionLexeme(tokens.Skip(1).ToList());
        }
    }
}