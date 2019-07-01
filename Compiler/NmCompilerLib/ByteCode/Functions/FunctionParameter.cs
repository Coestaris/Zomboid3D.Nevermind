using Nevermind.ByteCode.Types;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Functions
{
    internal class FunctionParameter
    {
        public Type Type;
        public string Name;
        public Token CodeToken;

        public FunctionParameter(Type type, string name, Token token)
        {
            Type = type;
            Name = name;
            CodeToken = token;
        }
    }
}