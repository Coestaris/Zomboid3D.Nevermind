using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.Semantics
{
    internal class Attribute
    {
        public Token Name;
        public List<Token> Parameters;

        public Attribute(List<Token> tokens)
        {
            Name = tokens[0];

            if (tokens.Count == 0) return;
            Parameters = new List<Token>();

            var iterator = new TokenIterator<Token>(tokens.Skip(3).Take(tokens.Count - 4));
            while (iterator.GetNext() != null)
            {
                if(iterator.Current.Type != TokenType.ComaSign)
                    Parameters.Add(iterator.Current);
            }
        }
    }
}