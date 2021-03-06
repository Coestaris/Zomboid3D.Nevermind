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

        private static void ResolveLexemeTypes(BlockLexeme root, out CompileError error, bool prototypesOnly)
        {
            error = null;
            for(var i = 0; i < root.ChildLexemes.Count; i++)
            {
                var lexeme = root.ChildLexemes[i];

                if (lexeme.Type == LexemeType.Block)
                {
                    ResolveLexemeTypes((BlockLexeme)lexeme, out error, prototypesOnly);
                    if(error != null) return;
                }
                else if(lexeme.Type == LexemeType.Unknown)
                {
                    LexemeInfo matchedLexeme = LexemePatternToken.GetMatchedLexeme(lexeme.Tokens);
                    if (matchedLexeme != null)
                    {
                        try
                        {
                            root.ChildLexemes[i] = matchedLexeme.CreateLexeme(lexeme.Tokens, prototypesOnly);
                            //if (root.ChildLexemes.Last().Type == LexemeType.Var)
                            //((VarLexeme) root.ChildLexemes.Last()).Index = i;

                            if(!prototypesOnly)
                            {
                                if (root.ChildLexemes[i].Type == LexemeType.Else)
                                {
                                    if (i == 0 ||
                                        (root.ChildLexemes[i - 1].Type != LexemeType.If) &&
                                        (root.ChildLexemes[i - 1].Type == LexemeType.Block &&
                                         root.ChildLexemes[i - 2].Type != LexemeType.If))
                                    {
                                        error = new CompileError(CompileErrorType.ElseWithoutCondition,
                                            lexeme.Tokens[0]);
                                        return;
                                    }

                                    if (root.ChildLexemes[i - 1].Type == LexemeType.If)
                                    {
                                        (root.ChildLexemes[i - 1] as IfLexeme).ElseLexeme =
                                            (ElseLexeme) root.ChildLexemes[i];
                                    }
                                    else
                                    {
                                        (root.ChildLexemes[i - 2] as IfLexeme).ElseLexeme =
                                            (ElseLexeme) root.ChildLexemes[i];
                                    }
                                }
                            }
                        }
                        catch(CompileException e)
                        {
                            error = e.ToError();
                            return;
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException?.GetType() == typeof(CompileException))
                            {
                                error = ((CompileException)e.InnerException).ToError();
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

        private static void LinkBlocksToLexemes(BlockLexeme root, out CompileError error, bool prototypesOnly)
        {
            var toDelete = new List<int>();
            int index = 0;
            error = null;

            foreach (var lexeme in root.ChildLexemes)
            {
                if(lexeme == null)
                    toDelete.Add(index);
                else
                {
                    if (lexeme.Type == LexemeType.Block)
                    {
                        var bl = (BlockLexeme) lexeme;
                        var prev = index > 0 ? root.ChildLexemes[index - 1] : null;
                        if (prev != null && prev.RequireBlock)
                        {
                            toDelete.Add(index);
                            ((ComplexLexeme) prev).Block = bl;
                        }

                        LinkBlocksToLexemes((BlockLexeme) lexeme, out error, prototypesOnly);
                        if (error != null)
                            return;
                    }
                }

                index++;
            }

            toDelete.Reverse();
            foreach (var ind in toDelete)
                root.ChildLexemes.RemoveAt(ind);

            foreach (var lexeme in root.ChildLexemes)
            {
                if (!prototypesOnly && lexeme.RequireBlock && ((ComplexLexeme)lexeme).Block == null)
                    error = new CompileError(CompileErrorType.LexemeWithoutRequiredBlock, lexeme.Tokens?[0]);
            }
        }

        private static void PrintLexemeTree(Lexeme root, int level)
        {
            root.Print(level);
        }

        public static List<Lexeme> Lexemize(List<Token> tokens, out CompileError error, bool verbose, bool prototypesOnly)
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

            ResolveLexemeTypes(root, out error, prototypesOnly);
            if (error != null)
                return null;

            LinkBlocksToLexemes(root, out error, prototypesOnly);
            if (error != null)
                return null;

            if(verbose)
                PrintLexemeTree(root, 0);

            return root.ChildLexemes;
        }
    }
}