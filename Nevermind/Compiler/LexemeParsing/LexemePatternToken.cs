using System.Collections.Generic;

namespace Nevermind.Compiler.LexemeParsing
{
    internal class LexemePatternToken
    {
        public readonly bool Required;
        public readonly bool Greedy;
        public readonly TokenType Type;

        public LexemePatternToken(TokenType type, bool isRequired = true, bool greedy = false)
        {
            Greedy = greedy;
            Required = isRequired;
            Type = type;
        }

        public static LexemeInfo GetMatchedLexeme(List<Token> tokens)
        {
            foreach (var lexemeInfo in Lexeme.Lexemes)
                if (MatchLexeme(lexemeInfo.Pattern, tokens))
                    return lexemeInfo;
            return null;
        }

        private static bool MatchLexeme(List<LexemePatternToken> pattern, List<Token> tokens)
        {
            int patternCounter = 0;
            int tokenCounter = 0;

            while (patternCounter < pattern.Count && tokenCounter < tokens.Count)
            {
                if (pattern[patternCounter].Type.HasFlag(TokenType.ComplexToken))
                {
                    if (!pattern[patternCounter].Greedy)
                    {
                        if (patternCounter != pattern.Count - 1)
                        {
                            TokenType tokenToSeek = pattern[patternCounter + 1].Type;
                            while (
                                pattern[patternCounter].Type.HasFlag(tokens[tokenCounter].Type) && 
                                !tokens[tokenCounter].Type.HasFlag(tokenToSeek))
                            {
                                tokenCounter++;
                            }

                            if (tokens[tokenCounter++].Type.HasFlag(tokenToSeek))
                                patternCounter++;
                            else return false;
                        }
                        else
                        {
                            while (pattern[patternCounter].Type.HasFlag(tokens[tokenCounter].Type))
                            {
                                tokenCounter++;
                                if (tokenCounter == tokens.Count)
                                    return true;
                            }
                            return false;
                        }
                    }
                    else
                    {
                        int seekStart = tokenCounter;
                        while (pattern[patternCounter].Type.HasFlag(tokens[tokenCounter].Type) && tokenCounter < tokens.Count - 1)
                        {
                            tokenCounter++;
                        }

                        patternCounter++;
                        while (!pattern[patternCounter].Type.HasFlag(tokens[tokenCounter].Type))
                        {
                            tokenCounter--;
                            if (tokenCounter < seekStart)
                                return false;
                        }
                    }
                }
                else
                {
                    if (pattern[patternCounter].Type.HasFlag(tokens[tokenCounter].Type))
                    {
                        if (patternCounter != pattern.Count - 1 &&
                            pattern[patternCounter + 1] != pattern[patternCounter] &&
                            !pattern[patternCounter].Type.HasFlag(TokenType.ComplexToken))
                        {
                            patternCounter++;
                        }

                        tokenCounter++;
                    }
                    else
                    {
                        patternCounter++;
                        if (patternCounter > tokenCounter)
                        {
                            if (pattern[patternCounter - 1].Required) break;
                        }
                    }
                }
            }

            return (tokenCounter == tokens.Count && patternCounter == pattern.Count - 1);
        }
    }
}