using System;
using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class ExpressionLexeme : Lexeme
    {
        public ExpressionToken Root;

        public ExpressionLexeme(List<Token> tokens) : base(tokens, LexemeType.Expression, false)
        {
            var lastParent = new ExpressionToken(null);
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.BracketOpen)
                {
                    var oldParent = lastParent;
                    lastParent = new ExpressionToken(null);
                    lastParent.Parent = oldParent;
                    oldParent.SubTokens.Add(lastParent);

                }
                else if (token.Type == TokenType.BracketClosed)
                {
                    lastParent = lastParent.Parent;
                }
                else
                {
                    lastParent.SubTokens.Add(new ExpressionToken(token));
                }
            }

            Root = lastParent;
        }

        public void PrintExpression(int level, ExpressionToken token = null)
        {
            if (token == null)
                token = Root;

            if(Root.SubTokens.Count == 0)
                return;

            Console.Write("{0} - Expression[", new string(' ', level * 3));
            foreach (var subToken in token.SubTokens)
                Console.Write(subToken);
            Console.WriteLine("]");

            foreach (var subToken in token.SubTokens)
                if(subToken.SubTokens.Count != 0) PrintExpression(level + 1, subToken);
        }

        public override void Print(int level)
        {
            base.Print(level);
            PrintExpression(level + 1);
        }
    }
}