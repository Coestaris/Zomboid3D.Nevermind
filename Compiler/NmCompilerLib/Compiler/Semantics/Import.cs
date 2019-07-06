using System.IO;

namespace Nevermind.Compiler.Semantics
{
    internal class Import
    {
        public bool Library;

        public string Name;
        public string FileName;

        public Module LinkedModule;

        public static CompileError CreateImport(string name, string fileName, out Import import)
        {
            var compilerBinaryName = new FileInfo(fileName).Directory.FullName + Path.DirectorySeparatorChar +
                                     new FileInfo(fileName).Name + Path.DirectorySeparatorChar + ".nmb";

            if (File.Exists(compilerBinaryName))
            {

            }

            import = null;
            return null;
        }
    }
}