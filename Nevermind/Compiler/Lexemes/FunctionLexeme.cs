using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.Lexemes
{
    internal class FunctionLexeme : Lexeme
    {
        public Token Name;
        public BlockLexeme Block;
        public ExpressionLexeme Parameters;

        public FunctionLexeme(List<Token> tokens) : base(tokens, LexemeType.Function, true)
        {
            Name = tokens[1];
            Parameters = new ExpressionLexeme(tokens.Skip(3).Take(tokens.Count - 4).ToList());
        }

        public override void Print(int level)
        {
            base.Print(level);
            Parameters.PrintExpression(level + 1);
            Block.Print(level + 1);
        }
    }
}