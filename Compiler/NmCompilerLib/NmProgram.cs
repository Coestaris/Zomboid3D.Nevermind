using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.InternalClasses;
using Nevermind.ByteCode.Types;
using Nevermind.Compiler;
using Nevermind.Compiler.Formats.Constants;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.Semantics;

namespace Nevermind
{
    public partial class NmProgram
    {
        internal NmSource Source;

        internal bool IsModule;
        internal Module Module;

        public NmMetadata Metadata;
        public bool SaveDebugInfo;
        public bool Verbose;

        internal List<Import> Imports;
        internal List<Variable> ProgramLocals;

        public ByteCode.ByteCode Program;
        internal List<Lexeme> Lexemes;
        internal List<Constant> Constants;

        internal List<Function> Functions;
        internal Function EntrypointFunction;

        internal List<ByteCode.Types.Type> UsedTypes;
        internal List<NamedType> AvailableTypes;

        private Dictionary<ElapsedTimeType, TimeSpan> _time = new Dictionary<ElapsedTimeType, TimeSpan>();
        private DateTime _start;
        private void StartMeasureTime()
        {
            _start = DateTime.Now;
        }

        private void EndMeasureTime(ElapsedTimeType type)
        {
            _time[type] = TimeSpan.FromMilliseconds((DateTime.Now - _start).TotalMilliseconds);
        }

        public TimeSpan GetElapsedTime(ElapsedTimeType type)
        {
            if(type == ElapsedTimeType.Total)
            {
                return TimeSpan.FromMilliseconds(_time.Values.ToList().Sum(p => p.TotalMilliseconds));
            }
            else return _time[type];
        }

        public NmProgram(NmSource source, NmMetadata metadata = null)
        {
            if(source == null)
                throw new ArgumentNullException(nameof(source));

            Source = source;

            ProgramLocals = new List<Variable>();
            Constants = new List<Constant>();
            Functions = new List<Function>();

            UsedTypes = new List<ByteCode.Types.Type>();
            AvailableTypes = BuiltInTypes.Get();
            Imports = new List<Import>();

            Metadata = metadata;
        }

        public CompileError Parse()
        {
            Tokenizer.InitTokenizer();

            CompileError error;
            StartMeasureTime();
            var source = Source.GetSource(out error);
            if (error != null)
                return error;
            EndMeasureTime(ElapsedTimeType.SourceReading);

            List<Token> tokens;
            try
            {
                StartMeasureTime();
                tokens = Tokenizer.Tokenize(source, Source.FileName, this);

                if(Verbose)
                    Console.WriteLine(string.Join("\n", tokens));

                EndMeasureTime(ElapsedTimeType.Tokenizing);
            }
            catch (ParseException ex)
            {
                return ex.ToError();
            }

            StartMeasureTime();
            Lexemes = Lexemizer.Lexemize(tokens, out error, Verbose);
            EndMeasureTime(ElapsedTimeType.Lexemizing);
            if (error != null)
                return error;

            StartMeasureTime();
            if ((error = StructureParser.Parse(this)) != null)
                return error;
            EndMeasureTime(ElapsedTimeType.StructurePasing);

            return null;
        }

        public CompileError Expand()
        {
            try
            {
                StartMeasureTime();
                Program = new ByteCode.ByteCode(this);
                Program.Proceed();
                EndMeasureTime(ElapsedTimeType.Expanding);
            }
            catch (ParseException ex)
            {
                return ex.ToError();
            }

            return null;
        }
    }
}