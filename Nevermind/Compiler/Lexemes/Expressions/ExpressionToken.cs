using System.Collections.Generic;
using Nevermind.ByteCode;

namespace Nevermind.Compiler.Lexemes.Expressions
{
    internal class ExpressionToken
    {
        public readonly Token CodeToken;
        public readonly List<ExpressionToken> SubTokens;
        public ExpressionToken Parent;

        public Operator LOperator;
        public Operator ROperator;

        public Operator UnaryOperator;
        public Token UnaryFunction;

        public ExpressionToken(Token codeToken)
        {
            CodeToken = codeToken;
            SubTokens = new List<ExpressionToken>();
        }

        public override string ToString()
        {
            var s = CodeToken != null ? $"{LOperator}{CodeToken.ToSource()}{ROperator}" : $"({LOperator}<complex>{ROperator})";
            return UnaryFunction != null ? $"{UnaryFunction.StringValue}({s})" : s;
        }
    }
}