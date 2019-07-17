using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Types
{
    internal abstract class Type
    {
        public TypeID ID;

        public virtual int GetBase() { return -1; }

        public abstract List<byte> Serialize(object value);

        public abstract bool Compare(Type type);

        public abstract bool HasLength { get; }

        public static CompileError GetType(NmProgram program, List<Token> tokens, out Type type)
        {
            type = null;

            if (tokens[tokens.Count - 1].Type == TokenType.SquareBracketClosed &&
                tokens[tokens.Count - 2].Type == TokenType.SquareBracketOpen)
            {
                Type innerType;
                CompileError error;
                if ((error = GetType(program, tokens.Take(tokens.Count - 2).ToList(), out innerType)) != null)
                    return error;

                type = new ArrayType(innerType);

                program.UsedTypes.Add(type);
                return null;
            }
            else
            {
                type = BuiltInTypes.Get().Find(p => p.Name == tokens[0].StringValue)?.Type;

                if (type != null) program.UsedTypes.Add(type);

                return type == null ? new CompileError(CompileErrorType.UnknownTypeName, tokens[0]) : null;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Type)) return false;

            var type = (Type)obj;
            return ID == type.ID && Compare(type);
        }

        public override int GetHashCode()
        {
            return (int)ID * 100 + GetBase();
        }

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

        public static bool CanCastAssignment(Type dest, Type src)
        {
            if (dest.ID == TypeID.String || src.ID == TypeID.String)
                return false;

            if (dest.ID == TypeID.Array && src.ID == TypeID.Array)
                return dest == src;

            return dest.GetBase() >= src.GetBase() && (dest.ID != TypeID.Integer && dest.ID != TypeID.UInteger || src.ID != TypeID.Float);
        }

        public static Type CastTypes(Type a, Type b, Type preferredType = null)
        {
            if (a.ID == TypeID.Array || b.ID == TypeID.Array)
                return null;

            //Types the same
            if (a.ID == b.ID && a.GetBase() == b.GetBase())
                return a;

            //Types the same, but different base
            if (a.ID == b.ID && a.GetBase() != b.GetBase())
                //Cast to bigger type
                return a.GetBase() > b.GetBase() ? a : b;

            //if we prefer something
            if (preferredType != null)
            {
                var type = CastTypes(a, b, null);

                if (type == null) return null;
                return CastTypes(type, preferredType, null);
            }
            else
            {
                //We cant cast string and array to anything else
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

        private static Type GetNumericType(bool signed, int typeBase)
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