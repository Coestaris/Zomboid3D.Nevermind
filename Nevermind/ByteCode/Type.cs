using Nevermind.Compiler;

namespace Nevermind.ByteCode
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
        public abstract bool HasLength { get; }
    }
}