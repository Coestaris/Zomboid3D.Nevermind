namespace Nevermind.ByteCode.Functions
{
    internal class FunctionParameter
    {
        public Type Type;
        public string Name;
        
        public FunctionParameter(Type type, string name)
        {
            Type = type;
            Name = name;
        }
        
    }
}