using System;
using System.Collections.Generic;
using Nevermind.ByteCode.NMB;

namespace Nevermind.Compiler.Semantics.Attributes
{
    internal class SyscallAttribute : Attribute
    {
        public Token CallName;
        public bool AppendToEnd;
        public UInt16 CallCode;

        protected override CompileError VerifyParameters()
        {
            CallName = Parameters[0];
            if (Parameters[1].StringValue == "end") AppendToEnd = true;
            else if (Parameters[1].StringValue == "begin") AppendToEnd = false;
            else return new CompileError(CompileErrorType.WrongSyscallAppendType, Parameters[1]);

            if(!Codes.SyscallTypes.TryGetValue(CallName.StringValue, out CallCode))
                return new CompileError(CompileErrorType.UnknownSyscallType, Parameters[0]);

            return null;
        }

        public SyscallAttribute(Token name, List<Token> parameters) : base(AttributeType.Syscall, name, parameters) { }
    }
}