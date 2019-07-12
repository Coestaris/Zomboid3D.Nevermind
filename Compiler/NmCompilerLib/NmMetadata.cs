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
            ch.Data.Add((byte)CompilationDate.Second);
            ch.Data.Add((byte)CompilationDate.Minute);
            ch.Data.Add((byte)CompilationDate.Hour);
            ch.Data.Add((byte)CompilationDate.Day);
            ch.Data.Add((byte)CompilationDate.Month);
            ch.Data.AddRange(Chunk.Int16ToBytes(CompilationDate.Year));

            ch.Data.AddRange(Chunk.Int16ToBytes(BinaryName.Length));
            ch.Data.AddRange(BinaryName.Select(p => (byte)p));

            ch.Data.AddRange(Chunk.Int16ToBytes(BinaryDescription.Length));
            ch.Data.AddRange(BinaryDescription.Select(p => (byte)p));

            ch.Data.AddRange(Chunk.Int16ToBytes(BinaryAuthor.Length));
            ch.Data.AddRange(BinaryAuthor.Select(p => (byte)p));

            ch.Data.AddRange(Chunk.Int16ToBytes(MinorVersion));
            ch.Data.AddRange(Chunk.Int16ToBytes(MajorVersion));

            return ch;
        }
    }
}