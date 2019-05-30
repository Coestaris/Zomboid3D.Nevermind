using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Nevermind.Compiler.Lexemes;
using Nevermind.Compiler.Lexemes.Expressions;

namespace Nevermind.Compiler
{
    internal class Lexemizer
    {
        private static BlockLexeme GetStructureLexeme(List<Token> tokens, out CompileError error)
        {
            error = null;
            var iterator = new TokenIterator<Token>(tokens);
            var possibleLexeme = new List<Token>();
            var currentParent = new BlockLexeme(null);

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
                            currentParent = new BlockLexeme(oldParent);
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

                                if (c2 != lexemeInfo.TokenTypes.Count - 1 &&
                                    lexemeInfo.TokenTypes[c2 + 1] != lexemeInfo.TokenTypes[c2] &&
                                    !lexemeInfo.TokenTypes[c2].HasFlag(TokenType.ComplexToken))
                                {
                                    c2++;
                                }

                                c1++;
                            }
                            else
                            {
                                c2++;
                                if(c2 > c1) break;
                            }
                        }

                        if(c1 == lexeme.Tokens.Count && c2 == lexemeInfo.TokenTypes.Count - 1) matchedLexemes.Add(lexemeInfo);
                    }

                    if (matchedLexemes.Count != 0)
                    {
                        try
                        {
                            root.ChildLexemes[i] = (Lexeme)Activator.CreateInstance(matchedLexemes[0].LexemeType, lexeme.Tokens);
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException.GetType() == typeof(ParseExpressionException))
                            {
                                error = ((ParseExpressionException)e.InnerException).ToError();
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
                        switch (prev.Type)
                        {
                            case LexemeType.If:
                                ((IfLexeme)prev).Block = bl;
                                break;
                            case LexemeType.Function:
                                ((FunctionLexeme)prev).Block = bl;
                                break;
                        }
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
                if (!lexeme.RequireBlock) continue;
                switch (lexeme.Type)
                {
                    case LexemeType.If:
                        if (((IfLexeme) lexeme).Block == null)
                            error = new CompileError(CompileErrorType.LexemeWithoutRequiredBlock, lexeme.Tokens?[0]);
                        break;
                    case LexemeType.Function:
                        if (((FunctionLexeme) lexeme).Block == null)
                            error = new CompileError(CompileErrorType.LexemeWithoutRequiredBlock, lexeme.Tokens?[0]);
                        break;
                }
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