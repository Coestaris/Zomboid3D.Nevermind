namespace Nevermind.ByteCode.Types
{
    internal class StringType : Type
    {
        public IntegerType CharType;
        public override bool HasLength => true;

        public StringType(IntegerType charType)
        {
            ID = TypeID.String;
            CharType = charType;
        }

        public override int GetBase()
        {
            return CharType.GetBase();
        }
    }
}