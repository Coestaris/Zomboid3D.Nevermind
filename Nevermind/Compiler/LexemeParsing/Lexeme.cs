using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.Compiler.LexemeParsing.Lexemes;

namespace Nevermind.Compiler.LexemeParsing
{
    internal abstract class Lexeme
    {
        public LexemeType Type;
        public List<Token> Tokens;
        public List<Lexeme> ChildLexemes;

        public virtual void Print(int level)
        {
            Console.WriteLine("{0} - {1}", new string(' ', level * 3), ToString());
            foreach (var childLexeme in ChildLexemes)
            {
                childLexeme.Print(level + 1);
            }
        }

        protected Lexeme(List<Token> tokens, LexemeType type, bool requireBlock)
        {
            if(tokens != null)
                Tokens = tokens.Select(p => p).ToList();

            RequireBlock = requireBlock;
            Type = type;
            ChildLexemes = new List<Lexeme>();
        }

        public readonly bool RequireBlock;

        public virtual string GetStringInfo()
        {
            return Tokens != null ? string.Join(" ", Tokens.Select(p => p.ToSource())) : "";
        }

        public override string ToString()
        {
            return $"Lexeme[{nameof(Type)}: {Type}. {GetStringInfo()}]";
        }

        public static List<LexemeInfo> Lexemes = new List<LexemeInfo>
        {
            new LexemeInfo(
                LexemeType.Import,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.ImportKeyword, true),
                    new LexemePatternToken(TokenType.Identifier,    true)
                }),

            new LexemeInfo(
                LexemeType.Module,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.ModuleKeyword, true),
                    new LexemePatternToken(TokenType.Identifier,    true)
                }),

            new LexemeInfo(
                LexemeType.Var,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.VarKeyword,          true),
                    new LexemePatternToken(TokenType.Identifier,          true),
                    new LexemePatternToken(TokenType.Colon,               true),
                    new LexemePatternToken(TokenType.Identifier,          true),
                    new LexemePatternToken(TokenType.EqualSign,           true),
                    new LexemePatternToken(Token.MathExpressionTokenType, true),
                }),

            new LexemeInfo(
                LexemeType.If,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.IfKeyword,           true),
                    new LexemePatternToken(TokenType.BracketOpen,         true),
                    new LexemePatternToken(Token.MathExpressionTokenType, true),
                    new LexemePatternToken(TokenType.BracketClosed,       true)
                }),

            new LexemeInfo(
                LexemeType.Function,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(
                        TokenType.PublicKeyword | TokenType.PrivateKeyword | TokenType.EntrypointKeyword |
                        TokenType.FinalizationKeyword | TokenType.InitializationKeyword, false),
                    new LexemePatternToken(TokenType.FunctionKeyword, true),
                    new LexemePatternToken(TokenType.Identifier,      true),
                    new LexemePatternToken(TokenType.Identifier,      false),
                    new LexemePatternToken(TokenType.BracketOpen,     true),
                    new LexemePatternToken(Token.AnyTokenType,        true),
                    new LexemePatternToken(TokenType.BracketClosed,   true)
                }),

           new LexemeInfo(
                LexemeType.Return,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.ReturnKeyword,       true),
                    new LexemePatternToken(Token.MathExpressionTokenType, true)
                }),

            new LexemeInfo(
                LexemeType.Expression,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(Token.AnyTokenType, true),
                }),
        };
    }
}