using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler
{
    internal static class Tokenizer
    {
        private static readonly List<char> TokenSplitCharacters = new List<char>()
        {
            '\n', ' ', '.', '(', ')', ':', ';', '"', '=', '}', '{', ','
        };

        private struct RawToken
        {
            public string Token;
            public int LineIndex;
            public int charIndex;

            public RawToken(string token, int lineIndex, int charIndex)
            {
                Token = token;
                LineIndex = lineIndex;
                this.charIndex = charIndex;
            }
        }

        public static List<Token> Tokenize(string input)
        {
            return Tokenize(input, null);
        }

        public static List<Token> Tokenize(string input, string fileName)
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
                tokens.Add(new Token(token.Token, fileName, token.LineIndex, token.charIndex));
            }

            return tokens;
        }
    }
}