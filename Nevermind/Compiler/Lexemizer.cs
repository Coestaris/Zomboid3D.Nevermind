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

        private static BlockLexeme GetStructureLexeme(List<Token> tokens)
        {
            var iterator = new TokenIterator<Token>(tokens);
            var possibleLexeme = new List<Token>();
            var currentParent = new BlockLexeme(null, null);

            while (iterator.GetNext() != null)
            {
                if (iterator.Current.Type == TokenType.Semicolon ||
                    iterator.Current.Type == TokenType.BraceOpened ||
                    iterator.Current.Type == TokenType.BraceClosed)
                {
                    currentParent.ChildLexemes.Add(new UnknownLexeme(possibleLexeme));
                    possibleLexeme.Clear();

                    switch (iterator.Current.Type)
                    {
                        case TokenType.BraceOpened:

                            var oldParent = currentParent;
                            currentParent = new BlockLexeme(oldParent, oldParent.ChildLexemes.Last());
                            oldParent.ChildLexemes.Add(currentParent);

                            break;
                        case TokenType.BraceClosed:

                            currentParent = (BlockLexeme)currentParent.Parent;
                            break;
                    }
                }
                else
                {
                    possibleLexeme.Add(iterator.Current);
                }
            }

            return currentParent;
        }

        private static void ClearEmptyLexemes(BlockLexeme root)
        {
            var toDelete = new List<int>();
            int index = 0;
            foreach (var lexeme in root.ChildLexemes)
            {
                if (lexeme.Type == LexemeType.Block) ClearEmptyLexemes((BlockLexeme)lexeme);
                else
                {
                    if(lexeme.Tokens.Count == 0)
                        toDelete.Add(index);
                }
                index++;
            }
            toDelete.Reverse();

            foreach (var ind in toDelete)
                root.ChildLexemes.RemoveAt(ind);
        }

        private static void ResolveLexemeTypes(BlockLexeme root)
        {
            for(var i = 0; i < root.ChildLexemes.Count; i++)
            {
                var lexeme = root.ChildLexemes[i];

                if (lexeme.Type == LexemeType.Block) ResolveLexemeTypes((BlockLexeme)lexeme);
                else if(lexeme.Type == LexemeType.Unknown)
                {

                    var matchedLexemes = new List<LexemeInfo>();
                    foreach (var lexemeInfo in Lexeme.Lexemes)
                    {
                        int c1 = 0, c2 = 0;
                        while (c1 < lexeme.Tokens.Count && c2 < lexemeInfo.TokenTypes.Count)
                        {
                            if (lexemeInfo.TokenTypes[c2].HasFlag(lexeme.Tokens[c1].Type))
                            {
                                if (c2 != lexemeInfo.TokenTypes.Count - 1 &&
                                    lexemeInfo.TokenTypes[c2].HasFlag(TokenType.ComplexToken) &&
                                    !lexemeInfo.TokenTypes[c2 + 1].HasFlag(TokenType.ComplexToken) &&
                                    lexemeInfo.TokenTypes[c2 + 1].HasFlag(lexeme.Tokens[c1].Type))
                                {
                                    c2++;
                                }

                                c1++;
                            }
                            else
                            {
                                if(c1 == 0) break;
                                c2++;
                            }
                        }

                        if(c1 == lexeme.Tokens.Count && c2 == lexemeInfo.TokenTypes.Count - 1) matchedLexemes.Add(lexemeInfo);
                    }

                    //Console.WriteLine(string.Join(" ", possibleLexeme.Select(p => p.ToSource())));
                    if (matchedLexemes.Count != 0)
                    {
                        root.ChildLexemes[i] = (Lexeme)Activator.CreateInstance(matchedLexemes[0].LexemeType, lexeme.Tokens);
                    }
                    else
                    {
                        throw new ArgumentException("Unable to lexemize");
                    }
                }
            }
        }

        private static void PrintLexemeTree(Lexeme root, int level)
        {
            foreach (var lexeme in root.ChildLexemes)
            {
                if (lexeme.Type == LexemeType.Block) PrintLexemeTree((BlockLexeme) lexeme, level + 1);
                else Console.WriteLine("{0}-{1}", new string(' ', level * 3), lexeme);
            }
        }

        public static List<Lexeme> Lexemize(List<Token> tokens)
        {
            var root = GetStructureLexeme(tokens);
            ClearEmptyLexemes(root);
            ResolveLexemeTypes(root);

            PrintLexemeTree(root, 0);

            return null;
        }
    }
}