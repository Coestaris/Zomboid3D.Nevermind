namespace Nevermind.ByteCode.Types
{
    internal class IntegerType : Type
    {
        public int TypeBase;
        public bool Signed;

        public IntegerType(int typeBase, bool signed)
        {
            Signed = signed;
            TypeBase = typeBase;
        }
    }
}