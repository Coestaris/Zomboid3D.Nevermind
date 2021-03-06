using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.InternalClasses;
using Nevermind.ByteCode.Types.Scalar;

namespace Nevermind.ByteCode.Types
{
    internal static class BuiltInTypes
    {
        private static List<NamedType> _builtinTypes = new List<NamedType>
        {
            new  NamedType("byte",    new IntegerType(8,  false)),
            new  NamedType("char",    new IntegerType(8,  true)),
            new  NamedType("short",   new IntegerType(16, true)),
            new  NamedType("ushort",  new IntegerType(16, false)),
            new  NamedType("integer", new IntegerType(32, true)),
            new  NamedType("uint",    new IntegerType(32, false)),
            new  NamedType("long",    new IntegerType(64, true)),
            new  NamedType("ulong",   new IntegerType(64, false)),

            new  NamedType("real",    new FloatType(32)),
            new  NamedType("double",  new FloatType(64)),

            new  NamedType("string",  new StringType(new IntegerType(8,  true))),
            new  NamedType("wstring", new StringType(new IntegerType(16, true))),
        };

        private static List<IntegerType> _intTypes = _builtinTypes.FindAll(p => p.Type.ID == TypeID.Integer || p.Type.ID == TypeID.UInteger).Select(p => (IntegerType)p.Type).ToList();
        private static List<FloatType> _floatTypes = _builtinTypes.FindAll(p => p.Type.ID == TypeID.Float).Select(p => (FloatType)p.Type).ToList();

        public static Type DefaultConstInt8Type   => _builtinTypes[0].Type;
        public static Type DefaultConstUInt8Type  => _builtinTypes[1].Type;
        public static Type DefaultConstInt16Type  => _builtinTypes[2].Type;
        public static Type DefaultConstUInt16Type => _builtinTypes[3].Type;
        public static Type DefaultConstInt32Type  => _builtinTypes[4].Type;
        public static Type DefaultConstUInt32Type => _builtinTypes[5].Type;
        public static Type DefaultConstInt64Type  => _builtinTypes[6].Type;
        public static Type DefaultConstUInt64Type => _builtinTypes[7].Type;

        public static Type DefaultConstFloat32Type => _builtinTypes[8].Type;
        public static Type DefaultConstFloat64Type => _builtinTypes[9].Type;

        public static Type DefaultConstStringType => _builtinTypes[10].Type;

        public static FloatType GetFloatType(int typeBase)
        {
            return _floatTypes.Find(p => p.TypeBase == typeBase);
        }

        public static IntegerType GetIntegerType(int typeBase, bool signed)
        {
            return _intTypes.Find(p => p.TypeBase == typeBase && p.Signed == signed);
        }

        public static List<NamedType> Get()
        {
            return _builtinTypes;
        }
    }
}