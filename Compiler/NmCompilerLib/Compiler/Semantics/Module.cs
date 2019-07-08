using Nevermind.ByteCode.Functions;

namespace Nevermind.Compiler.Semantics
{
    internal class Module
    {
        public string Name;
        public Function InitializationFunc;
        public Function FinalizationFunc;
        public bool IsLibrary;

        public NmProgram Program;

        public static Module RequestModule(string name)
        {
            return null;
        }

        public bool HasItem(string name, ModuleItem item)
        {
            return false;
        }

        public Module(string name, NmProgram program)
        {
            Name = name;
            Program = program;
        }
    }
}