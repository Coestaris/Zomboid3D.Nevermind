using System;
using System.Collections.Generic;
using System.Linq;

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
                    new LexemePatternToken(TokenType.ImportKeyword | TokenType.LibraryKeyword),
                    new LexemePatternToken(TokenType.Identifier   )
                }),

            new LexemeInfo(
                LexemeType.Module,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.ModuleKeyword),
                    new LexemePatternToken(TokenType.Identifier   )
                }),

            //Variable with definition
            new LexemeInfo(
                LexemeType.Var,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.VarKeyword         ),
                    new LexemePatternToken(TokenType.Identifier         ),
                    new LexemePatternToken(TokenType.Colon              ),
                    new LexemePatternToken(TokenType.Identifier         ),
                    new LexemePatternToken(
                        TokenType.SquareBracketOpen | TokenType.SquareBracketClosed | TokenType.ComplexToken, false),
                    new LexemePatternToken(TokenType.EqualSign          , false),
                    new LexemePatternToken(Token.MathExpressionTokenType, false),
                }),

            new LexemeInfo(
                LexemeType.If,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.IfKeyword                      ),
                    new LexemePatternToken(TokenType.BracketOpen                    ),
                    new LexemePatternToken(Token.MathExpressionTokenType, true, true),
                    new LexemePatternToken(TokenType.BracketClosed                  )
                }),

            new LexemeInfo(
                LexemeType.Else,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.ElseKeyword),
                }),

            new LexemeInfo(
                LexemeType.Function,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(
                        TokenType.PublicKeyword     | TokenType.PrivateKeyword      | 
                        TokenType.EntrypointKeyword | TokenType.FinalizationKeyword | 
                        TokenType.InitializationKeyword,              false),
                    new LexemePatternToken(TokenType.FunctionKeyword       ),
                    new LexemePatternToken(TokenType.Identifier  ,false),
                    new LexemePatternToken(
                        TokenType.SquareBracketOpen | TokenType.SquareBracketClosed | TokenType.ComplexToken, false),
                    new LexemePatternToken(TokenType.Identifier),
                    new LexemePatternToken(TokenType.BracketOpen           ),
                    new LexemePatternToken(Token.AnyTokenType              ),
                    new LexemePatternToken(TokenType.BracketClosed         )
                }),

           new LexemeInfo(
                LexemeType.Return,
                new List<LexemePatternToken>
                {
                    new LexemePatternToken(TokenType.ReturnKeyword      ),
                    new LexemePatternToken(Token.MathExpressionTokenType)
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