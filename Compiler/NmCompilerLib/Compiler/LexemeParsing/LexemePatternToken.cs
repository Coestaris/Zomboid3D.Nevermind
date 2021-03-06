using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Nevermind.Compiler.LexemeParsing.Lexemes;

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
                        //last or last required
                        if (patternCounter != pattern.Count - 1 && pattern.Skip(patternCounter).All(p => p.Required))
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

                            if (!pattern[patternCounter].Required)
                            {
                                patternCounter++;
                                continue;
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

            var lastNotRequiredTokens = 0;
            for (int i = pattern.Count - 1; i >= 0; i--)
            {
                if (!pattern[i].Required)
                    lastNotRequiredTokens++;
                else
                    break;
            }


            return (tokenCounter == tokens.Count && patternCounter == pattern.Count - 1 ||
                    tokenCounter == tokens.Count && patternCounter == pattern.Count - lastNotRequiredTokens);
        }
    }
}