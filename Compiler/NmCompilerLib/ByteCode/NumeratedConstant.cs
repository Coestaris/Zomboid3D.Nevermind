﻿using Nevermind.Compiler.Formats.Constants;

namespace Nevermind.ByteCode
{
    internal class NumeratedConstant
    {
        public int Index;
        public Constant Constant;

        public NumeratedConstant(int index, Constant constant)
        {
            Index = index;
            Constant = constant;
        }
    }
}