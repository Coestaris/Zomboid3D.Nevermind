using System.Collections.Generic;

namespace Nevermind.ByteCode.Types
{
    internal class ArrayType : Type
    {
        public Type BaseType;
        public int Dimensions;

        public ArrayType(Type baseType, int dimensions)
        {
            BaseType = baseType;
            Dimensions = dimensions;

            ID = TypeID.Array;
        }

        public override List<byte> Serialize(object value)
        {
            throw new System.NotImplementedException();
        }

        public override bool Compare(Type type)
        {
            var arrayType = (type as ArrayType);

            return arrayType.BaseType == BaseType &&
                   arrayType.Dimensions == Dimensions;
        }

        public override int GetBase() => -1;
        public override bool HasLength => true;
    }
}