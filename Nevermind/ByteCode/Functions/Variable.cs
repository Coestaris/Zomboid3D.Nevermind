using Nevermind.Compiler;

namespace Nevermind.ByteCode.Functions
{
    internal class Variable
    {
        public readonly int Scope;
        public readonly Type Type;
        public readonly string Name;

        public Token Token;

        public Variable(Type type, string name, int scope, Token token)
        {
            Type = type;
            Name = name;
            Scope = scope;
            Token = token;
        }
    }
}