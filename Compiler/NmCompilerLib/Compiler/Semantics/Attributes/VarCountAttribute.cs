using System;
using System.Collections.Generic;

namespace Nevermind.Compiler.Semantics.Attributes
{
    internal enum VarCountMode
    {
        Greater,
        GreaterOrEq,
        Less,
        LessOrEq,
        Equals,

        Any
    }

    internal class VarCountAttribute : Attribute
    {
        private Dictionary<string, VarCountMode> modes = new Dictionary<string, VarCountMode>
        {
            { "gr",   VarCountMode.Greater },
            { "grEq", VarCountMode.GreaterOrEq },
            { "ls",   VarCountMode.Less },
            { "lsEq", VarCountMode.LessOrEq },
            { "eq",   VarCountMode.Equals }
        };

        public VarCountMode CountMode;
        public int Count;

        protected override CompileError VerifyParameters()
        {
            if(!modes.TryGetValue(Parameters[0].StringValue, out CountMode))
                return new CompileError(CompileErrorType.UnknownCountMode, Parameters[0]);

            if(!int.TryParse(Parameters[1].StringValue, out Count))
                return new CompileError(CompileErrorType.WrongCountValue, Parameters[1]);

            return null;
        }

        public VarCountAttribute(List<Token> parameters) : base(AttributeType.VarCount, parameters) { }

        public bool CheckCount(int tupleCount)
        {
            switch (CountMode)
            {
                case VarCountMode.Greater:
                    return Count > tupleCount;
                case VarCountMode.GreaterOrEq:
                    return Count >= tupleCount;
                case VarCountMode.Less:
                    return Count < tupleCount;
                case VarCountMode.LessOrEq:
                    return Count <= tupleCount;
                case VarCountMode.Equals:
                    return Count == tupleCount;
                case VarCountMode.Any:
                    return true;
                default:
                    return false;
            }
        }
    }
}