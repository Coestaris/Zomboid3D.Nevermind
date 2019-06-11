using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class VarLexeme : Lexeme
    {
        public Token VarName;
        public Token TypeName;
        public ExpressionLexeme Expression;

        public int Index;

        public VarLexeme(List<Token> tokens) : base(tokens, LexemeType.Var, false)
        {
            VarName = tokens[1];
            TypeName = tokens[3];

            Expression = new ExpressionLexeme(tokens.Skip(5).Take(tokens.Count - 5).ToList());
        }

        public override void Print(int level)
        {
            base.Print(level);
            Expression?.PrintExpression(level + 1);
        }
    }
}