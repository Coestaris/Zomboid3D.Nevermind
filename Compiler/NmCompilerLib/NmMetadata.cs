using System;
using System.Linq;
using Nevermind.ByteCode.NMB;

namespace Nevermind
{
    public class NmMetadata
    {
        public readonly string BinaryName;
        public readonly string BinaryDescription;
        public readonly string BinaryAuthor;
        public readonly DateTime CompilationDate;

        public readonly UInt16 MinorVersion;
        public readonly UInt16 MajorVersion;

        public NmMetadata(string binaryName = "binary", string binaryDescription = "", string binaryAuthor = null,
            DateTime compilationDate = default(DateTime), ushort minorVersion = 1, ushort majorVersion = 1)
        {
            BinaryName = binaryName;
            BinaryDescription = binaryDescription;
            BinaryAuthor = binaryAuthor ?? Environment.UserName;
            CompilationDate = compilationDate == default(DateTime) ? DateTime.Now : compilationDate;
            MinorVersion = minorVersion;
            MajorVersion = majorVersion;
        }

        internal Chunk GetChunk()
        {
            var ch = new Chunk(ChunkType.METADATA);
            ch.Add((byte)CompilationDate.Second);
            ch.Add((byte)CompilationDate.Minute);
            ch.Add((byte)CompilationDate.Hour);
            ch.Add((byte)CompilationDate.Day);
            ch.Add((byte)CompilationDate.Month);
            ch.Add((UInt16)CompilationDate.Year);

            ch.Add((UInt16)BinaryName.Length);
            ch.Add(BinaryName.Select(p => (byte)p));

            ch.Add((UInt16)BinaryDescription.Length);
            ch.Add(BinaryDescription.Select(p => (byte)p));

            ch.Add((UInt16)BinaryAuthor.Length);
            ch.Add(BinaryAuthor.Select(p => (byte)p));

            ch.Add((UInt16)MinorVersion);
            ch.Add((UInt16)MajorVersion);

            return ch;
        }
    }
}