using System;
using System.IO;
using System.Text;

namespace Nevermind
{
    public class NmSource
    {
        private string _source;

        public NmSource(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            _source = source;
        }

        public NmSource(FileInfo fi)
        {
            if (fi == null) throw new ArgumentNullException(nameof(fi));
            if (!fi.Exists) throw new FileNotFoundException(fi.FullName);
            _source = File.ReadAllText(fi.FullName, Encoding.UTF8);
        }
    }
}