using System.IO;
using System.Text;
using Nevermind.Compiler;

namespace Nevermind
{
    public class NmSource
    {
        private readonly string _source;
        private readonly string _fileName;

        private NmSource(string source, string fileName = null)
        {
            _source = source;
            _fileName = fileName;
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

        public string FileName => _fileName;

        internal string GetSource(out CompileError error)
        {
            error = null;
            if (_source != null)
                return _source;

            if (_fileName == null || !new FileInfo(_fileName).Exists)
            {
                error = new CompileError(CompileErrorType.UnableToOpenFile, new Token("", _fileName, -1, -1, null));
                return null;
            }

            return File.ReadAllText(_fileName, Encoding.UTF8);
        }
    }
}