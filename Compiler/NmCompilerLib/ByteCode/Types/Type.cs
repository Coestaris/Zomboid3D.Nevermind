using System;
using System.Collections.Generic;
using System.IO;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Types
{
    internal abstract class Type
    {
        public TypeID ID;

        public static CompileError GetType(NmProgram program, Token name, out Type type)
        {
            type = program.AvailableTypes.Find(p => p.Name == name.StringValue)?.Type;

            if (type != null) program.UsedTypes.Add(type);

            return type == null ? new CompileError(CompileErrorType.UnknownTypeName, name) : null;
        }

        public virtual int GetBase() { return -1; }

        public abstract List<byte> Serialize(object value);

        public override bool Equals(object obj)
        {
            if (!(obj is Type)) return false;

            var type = (Type)obj;
            return ID == type.ID && GetBase() == type.GetBase();
        }

        public override int GetHashCode()
        {
            return (int)ID * 100 + GetBase();
        }

        public abstract bool HasLength { get; }

        public static bool operator ==(Type a, Type b)
        {
            var aNull = (object) a == null;
            var bNull = (object) b == null;

            if (aNull && bNull)
                return true;

            if (!aNull && bNull || aNull && !bNull)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Type a, Type b)
        {
            return !(a == b);
        }

        public static Type CastTypes(Type a, Type b, Type preferedType = null)
        {
            //Types the same
            if (a.ID == b.ID && a.GetBase() == b.GetBase())
                return a;

            //Types the same, but different base
            if (a.ID == b.ID && a.GetBase() != b.GetBase())
                //Cast to bigger type
                return a.GetBase() > b.GetBase() ? a : b;

            //if we prefer something
            if (preferedType != null)
            {
                var type = CastTypes(a, b, null);

                if (type == null) return null;
                return CastTypes(type, preferedType, null);
            }
            else
            {
                //We cant cast string to anything else
                if (a.ID == TypeID.String || b.ID == TypeID.String)
                    return null;

                //Cast integers to float
                if (a.ID == TypeID.Float && b.ID != TypeID.Float)
                    return a;
                if (b.ID == TypeID.Float && a.ID != TypeID.Float)
                    return b;

                //choose best option
                var resultSigned = a.ID == TypeID.UInteger || b.ID == TypeID.UInteger;
                var resultBase = Math.Max(a.GetBase(), b.GetBase());

                return GetNumericType(resultSigned, resultBase);
            }
        }

        public static Type GetNumericType(bool signed, int typeBase)
        {
            if(signed)
                switch (typeBase)
                {
                    case 8: return BuiltInTypes.DefaultConstInt8Type;
                    case 16: return BuiltInTypes.DefaultConstInt16Type;
                    case 32: return BuiltInTypes.DefaultConstInt32Type;
                    case 64: return BuiltInTypes.DefaultConstInt64Type;
                }
            else
                switch (typeBase)
                {
                    case 8: return BuiltInTypes.DefaultConstUInt8Type;
                    case 16: return BuiltInTypes.DefaultConstUInt16Type;
                    case 32: return BuiltInTypes.DefaultConstUInt32Type;
                    case 64: return BuiltInTypes.DefaultConstUInt64Type;
                }

            return null;
        }
    }
}