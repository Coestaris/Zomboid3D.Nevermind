namespace Nevermind.Compiler.Semantics
{
    internal class Import
    {
        public bool Library;
        public string Name;
        public Module LinkedModule;

        public Import(string name)
        {
            Name = name;
        }
    }
}