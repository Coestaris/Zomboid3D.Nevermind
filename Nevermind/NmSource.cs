using System;
using System.IO;
using System.Text;

namespace Nevermind
{
    public class NmSource
    {
        internal readonly string Source;
        internal readonly string FileName = null;

        public NmSource(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            Source = source;
        }

        public NmSource(FileInfo fi)
        {
            if (fi == null) throw new ArgumentNullException(nameof(fi));
            if (!fi.Exists) throw new FileNotFoundException(fi.FullName);
            FileName = fi.FullName;
            Source = File.ReadAllText(fi.FullName, Encoding.UTF8);
        }
    }
}