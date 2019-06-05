namespace Nevermind.Compiler.Semantics
{
    internal class Import
    {
        public string Name;
        public Module LinkedModule;

        public Import(string name)
        {
            Name = name;
        }
    }
}