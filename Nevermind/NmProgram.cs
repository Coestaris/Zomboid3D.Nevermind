using System;
using System.Collections.Generic;
using Nevermind.ByteCode;
using Nevermind.Compiler;
using Nevermind.Compiler.Formats.Constants;

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
            Tokenizer.InitTokenizer();

            CompileError error;
            var source = _source.GetSource(out error);
            if (error != null)
                return error;

            List<Token> tokens;
            try
            {
                tokens = Tokenizer.Tokenize(source, _source.FileName, this);
            }
            catch (ParseException ex)
            {
                return ex.ToError();
            }

            foreach (var token in tokens)
                Console.WriteLine(token);

            var lexems = Lexemizer.Lexemize(tokens, out error);
            if (error != null)
                return error;

            return null;
        }
    }
}