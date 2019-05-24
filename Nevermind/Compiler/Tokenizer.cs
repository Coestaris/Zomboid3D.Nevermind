using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler
{
    internal static class Tokenizer
    {
        public static List<char> tokenSplitCharacters = new List<char>()
        {
            '\n', ' ', '.', '(', ')', ':', ';', '"', '=',
        };

        public static List<Token> Tokenize(string input)
        {
            var rawTokens = new List<string>();
            var currentToken = "";
            var lastContains = false;
            foreach (var c in input)
            {
                if (tokenSplitCharacters.Contains(c))
                {
                    rawTokens.Add(currentToken);
                    currentToken = c.ToString();
                    lastContains = true;
                }
                else
                {
                    if (lastContains)
                    {
                        rawTokens.Add(currentToken);
                        currentToken = "";
                    }
                    lastContains = false;
                    currentToken += c;
                }
            }

            rawTokens = rawTokens.FindAll(p => !string.IsNullOrWhiteSpace(p.Trim()));

            var tokens = new List<Token>();
            foreach (var token in rawTokens)
            {
                tokens.Add(new Token(token));
                Console.WriteLine(tokens.Last().ToString());
            }

            return tokens;
        }
    }
}