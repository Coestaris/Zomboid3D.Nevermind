using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class VarLexeme : Lexeme
    {
        public Token VarName;
        public List<Token> TypeTokens;
        public ExpressionLexeme Expression;

        public bool DeclarationOnly;
        public int Index;

        public VarLexeme(List<Token> tokens) : base(tokens, LexemeType.Var, false)
        {
            VarName = tokens[1];
            var index = tokens.FindIndex(p => p.Type == TokenType.EqualSign);

            if (index != -1)
            {
                TypeTokens = tokens.Skip(3).Take(index - 3).ToList();
                Expression = new ExpressionLexeme(tokens.Skip(index + 1).ToList());
            }
            else
            {
                TypeTokens = tokens.Skip(3).ToList();
                DeclarationOnly = true;
            }
        }

        public override void Print(int level)
        {
            base.Print(level);
            Expression?.PrintExpression(level + 1);
        }
    }
}