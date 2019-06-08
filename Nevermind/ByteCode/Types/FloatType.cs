namespace Nevermind.ByteCode.Types
{
    internal class FloatType : Type
    {
        public int TypeBase;
        public override bool HasLength => false;

        public FloatType(int typeBase)
        {
            ID = TypeID.Float;
            TypeBase = typeBase;
        }

        public override int GetBase()
        {
            return TypeBase;
        }
    }
}