using System;
using System.Collections.Generic;
using Nevermind.ByteCode.NMB;

namespace Nevermind.Compiler.Semantics.Attributes
{
    internal class VariadicAttribute : Attribute
    {
        protected override CompileError VerifyParameters()
        {
            return null;
        }

        public VariadicAttribute(List<Token> parameters) : base(AttributeType.Variadic, parameters) { }
    }
}