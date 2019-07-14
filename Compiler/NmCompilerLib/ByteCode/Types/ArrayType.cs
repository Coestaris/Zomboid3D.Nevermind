using System.Collections.Generic;

namespace Nevermind.ByteCode.Types
{
    internal class ArrayType : Type
    {
        public Type ElementType;

        public ArrayType(Type elementType)
        {
            ElementType = elementType;
            ID = TypeID.Array;
        }

        public override List<byte> Serialize(object value)
        {
            throw new System.NotImplementedException();
        }

        public override bool Compare(Type type)
        {
            return (type as ArrayType).ElementType == ElementType;
        }

        public override int GetBase() => ElementType.GetBase();
        public override bool HasLength => true;
    }
}