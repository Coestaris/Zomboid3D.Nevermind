using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.Compiler.Lexemes;

namespace Nevermind.Compiler
{
    internal class Lexemizer
    {
        private struct TokenIterator<T>
        {
            private readonly List<T> _list;
            public T Current;
            public int Index;

            public TokenIterator(List<T> list)
            {
                _list = list;
                Index = 0;
                Current = default(T);
            }

            public T GetNext()
            {
                if(Index > _list.Count - 1) return default(T);
                Current = _list[Index++];
                return Current;
            }
        }

        public static List<Lexeme> Lexemize(List<Token> tokens)
        {
            var iterator = new TokenIterator<Token>(tokens);
            var possibleLexeme = new List<Token>();

            while (iterator.GetNext() != null)
            {
                if(iterator.Current.Type == TokenType.Semicolon)
                {
                    var matchedLexemes = new List<LexemeInfo>();
                    foreach (var lexeme in Lexeme.Lexemes)
                    {
                        int count = 0;
                        for(var i = 0; i < Math.Min(lexeme.TokenTypes.Count, possibleLexeme.Count); i++)
                            if (lexeme.TokenTypes[i] == possibleLexeme[i].Type)
                                count++;

                        if(count == lexeme.TokenTypes.Count && count == possibleLexeme.Count)
                            matchedLexemes.Add(lexeme);
                    }

                    Console.WriteLine(string.Join(" ", possibleLexeme.Select(p => p.ToSource())));
                    if (matchedLexemes.Count == 0)
                    {
                        Console.WriteLine("Nothing =c");
                    }
                    else
                    {
                        Console.WriteLine("Count: {0} {1}", matchedLexemes.Count, matchedLexemes[0].ToString());
                    }
                    possibleLexeme.Clear();
                }
                else
                {
                    possibleLexeme.Add(iterator.Current);
                }
            }

            return null;
        }
    }
}