using System.Collections.Generic;

namespace Nevermind.ByteCode.Functions
{
    internal class Function
    {
        public string Name;
        public FunctionModifier Modifier;

        public Type ReturnType;
        public List<FunctionParameter> Parameters;

        public Function(string name, FunctionModifier modifier, Type returnType, List<FunctionParameter> parameters = null)
        {
            Name = name;
            Modifier = modifier;
            ReturnType = returnType;
            Parameters = parameters ?? new List<FunctionParameter>();
        }
    }
}