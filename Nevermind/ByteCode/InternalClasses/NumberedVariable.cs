using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.InternalClasses
{
    internal class NumeratedVariable
    {
        public readonly int Index;
        public readonly Variable Variable;

        public NumeratedVariable(int index, Variable variable)
        {
            Index = index;
            Variable = variable;
        }
    }
}