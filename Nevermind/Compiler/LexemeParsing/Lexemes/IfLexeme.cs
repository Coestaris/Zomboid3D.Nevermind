using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class IfLexeme : ComplexLexeme
    {
        public ExpressionLexeme Expression;

        public IfLexeme(List<Token> tokens) : base(tokens, LexemeType.If, true)
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