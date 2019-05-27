using System;
using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.Lexemes
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
            new LexemeInfo(LexemeType.Import, new List<TokenType>
            {
                TokenType.ImportKeyword,
                TokenType.Identifier

            }, typeof(ImportLexeme)),

            new LexemeInfo(LexemeType.Var, new List<TokenType>
            {
                TokenType.VarKeyword, TokenType.Identifier, TokenType.Colon, TokenType.Identifier, TokenType.EqualSign, Token.MathExpressionTokenType

            }, typeof(VarLexeme)),

            new LexemeInfo(LexemeType.If, new List<TokenType>
            {
                TokenType.IfKeyword, TokenType.BracketOpen, Token.MathExpressionTokenType, TokenType.BracketClosed,

            }, typeof(IfLexeme)),

            new LexemeInfo(LexemeType.FunctionCall, new List<TokenType>
            {
                TokenType.Identifier, TokenType.BracketOpen, Token.AnyTokenType, TokenType.BracketClosed,

            }, typeof(FunctionCallLexeme)),

            new LexemeInfo(LexemeType.Function, new List<TokenType>
            {
                TokenType.FunctionKeyword, TokenType.Identifier, TokenType.BracketOpen, Token.AnyTokenType, TokenType.BracketClosed,

            }, typeof(FunctionLexeme)),

            new LexemeInfo(LexemeType.Expression, new List<TokenType>
            {
                Token.AnyTokenType,

            }, typeof(ExpressionLexeme)),
        };
    }
}