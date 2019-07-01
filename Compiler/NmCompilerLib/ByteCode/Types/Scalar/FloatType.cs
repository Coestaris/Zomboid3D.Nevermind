using System.Collections.Generic;

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
            throw new System.NotImplementedException();
        }
    }
}