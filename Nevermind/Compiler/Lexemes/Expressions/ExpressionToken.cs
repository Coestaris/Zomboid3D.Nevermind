using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Operator> UnaryOperators;
        public Token UnaryFunction;

        public ExpressionToken(Token codeToken)
        {
            CodeToken = codeToken;
            SubTokens = new List<ExpressionToken>();
            UnaryOperators = new List<Operator>();
        }

        public override string ToString()
        {
            var s = CodeToken != null ? $"{LOperator}{CodeToken.ToSource()}{ROperator}" : $"({LOperator}<complex>{ROperator})";
            if(UnaryFunction != null) return $"{UnaryFunction.StringValue}({s})";
            if(UnaryOperators.Count != 0) return $"{string.Join("", UnaryOperators)}({s})";
            return s;
        }
    }
}