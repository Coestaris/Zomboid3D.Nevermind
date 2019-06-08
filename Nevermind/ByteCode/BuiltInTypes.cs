using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Types;

namespace Nevermind.ByteCode
{
    internal enum TypeID
    {
        Integer = 0,
        Float = 1,
        String = 2
    }

    internal class NamedType
    {
        public string Name;
        public Type Type;

        public NamedType(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }

    internal static class BuiltInTypes
    {
        private static List<NamedType> _builtinTypes = new List<NamedType>
        {
            new  NamedType("ulong",   new IntegerType(64, false)),
            new  NamedType("uint",    new IntegerType(32, false)),
            new  NamedType("ushort",  new IntegerType(16, false)),
            new  NamedType("byte",    new IntegerType(8,  false)),
            new  NamedType("long",    new IntegerType(64, true)),
            new  NamedType("integer", new IntegerType(32, true)),
            new  NamedType("short",   new IntegerType(16, true)),
            new  NamedType("char",    new IntegerType(8,  true)),

            new  NamedType("real",    new FloatType(32)),
            new  NamedType("double",  new FloatType(64)),

            new  NamedType("string",  new StringType(new IntegerType(8,  true))),
            new  NamedType("wstring", new StringType(new IntegerType(16, true))),
        };

        public static Type DefaultConstIntType => _builtinTypes[5].Type;
        public static Type DefaultConstFloatType => _builtinTypes[8].Type;
        public static Type DefaultConstStringType => _builtinTypes[10].Type;

        public static List<NamedType> Get()
        {
            return _builtinTypes;
        }
    }
}