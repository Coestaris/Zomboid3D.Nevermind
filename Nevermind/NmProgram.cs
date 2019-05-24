using System;
using System.Collections.Generic;
using Nevermind.ByteCode;

namespace Nevermind
{
    public class NmProgram
    {
        private NmSource _source;
        private List<Instruction> _program;

        public NmProgram(NmSource source)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));
            _source = source;
        }

        public void Compile()
        {

        }
    }
}