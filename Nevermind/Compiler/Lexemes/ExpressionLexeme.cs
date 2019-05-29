using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.Compiler.Lexemes.Expressions;

namespace Nevermind.Compiler.Lexemes
{
    internal class ExpressionLexeme : Lexeme
    {
        public ExpressionToken Root;

        public ExpressionLexeme(List<Token> tokens) : base(tokens, LexemeType.Expression, false)
        {
            var lastParent = new ExpressionToken(null);
            var iterator = new TokenIterator<Token>(tokens);

            var possibleOperator = new List<TokenType>();
            Token lastFunctionCallToken = null;

            while(iterator.GetNext() != null)
            {
                if (iterator.Current.Type == TokenType.BracketOpen)
                {
                    var oldParent = lastParent;
                    lastParent = new ExpressionToken(null);
                    if (lastFunctionCallToken != null)
                        lastParent.UnaryFunction = lastFunctionCallToken;

                    lastParent.Parent = oldParent;
                    oldParent.SubTokens.Add(lastParent);

                }
                else if (iterator.Current.Type == TokenType.BracketClosed)
                {
                    if (lastParent.SubTokens.Count == 1)
                    {
                        lastParent.Parent.SubTokens.Remove(lastParent);
                        lastParent.Parent.SubTokens.Add(lastParent.SubTokens[0]);
                        lastParent.SubTokens[0].UnaryFunction = lastParent.UnaryFunction;
                    }
                    lastParent = lastParent.Parent;
                }
                else
                {
                    if (Token.MathOperatorTokenType.HasFlag(iterator.Current.Type))
                    {
                        possibleOperator.Add(iterator.Current.Type);
                        while(iterator.Index != tokens.Count - 1 && Token.MathOperatorTokenType.HasFlag(tokens[iterator.Index + 1].Type))
                        {
                            possibleOperator.Add(iterator.GetNext().Type);
                        }


                    }
                    else
                    {
                        if (iterator.Current.Type == TokenType.Number || iterator.Current.Type == TokenType.FloatNumber || iterator.Current.Type == TokenType.Identifier)
                        {
                            if (iterator.Current.Type == TokenType.Number || iterator.Current.Type == TokenType.FloatNumber ||
                                iterator.Current.Type == TokenType.Identifier && iterator.Index != tokens.Count - 1 && tokens[iterator.Index + 1].Type != TokenType.BracketOpen)
                            {
                                lastParent.SubTokens.Add(new ExpressionToken(iterator.Current));
                            }

                            if (iterator.Current.Type == TokenType.Identifier && iterator.Index != tokens.Count - 1 &&
                                tokens[iterator.Index + 1].Type == TokenType.BracketOpen)
                                lastFunctionCallToken = iterator.Current;
                            else
                                lastFunctionCallToken = null;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
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