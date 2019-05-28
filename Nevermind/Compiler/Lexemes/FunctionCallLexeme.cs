using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.Lexemes
{
    internal class FunctionCallLexeme : Lexeme
    {
        public Token Name;
        public ExpressionLexeme Expression;

        public FunctionCallLexeme(List<Token> tokens) : base(tokens, LexemeType.FunctionCall, false)
        {
            Name = tokens[0];
            Expression = new ExpressionLexeme(tokens.Skip(2).Take(tokens.Count - 3).ToList());
        }

        public override void Print(int level)
        {
            base.Print(level);
            Expression?.PrintExpression(level + 1);
        }
    }
}