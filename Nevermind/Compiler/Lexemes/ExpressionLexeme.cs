using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.Compiler.Lexemes.Expressions;

namespace Nevermind.Compiler.Lexemes
{
    internal class ExpressionLexeme : Lexeme
    {
        public readonly ExpressionToken Root;

        public ExpressionLexeme(List<Token> tokens) : base(tokens, LexemeType.Expression, false)
        {
            var lastParent = new ExpressionToken(null);
            var iterator = new TokenIterator<Token>(tokens);
            var level = 0;

            bool first = true;
            Operator lastOperator = null;
            Token lastBracketClosed = null;
            Token lastFunctionCallToken = null;

            while(iterator.GetNext() != null)
            {
                if (iterator.Current.Type == TokenType.BracketOpen)
                {
                    var oldParent = lastParent;
                    lastParent = new ExpressionToken(null);
                    if (lastFunctionCallToken != null)
                        lastParent.UnaryFunction = lastFunctionCallToken;

                    lastParent.LOperator = lastOperator;
                    lastOperator = null;

                    lastParent.Parent = oldParent;
                    oldParent.SubTokens.Add(lastParent);
                    first = true;
                    level++;
                }
                else if (iterator.Current.Type == TokenType.BracketClosed)
                {
                    if (lastParent.SubTokens.Count == 1)
                    {
                        lastParent.Parent.SubTokens.Remove(lastParent);
                        lastParent.Parent.SubTokens.Add(lastParent.SubTokens[0]);
                        lastParent.SubTokens[0].UnaryFunction = lastParent.UnaryFunction;
                        lastParent.SubTokens[0].LOperator = lastParent.LOperator;
                        lastParent.SubTokens[0].ROperator = lastParent.ROperator;
                    }

                    if(lastOperator != null)
                        throw new ParseExpressionException(iterator.Current, CompileErrorType.OperatorWithoutOperand);

                    lastBracketClosed = iterator.Current;
                    lastParent = lastParent.Parent;
                    level--;
                }
                else
                {
                    if (Token.MathOperatorTokenType.HasFlag(iterator.Current.Type))
                    {
                        var possibleOperator = new List<TokenType>();
                        var matchedOperators = new List<Operator>();

                        possibleOperator.Add(iterator.Current.Type);
                        while (iterator.Index != tokens.Count - 1 && Token.MathOperatorTokenType.HasFlag(tokens[iterator.Index + 1].Type))
                        {
                            possibleOperator.Add(iterator.GetNext().Type);
                        }

                        foreach (var op in Operator.Operators)
                        {
                            var c1 = 0;
                            while (c1 < op.OperatorTypes.Count && c1 < possibleOperator.Count)
                            {
                                if (op.OperatorTypes[c1].HasFlag(possibleOperator[c1]))
                                    c1++;
                                else break;
                            }

                            if (c1 == op.OperatorTypes.Count && c1 == possibleOperator.Count)
                                matchedOperators.Add(op);
                        }

                        if (matchedOperators.Count != 0)
                        {
                            //Console.WriteLine(matchedOperators[0]);

                            if (lastParent.SubTokens.Count == 0)
                                throw new ParseExpressionException(iterator.Current, CompileErrorType.OperatorWithoutOperand);

                            if(lastParent.SubTokens.Last().ROperator != null)
                                throw new ParseExpressionException(iterator.Current, CompileErrorType.MultipleOperators);

                            lastParent.SubTokens.Last().ROperator = matchedOperators[0];
                            lastOperator = matchedOperators[0];
                        }
                        else throw new ParseExpressionException(iterator.Current, CompileErrorType.UnknownOperator);
                    }
                    else
                    {
                        if (iterator.Current.Type == TokenType.Number || iterator.Current.Type == TokenType.FloatNumber || iterator.Current.Type == TokenType.Identifier)
                        {
                            if (iterator.Current.Type == TokenType.Number || iterator.Current.Type == TokenType.FloatNumber ||
                               (iterator.Current.Type == TokenType.Identifier && iterator.Index != tokens.Count - 1 && tokens[iterator.Index + 1].Type != TokenType.BracketOpen) |
                               (iterator.Current.Type == TokenType.Identifier && iterator.Index == tokens.Count - 1))
                            {
                                if(!first && lastOperator == null)
                                    throw new ParseExpressionException(iterator.Current, CompileErrorType.UnexpectedToken);

                                first = false;
                                lastParent.SubTokens.Add(new ExpressionToken(iterator.Current));
                                lastParent.SubTokens.Last().LOperator = lastOperator;
                                lastOperator = null;
                            }

                            if (iterator.Current.Type == TokenType.Identifier && iterator.Index != tokens.Count - 1 &&
                                tokens[iterator.Index + 1].Type == TokenType.BracketOpen)
                                lastFunctionCallToken = iterator.Current;
                            else
                                lastFunctionCallToken = null;
                        }
                        else
                        {
                            throw new ParseExpressionException(iterator.Current, CompileErrorType.WrongTokenInExpression);
                        }
                    }
                }
            }

            if(level != 0)
                throw new ParseExpressionException(lastBracketClosed, CompileErrorType.WrongExpresionStructure);

            if(lastOperator != null)
                throw new ParseExpressionException(iterator.Current, CompileErrorType.OperatorWithoutOperand);

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