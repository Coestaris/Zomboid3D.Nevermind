using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.Compiler.LexemeParsing.Lexemes;

namespace Nevermind.Compiler.LexemeParsing
{
    internal class Lexemizer
    {
        private static BlockLexeme GetStructureLexeme(List<Token> tokens, out CompileError error)
        {
            error = null;
            var iterator = new TokenIterator<Token>(tokens);
            var possibleLexeme = new List<Token>();
            var currentParent = new BlockLexeme(null, -1);

            int level = 0;
            Token lastClosed = null;

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
                            currentParent = new BlockLexeme(oldParent, iterator.Index);
                            oldParent.ChildLexemes.Add(currentParent);
                            level++;

                            break;
                        case TokenType.BraceClosed:

                            currentParent = (BlockLexeme)currentParent.Parent;
                            lastClosed = iterator.Current;
                            level--;

                            break;
                    }
                }
                else
                {
                    possibleLexeme.Add(iterator.Current);
                }
            }

            if(possibleLexeme.Count != 0)
                currentParent.ChildLexemes.Add(new UnknownLexeme(possibleLexeme));

            if(level != 0)
                error = new CompileError(CompileErrorType.WrongCodeStructure, lastClosed);

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

        private static void ResolveLexemeTypes(BlockLexeme root, out CompileError error)
        {
            error = null;
            for(var i = 0; i < root.ChildLexemes.Count; i++)
            {
                var lexeme = root.ChildLexemes[i];

                if (lexeme.Type == LexemeType.Block)
                {
                    ResolveLexemeTypes((BlockLexeme)lexeme, out error);
                    if(error != null) return;
                }
                else if(lexeme.Type == LexemeType.Unknown)
                {
                    var matchedLexemes = new List<LexemeInfo>();
                    foreach (var lexemeInfo in Lexeme.Lexemes)
                    {
                        int c1 = 0, c2 = 0;
                        while (c1 < lexeme.Tokens.Count && c2 < lexemeInfo.Pattern.Count)
                        {
                            if (lexemeInfo.Pattern[c2].Type.HasFlag(lexeme.Tokens[c1].Type))
                            {
                                if (c2 != lexemeInfo.Pattern.Count - 1 &&
                                    lexemeInfo.Pattern[c2].Type.HasFlag(TokenType.ComplexToken) &&
                                    !lexemeInfo.Pattern[c2 + 1].Type.HasFlag(TokenType.ComplexToken) &&
                                    lexemeInfo.Pattern[c2 + 1].Type.HasFlag(lexeme.Tokens[c1].Type))
                                {
                                    c2++;
                                }

                                if (c2 != lexemeInfo.Pattern.Count - 1 &&
                                    lexemeInfo.Pattern[c2 + 1] != lexemeInfo.Pattern[c2] &&
                                    !lexemeInfo.Pattern[c2].Type.HasFlag(TokenType.ComplexToken))
                                {
                                    c2++;
                                }

                                c1++;
                            }
                            else
                            {
                                c2++;
                                if (c2 > c1)
                                {
                                    if (lexemeInfo.Pattern[c2 - 1].IsRequired) break;
                                }
                            }
                        }

                        if(c1 == lexeme.Tokens.Count && c2 == lexemeInfo.Pattern.Count - 1) matchedLexemes.Add(lexemeInfo);
                    }

                    if (matchedLexemes.Count != 0)
                    {
                        try
                        {
                            root.ChildLexemes[i] = (Lexeme)Activator.CreateInstance(matchedLexemes[0].LexemeType, lexeme.Tokens);
                            if (root.ChildLexemes.Last().Type == LexemeType.Var)
                                ((VarLexeme) root.ChildLexemes.Last()).Index = i;
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException?.GetType() == typeof(ParseException))
                            {
                                error = ((ParseException)e.InnerException).ToError();
                            }
                            else
                            {
                                throw e;
                            }
                            return;
                        }
                    }
                    else
                    {
                        error = new CompileError(CompileErrorType.UnknownLexeme, lexeme?.Tokens[0]);
                        return;
                    }
                }
            }
        }

        private static void LinkBlocksToLexemes(BlockLexeme root, out CompileError error)
        {
            var toDelete = new List<int>();
            int index = 0;
            error = null;

            foreach (var lexeme in root.ChildLexemes)
            {
                if (lexeme.Type == LexemeType.Block)
                {
                    var bl = (BlockLexeme)lexeme;
                    var prev = index > 0 ? root.ChildLexemes[index - 1] : null;
                    if (prev != null && prev.RequireBlock)
                    {
                        toDelete.Add(index);
                        ((ComplexLexeme)prev).Block = bl;
                    }
                    LinkBlocksToLexemes((BlockLexeme) lexeme, out error);
                    if (error != null)
                        return;
                }
                index++;
            }

            toDelete.Reverse();
            foreach (var ind in toDelete)
                root.ChildLexemes.RemoveAt(ind);

            foreach (var lexeme in root.ChildLexemes)
            {
                if (lexeme.RequireBlock && ((ComplexLexeme)lexeme).Block == null)
                    error = new CompileError(CompileErrorType.LexemeWithoutRequiredBlock, lexeme.Tokens?[0]);
            }
        }

        private static void PrintLexemeTree(Lexeme root, int level)
        {
            root.Print(level);
        }

        public static List<Lexeme> Lexemize(List<Token> tokens, out CompileError error)
        {
            error = null;

            var root = GetStructureLexeme(tokens, out error);
            if (error != null)
                return null;

            ClearEmptyLexemes(root);
            if (root.ChildLexemes.Count == 0)
            {
                error = new CompileError(CompileErrorType.EmptyFile);
                return null;
            }

            ResolveLexemeTypes(root, out error);
            if (error != null)
                return null;

            LinkBlocksToLexemes(root, out error);
            if (error != null)
                return null;

            PrintLexemeTree(root, 0);

            return root.ChildLexemes;
        }
    }
}