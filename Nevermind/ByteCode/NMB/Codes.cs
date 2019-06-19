using System;
using System.Collections.Generic;
using Nevermind.ByteCode.Types;

namespace Nevermind.ByteCode.NMB
{
    internal static class Codes
    {
        public static readonly UInt16 CurrentNMVersion = 1000;

        public static byte[] NMPSignature = {(byte) 'N', (byte) 'M', (byte) 'B'};

        public static readonly Dictionary<TypeID, UInt16> TypeIdDict = new Dictionary<TypeID, ushort>
        {
            {TypeID.Integer, 0x1},
            {TypeID.Float, 0x2},
            {TypeID.String, 0x3},
        };
    }
}