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

        public long TrimValue(long l)
        {
            return l & ((2L << (TypeBase - 1 - (Signed ? 1 : 0))) - 1);
        }

        public override int GetBase()
        {
            return Signed ? TypeBase : -TypeBase;
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