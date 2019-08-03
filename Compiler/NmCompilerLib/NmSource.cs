using System.Collections.Generic;
using System.IO;
using System.Text;
using Nevermind.Compiler;

namespace Nevermind
{
    public class NmSource
    {
        private readonly string _source;

        internal List<string> ModuleNames;

        internal void ProceedModuleNames(List<string> includeDirectories)
        {
            ModuleNames = new List<string>();
            foreach (var directory in includeDirectories)
                ModuleNames.AddRange(Directory.GetFiles(directory, "*.nmm"));
        }

        private NmSource(string source, string fileName = null)
        {
            _source = source;
            FileName = fileName;
        }

        public static NmSource FromText(string source)
        {
            return new NmSource(source);
        }

        public static NmSource FromFile(string fileName)
        {
            return new NmSource(null, fileName);
        }

        public static NmSource FromFile(FileInfo fi)
        {
            return new NmSource(null, fi.FullName);
        }

        public string FileName { get; }

        internal string GetSource(out CompileError error)
        {
            error = null;
            if (_source != null)
                return _source;

            if (FileName == null || !new FileInfo(FileName).Exists)
            {
                error = new CompileError(CompileErrorType.UnableToOpenFile, new Token(FileName));
                return null;
            }

            return File.ReadAllText(FileName, Encoding.UTF8);
        }
    }
}