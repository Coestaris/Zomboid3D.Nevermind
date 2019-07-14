using System.Collections.Generic;

namespace Nevermind.ByteCode.Types.Scalar
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

        public override bool Compare(Type type)
        {
            return (type as StringType).CharType == CharType;
        }

        public override int GetBase()
        {
            return CharType.GetBase();
        }

        public override List<byte> Serialize(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}