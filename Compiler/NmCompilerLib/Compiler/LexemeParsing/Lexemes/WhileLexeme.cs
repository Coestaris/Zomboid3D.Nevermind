using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class WhileLexeme : ComplexLexeme
    {
        public ExpressionLexeme Expression;

        public WhileLexeme(List<Token> tokens) : base(tokens, LexemeType.While, true)
        {
            Expression = new ExpressionLexeme(tokens.Skip(2).Take(tokens.Count - 3).ToList());
        }

        public override void Print(int level)
        {
            base.Print(level);
            Expression?.PrintExpression(level + 1);
        }
    }
}