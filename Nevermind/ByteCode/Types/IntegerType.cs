namespace Nevermind.ByteCode.Types
{
    internal class IntegerType : Type
    {
        public int TypeBase;
        public bool Signed;
        public override bool HasLength => false;

        public IntegerType(int typeBase, bool signed)
        {
            ID = TypeID.Integer;
            Signed = signed;
            TypeBase = typeBase;
        }

        public override int GetBase()
        {
            return Signed ? TypeBase : -TypeBase;
        }
    }
}