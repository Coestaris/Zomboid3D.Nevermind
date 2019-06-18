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
    }
}