using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler
{
    internal static class Tokenizer
    {
        private static List<char> TokenSplitCharacters = null;
        private static readonly string SingleLineComment =     "//";
        private static readonly string MultilineCommentStart = "/*";
        private static readonly string MultilineCommentEnd =   "*/";

        private static Regex _singleLineCommentRegex;
        private static Regex _multilineCommentRegex;

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
        }

        public static void InitTokenizer()
        {
            if (TokenSplitCharacters != null) return;

            TokenSplitCharacters = new List<char> { '\n', ' ',  ';', '"', '}', '{', '(', ')', ':' };
            TokenSplitCharacters.AddRange(Token.MathOperatorTokenType.GetFlags().Select(p => p.ToSource()[0]));

            _singleLineCommentRegex = new Regex($"{Regex.Escape(SingleLineComment)}.*?(\n|$)",
                RegexOptions.Multiline);
            _multilineCommentRegex = new Regex($"{Regex.Escape(MultilineCommentStart)}(\\s|.)*?{Regex.Escape(MultilineCommentEnd)}",
                RegexOptions.Multiline);

        }

        public static List<Token> Tokenize(string input, NmProgram program)
        {
            return Tokenize(input, null, program);
        }

        public static string RemoveComments(string input)
        {
            input = _singleLineCommentRegex.Replace(input, "");
            input = _multilineCommentRegex.Replace(input, "");
            return input;
        }

        public static List<Token> Tokenize(string input, string fileName, NmProgram program)
        {
            input = RemoveComments(input);

            var rawTokens = new List<RawToken>();
            var currentToken = "";
            var lastContains = false;
            var lineCount = 0;
            var charCount = 0;

            var collectingString = false;
            var str = "";

            foreach (var c in input)
            {
                if (c == '"')
                {
                    if (collectingString)
                    {
                        rawTokens.Add(new RawToken(str, lineCount, charCount));
                        rawTokens.Last().IsString = true;
                        str = "";
                        collectingString = false;
                    }
                    else
                    {
                        collectingString = true;
                    }
                }
                else
                {
                    if (collectingString)
                    {
                        str += c;
                    }
                    else if (TokenSplitCharacters.Contains(c))
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