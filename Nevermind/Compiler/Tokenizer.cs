using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler
{
    internal static class Tokenizer
    {
        private static List<char> TokenSplitCharacters = null;

        private struct RawToken
        {
            public readonly string Token;
            public readonly int LineIndex;
            public readonly int CharIndex;

            public RawToken(string token, int lineIndex, int charIndex)
            {
                Token = token;
                LineIndex = lineIndex;
                CharIndex = charIndex;
            }
        }

        public static void InitTokenizer()
        {
            if (TokenSplitCharacters != null) return;

            TokenSplitCharacters = new List<char> { '\n', ' ',  ';', '"', '}', '{', '(', ')', ':' };
            TokenSplitCharacters.AddRange(Token.MathOperatorTokenType.GetFlags().Select(p => p.ToSource()[0]));
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

            foreach (var c in input)
            {
                if (TokenSplitCharacters.Contains(c))
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
                tokens.Add(new Token(token.Token, fileName, token.LineIndex, token.CharIndex, program));
            }

            return tokens;
        }
    }
}