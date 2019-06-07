using Nevermind.Compiler;

namespace Nevermind.ByteCode
{
    internal class Type
    {
        public static CompileError GetType(NmProgram program, Token name, out Type type)
        {
            type = program.AvailableTypes.Find(p => p.Name == name.StringValue)?.Type;
            return type == null ? new CompileError(CompileErrorType.UnknownTypeName, name) : null;
        }
    }
}