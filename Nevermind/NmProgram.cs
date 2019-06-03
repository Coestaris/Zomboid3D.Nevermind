using System;
using System.Collections.Generic;
using Nevermind.ByteCode;
using Nevermind.Compiler;
using Nevermind.Compiler.Constants;

namespace Nevermind
{
    public class NmProgram
    {
        private NmSource _source;
        private List<Instruction> _program;

        internal List<Constant> Constants;

        public NmProgram(NmSource source)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));
            _source = source;

            Constants = new List<Constant>();
        }

        public CompileError Compile()
        {
            List<Token> tokens;

            CompileError error;
            var source = _source.GetSource(out error);
            if (error != null)
                return error;

            tokens = Tokenizer.Tokenize(source, _source.FileName);

            foreach (var token in tokens)
                Console.WriteLine(token);

            var lexems = Lexemizer.Lexemize(tokens, out error);
            if (error != null)
                return error;

            return null;
        }
    }
}