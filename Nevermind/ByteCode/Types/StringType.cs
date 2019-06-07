namespace Nevermind.ByteCode.Types
{
    internal class StringType : Type
    {
        public IntegerType CharType;

        public StringType(IntegerType charType)
        {
            CharType = charType;
        }
    }
}