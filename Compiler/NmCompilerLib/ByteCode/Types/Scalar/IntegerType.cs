using System;
using System.Collections.Generic;

namespace Nevermind.ByteCode.Types.Scalar
{
    internal class IntegerType : Type
    {
        public int TypeBase;
        public bool Signed;
        public override bool HasLength => false;

        public IntegerType(int typeBase, bool signed)
        {
            ID = signed ? TypeID.Integer : TypeID.UInteger;
            Signed = signed;
            TypeBase = typeBase;
        }

        public override bool Compare(Type type)
        {
            var t = type as IntegerType;
            return t.TypeBase == TypeBase && t.Signed == Signed;
        }

        public long TrimValue(long l)
        {
            return l & ((2L << (TypeBase - 1 - (Signed ? 1 : 0))) - 1);
        }

        public override int GetBase()
        {
            return TypeBase;
            //return Signed ? TypeBase : -TypeBase;
        }

        public long Max()
        {
            if (Signed)
                return (2L << (TypeBase - 2)) - 1;
            else
                return (2L << (TypeBase - 1)) - 1;
        }

        public long Min()
        {
            if (Signed)
                return -(2L << (TypeBase - 1)) - 1;
            else
                return 0;
        }

        public override List<byte> Serialize(object value)
        {
            if(!(value is long))
                throw new ArgumentException("Expected long type", nameof(value));

            var trimmed = TrimValue((long)value);
            var list = new List<byte>();

            var byteCount = TypeBase / 8;
            for (var i = 0; i < byteCount; i++)
                list.Add((byte) ((trimmed >> (i * 8)) & 0xFF));

            return list;
        }
    }
}