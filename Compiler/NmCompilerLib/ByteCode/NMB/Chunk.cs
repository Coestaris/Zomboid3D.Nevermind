using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.NMB
{
    internal class Chunk
    {
        private List<byte> _data;
        public readonly ChunkType Type;

        public Chunk(ChunkType type)
        {
            _data = new List<byte>();
            Type = type;
        }

        public byte[] Serialize()
        {
            var destOffset = 0;
            var data = _data.ToArray();
            var buffer = new byte[10 + data.Length];
            Buffer.BlockCopy(Int32ToBytes(data.Length), 0, buffer, destOffset, 4);
            destOffset += 4;

            Buffer.BlockCopy(UInt32ToBytes(CRC32.ComputeChecksum(data)), 0, buffer, destOffset, 4);
            destOffset += 4;

            Buffer.BlockCopy(UInt16ToBytes(TypeToInt()), 0, buffer, destOffset, 2);
            destOffset += 2;

            Buffer.BlockCopy(data, 0, buffer, destOffset, data.Length);
            return buffer;
        }

        public void Add(IEnumerable<byte> enumerable)
        {
            _data.AddRange(enumerable);
        }

        public void Add(byte b)
        {
            _data.Add(b);
        }

        public void Add(Int32 i)
        {
            _data.AddRange(Int32ToBytes(i));
        }

        public void Add(UInt32 i)
        {
            _data.AddRange(UInt32ToBytes(i));
        }

        public void Add(Int16 i)
        {
            _data.AddRange(Int16ToBytes(i));
        }

        public void Add(UInt16 i)
        {
            _data.AddRange(UInt16ToBytes(i));
        }

        private static readonly Dictionary<ChunkType, string> Types = new Dictionary<ChunkType, string>
        {
            {ChunkType.HEAD,     "HE"},
            {ChunkType.METADATA, "ME"},
            {ChunkType.TYPE,     "TY"},
            {ChunkType.CONST,    "CO"},
            {ChunkType.FUNC,     "FU"},
            {ChunkType.DEBUG,    "DE"},
            {ChunkType.GLOBALS,  "GL"}
        };

        private UInt16 TypeToInt()
        {
            var strType = Types[Type];
            return (UInt16)((strType[1] << 8) | strType[0]);
        }

        public static byte[] Int32ToBytes(Int32 a) => UInt32ToBytes((UInt32) a);

        public static byte[] Int16ToBytes(Int32 a) => UInt16ToBytes((UInt16) a);

        public static byte[] UInt32ToBytes(UInt32 a)
        {
            return new[]
            {
                (byte) ((a >> 0) & 0xFF),
                (byte) ((a >> 8) & 0xFF),
                (byte) ((a >> 16) & 0xFF),
                (byte) ((a >> 24) & 0xFF),
            };
        }

        public static byte[] UInt16ToBytes(UInt16 a)
        {
            return new[]
            {
                (byte) ((a >> 0) & 0xFF),
                (byte) ((a >> 8) & 0xFF),
            };
        }
    }
}