using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nevermind.Compiler.Formats;

namespace Nevermind.Compiler
{
    internal static class Tokenizer
    {
        private static List<char> _tokenSplitCharacters = null;

        private static readonly string SingleLineComment =     "//";
        private static readonly string MultilineCommentStart = "/*";
        private static readonly string MultilineCommentEnd =   "*/";

        private class RawToken
        {
            public readonly string Token;
            public readonly int LineIndex;
            public readonly int CharIndex;

            public bool IsString;

            public RawToken(string token, int lineIndex, int charIndex)
            {
                Token = token;
                LineIndex = lineIndex;
                CharIndex = charIndex;
                IsString = false;
            }

            public override string ToString()
            {
                return $"Raw token: {Token}";
            }
        }

        public static void InitTokenizer()
        {
            if (_tokenSplitCharacters != null) return;

            _tokenSplitCharacters = new List<char> { '\n', ' ',  ';', '"', '}', '{', '(', ')', ':', '[', ']' };
            _tokenSplitCharacters.AddRange(Token.MathOperatorTokenType.GetFlags().Select(p => p.ToSource()[0]));
        }

        public static List<Token> Tokenize(string input, NmProgram program)
        {
            return Tokenize(input, null, program);
        }

        public static List<Token> Tokenize(string input, string fileName, NmProgram program)
        {
            var rawTokens = new List<RawToken>();
            var currentToken = "";
            var lastContains = false;
            var lineCount = 0;
            var charCount = 0;

            var collectingString = false;
            var collectingComment = false;
            var collectingMlComment = false;
            var lastSymbolIsEscapeSymbol = false;

            var slCommentIndex = 0;
            var mlCommentIndex = 0;
            var str = "";

            foreach (var c in input)
            {
                if (!collectingComment)
                {
                    if (c == SingleLineComment[slCommentIndex])
                    {
                        slCommentIndex++;
                        if (slCommentIndex == SingleLineComment.Length)
                        {
                            collectingComment = true;
                            currentToken = "";
                            slCommentIndex = 0;
                            collectingMlComment = false;
                        }
                    }
                    else slCommentIndex = 0;

                    if (c == MultilineCommentStart[mlCommentIndex])
                    {
                        mlCommentIndex++;
                        if (mlCommentIndex == MultilineCommentStart.Length)
                        {
                            collectingComment = true;
                            currentToken = "";
                            mlCommentIndex = 0;
                            collectingMlComment = true;
                        }
                    }
                    else mlCommentIndex = 0;
                }

                if (collectingComment)
                {
                    if (collectingMlComment)
                    {
                        if (c == MultilineCommentEnd[mlCommentIndex])
                        {
                            mlCommentIndex++;
                            if (mlCommentIndex == MultilineCommentEnd.Length)
                            {
                                collectingComment = false;
                                mlCommentIndex = 0;
                            }
                        }
                        else mlCommentIndex = 0;
                    }
                    else
                    {
                        if (c == '\n') collectingComment = false;
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        if (collectingString)
                        {
                            if (!lastSymbolIsEscapeSymbol)
                            {
                                rawTokens.Add(new RawToken(str, lineCount, charCount));
                                rawTokens.Last().IsString = true;
                                str = "";
                                collectingString = false;
                            }
                            else
                            {
                                str += '"';
                            }
                        }
                        else
                        {
                            if (lastContains)
                            {
                                rawTokens.Add(new RawToken(currentToken, lineCount, charCount));
                                currentToken = "";
                            }

                            collectingString = true;
                        }
                    }
                    else
                    {
                        if (collectingString)
                        {
                            str += c;
                        }
                        else if (_tokenSplitCharacters.Contains(c))
                        {
                            rawTokens.Add(new RawToken(currentToken, lineCount, charCount));
                            currentToken = c.ToString();
                            lastContains = true;
                        }
                        else
                        {
                            if (lastContains)
                            {
                                rawTokens.Add(new RawToken(currentToken, lineCount, charCount));
                                currentToken = "";
                            }
                            lastContains = false;
                            currentToken += c;
                        }
                    }
                }

                lastSymbolIsEscapeSymbol = c == EscapeSymbolToken.EscapeSymbol;

                if (c == '\n')
                {
                    lineCount++;
                    charCount = 0;
                }
                else
                {
                    charCount++;
                }
            }

            if(lastContains)
                rawTokens.Add(new RawToken(currentToken, lineCount, charCount));

            rawTokens = rawTokens.FindAll(p => !string.IsNullOrWhiteSpace(p.Token.Trim()));

            var tokens = new List<Token>();
            foreach (var token in rawTokens)
            {
                tokens.Add(new Token(token.Token, fileName, token.LineIndex, token.CharIndex, program, token.IsString));
            }

            return tokens;
        }
    }
}