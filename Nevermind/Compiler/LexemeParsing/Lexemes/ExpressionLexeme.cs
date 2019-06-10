using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.Compiler.LexemeParsing.Lexemes.Expressions;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
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
            Operator lastUnaryOperator = null;
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
                    if(lastUnaryOperator != null)
                        lastParent.UnaryOperators.Add(lastUnaryOperator);

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
                        lastParent.SubTokens[0].UnaryOperators.AddRange(lastParent.UnaryOperators);
                        lastParent.SubTokens[0].LOperator = lastParent.LOperator;
                        lastParent.SubTokens[0].ROperator = lastParent.ROperator;
                    }

                    if(lastOperator != null)
                        throw new ParseException(iterator.Current, CompileErrorType.OperatorWithoutOperand);

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

                        if (matchedOperators.Count == 0)
                        {
                            //Cant find full matches, try to find partial ones
                            foreach (var op in Operator.Operators.FindAll(p => p.IsUnary))
                            {
                                var c1 = 0;
                                while (c1 < op.OperatorTypes.Count)
                                {
                                    if (op.OperatorTypes[c1]
                                        .HasFlag(possibleOperator[possibleOperator.Count - c1 - 1]))
                                    {
                                        c1++;
                                    }
                                    else
                                    {
                                         break;
                                    }
                                }

                                if (c1 == op.OperatorTypes.Count)
                                    matchedOperators.Add(op);
                            }

                            if(matchedOperators.Count == 0)
                                throw new ParseException(iterator.Current, CompileErrorType.UnknownOperator);

                            if(possibleOperator.Count - matchedOperators[0].OperatorTypes.Count == 0)
                                throw new ParseException(iterator.Current, CompileErrorType.UnknownOperator);

                            var binaryOp = possibleOperator.Take(possibleOperator.Count - matchedOperators[0].OperatorTypes.Count).ToList();
                            Operator binary = null;

                            foreach (var op in Operator.Operators)
                            {
                                var c1 = 0;
                                while (c1 < op.OperatorTypes.Count && c1 < binaryOp.Count)
                                {
                                    if (op.OperatorTypes[c1].HasFlag(binaryOp[c1]))
                                        c1++;
                                    else break;
                                }

                                if (c1 == op.OperatorTypes.Count && c1 == binaryOp.Count)
                                {
                                    binary = op;
                                    break;
                                }
                            }

                            if(binary == null)
                                throw new ParseException(iterator.Current, CompileErrorType.UnknownOperator);

                            Console.Write(binary + " ");
                            Console.WriteLine(matchedOperators[0]);

                            if(lastParent.SubTokens.Last().ROperator != null)
                                throw new ParseException(iterator.Current, CompileErrorType.MultipleOperators);

                            lastParent.SubTokens.Last().ROperator = binary;
                            lastOperator = binary;
                            lastUnaryOperator = matchedOperators[0];
                        }
                        else
                        {
                            Console.WriteLine(matchedOperators[0]);

                            Operator unary = matchedOperators.Find(p => p.IsUnary);

                            if (lastParent.SubTokens.Count == 0 && unary == null)
                                throw new ParseException(iterator.Current, CompileErrorType.OperatorWithoutOperand);

                            if (unary != null && (lastParent.SubTokens.Count == 0 || lastParent.SubTokens.Last().ROperator != null))
                            {
                                lastUnaryOperator = unary;
                            }
                            else
                            {
                                if(lastParent.SubTokens.Last().ROperator != null)
                                    throw new ParseException(iterator.Current, CompileErrorType.MultipleOperators);

                                lastParent.SubTokens.Last().ROperator = matchedOperators[0];
                                lastOperator = matchedOperators[0];
                            }

                        }

                    }
                    else
                    {
                        if (iterator.Current.Type == TokenType.Number ||
                            iterator.Current.Type == TokenType.FloatNumber ||
                            iterator.Current.Type == TokenType.StringToken ||
                            iterator.Current.Type == TokenType.Identifier)
                        {
                            if (iterator.Current.Type == TokenType.Number || iterator.Current.Type == TokenType.FloatNumber || iterator.Current.Type == TokenType.StringToken ||
                               (iterator.Current.Type == TokenType.Identifier && iterator.Index != tokens.Count - 1 && tokens[iterator.Index + 1].Type != TokenType.BracketOpen) |
                               (iterator.Current.Type == TokenType.Identifier && iterator.Index == tokens.Count - 1))
                            {
                                if(!first && lastOperator == null && lastUnaryOperator == null)
                                    throw new ParseException(iterator.Current, CompileErrorType.UnexpectedToken);

                                first = false;

                                lastParent.SubTokens.Add(new ExpressionToken(iterator.Current));
                                lastParent.SubTokens.Last().LOperator = lastOperator;
                                lastParent.SubTokens.Last().UnaryOperators.Add(lastUnaryOperator);
                                lastUnaryOperator = null;
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
                            throw new ParseException(iterator.Current, CompileErrorType.WrongTokenInExpression);
                        }
                    }
                }
            }

            if(level != 0)
                throw new ParseException(lastBracketClosed, CompileErrorType.WrongExpresionStructure);

            if(lastOperator != null)
                throw new ParseException(iterator.Current, CompileErrorType.OperatorWithoutOperand);

            Root = lastParent;
        }

        public List<ExpressionLineItem> ToList()
        {
            int index = 0;
            return ToList(ref index, Root);
        }

        private static List<ExpressionLineItem> ToList(ref int resultIndex, ExpressionToken root)
        {
            var result = new List<ExpressionLineItem>();

            if (root.SubTokens.Count == 1)
                return null;

            foreach (var token in root.SubTokens)
            {
                if (token.SubTokens.Count != 0)
                    result.AddRange(ToList(ref resultIndex, token));
            }

            while (root.SubTokens.Count != 1)
            {
                var maxOperatorIndex = 0;
                var priority = -1;
                for (var i = 0; i < root.SubTokens.Count - 1; i++)
                {
                    if (root.SubTokens[i].ROperator.Priority > priority)
                    {
                        priority = root.SubTokens[i].ROperator.Priority;
                        maxOperatorIndex = i;
                    }
                }

                if (root.SubTokens[maxOperatorIndex].CalculatedIndex != -1 &&
                    root.SubTokens[maxOperatorIndex + 1].CalculatedIndex != -1)
                {
                    result.Add(new ExpressionLineItem(root.SubTokens[maxOperatorIndex].ROperator,
                        root.SubTokens[maxOperatorIndex].CalculatedIndex,
                        root.SubTokens[maxOperatorIndex + 1].CalculatedIndex, resultIndex++));
                }
                else
                {
                    if (root.SubTokens[maxOperatorIndex].CalculatedIndex != -1)
                    {
                        result.Add(new ExpressionLineItem(root.SubTokens[maxOperatorIndex].ROperator,
                            root.SubTokens[maxOperatorIndex].CalculatedIndex,
                            root.SubTokens[maxOperatorIndex + 1], resultIndex++));
                    }
                    else  if (root.SubTokens[maxOperatorIndex + 1].CalculatedIndex != -1)
                    {
                        result.Add(new ExpressionLineItem(root.SubTokens[maxOperatorIndex].ROperator,
                            root.SubTokens[maxOperatorIndex],
                            root.SubTokens[maxOperatorIndex + 1].CalculatedIndex, resultIndex++));
                    }
                    else
                    {
                        result.Add(new ExpressionLineItem(root.SubTokens[maxOperatorIndex].ROperator,
                            root.SubTokens[maxOperatorIndex],
                            root.SubTokens[maxOperatorIndex + 1], resultIndex++));
                    }

                }

                root.SubTokens.RemoveAt(maxOperatorIndex);
                root.SubTokens[maxOperatorIndex].CalculatedIndex = resultIndex - 1;
            }

            root.CalculatedIndex = root.SubTokens[0].CalculatedIndex;

            return result;
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