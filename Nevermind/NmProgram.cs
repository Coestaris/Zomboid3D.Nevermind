using System;
using System.Collections.Generic;
using Nevermind.ByteCode;
using Nevermind.ByteCode.Functions;
using Nevermind.Compiler;
using Nevermind.Compiler.Formats.Constants;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.Semantics;

namespace Nevermind
{
    public class NmProgram
    {
        internal NmSource Source;

        internal bool IsModule;
        internal Module Module;

        internal List<Import> Imports;
        internal List<Variable> ProgramLocals;

        internal List<Instruction> Program;
        internal List<Lexeme> Lexemes;
        internal List<Constant> Constants;

        internal List<Function> Functions;
        internal Function EntrypointFunction;

        internal List<NamedType> AvailableTypes;

        public NmProgram(NmSource source)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));
            Source = source;

            ProgramLocals = new List<Variable>();
            Constants = new List<Constant>();
            Functions = new List<Function>();

            AvailableTypes = BuiltInTypes.Get();
        }

        public CompileError Compile()
        {
            Tokenizer.InitTokenizer();

            CompileError error;
            var source = Source.GetSource(out error);
            if (error != null)
                return error;

            List<Token> tokens;
            try
            {
                tokens = Tokenizer.Tokenize(source, Source.FileName, this);
            }
            catch (ParseException ex)
            {
                return ex.ToError();
            }

            foreach (var token in tokens)
                Console.WriteLine(token);

            Lexemes = Lexemizer.Lexemize(tokens, out error);
            if (error != null)
                return error;

            if((error = StructureParser.Parse(this)) != null)
                return error;

            return null;
        }
    }
}