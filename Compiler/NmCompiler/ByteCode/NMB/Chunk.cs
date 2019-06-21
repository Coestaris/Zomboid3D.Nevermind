using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.NMB
{
    internal class Chunk
    {
        public Chunk(ChunkType type)
        {
            Data = new List<byte>();
            Type = type;
        }

        public List<byte> Data;
        public readonly ChunkType Type;

        public byte[] ToBytes()
        {
            var destOffset = 0;
            var data = Data.ToArray();
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

        private static readonly Dictionary<ChunkType, string> Types = new Dictionary<ChunkType, string>
        {
            {ChunkType.HEAD, "HE"},
            {ChunkType.METADATA, "ME"},
            {ChunkType.TYPE, "TY"},
            {ChunkType.CONST, "CO"},
            {ChunkType.FUNC, "FU"}
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
                (byte) ((a >> 24) & 0xFF),
                (byte) ((a >> 16) & 0xFF),
                (byte) ((a >> 8) & 0xFF),
                (byte) ((a >> 0) & 0xFF),
            };
        }

        public static byte[] UInt16ToBytes(UInt16 a)
        {
            return new[]
            {
                (byte) ((a >> 8) & 0xFF),
                (byte) ((a >> 0) & 0xFF),
            };
        }
    }
}