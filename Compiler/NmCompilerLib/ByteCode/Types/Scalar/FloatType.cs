using System;
using System.Collections.Generic;
using System.Linq;

namespace Nevermind.ByteCode.Types.Scalar
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

        public override List<byte> Serialize(object value)
        {
            var v = (double)value;
            if (TypeBase == 32)
                return BitConverter.GetBytes((float)v).ToList();
            else
                return BitConverter.GetBytes(v).ToList();
        }
    }
}