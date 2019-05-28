using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.Lexemes
{
    internal class IfLexeme : Lexeme
    {
        public BlockLexeme Block;
        public  ExpressionLexeme Expression;

        public IfLexeme(List<Token> tokens) : base(tokens, LexemeType.If, true)
        {
            Expression = new ExpressionLexeme(tokens.Skip(2).Take(tokens.Count - 3).ToList());
        }

        public override void Print(int level)
        {
            base.Print(level);
            Expression?.PrintExpression(level + 1);
            Block?.Print(level + 1);
        }
    }
}