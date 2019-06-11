using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode;
using Nevermind.ByteCode.Functions;
using Nevermind.Compiler;
using Nevermind.Compiler.Formats.Constants;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.Semantics;
using Type = System.Type;

namespace Nevermind
{
    public partial class NmProgram
    {
        internal NmSource Source;

        internal bool IsModule;
        internal Module Module;

        internal List<Import> Imports;
        internal List<Variable> ProgramLocals;

        public ByteCode.ByteCode Program;
        internal List<Lexeme> Lexemes;
        internal List<Constant> Constants;

        internal List<Function> Functions;
        internal Function EntrypointFunction;

        internal List<ByteCode.Type> UsedTypes;
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

        public NmProgram(NmSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));

            ProgramLocals = new List<Variable>();
            Constants = new List<Constant>();
            Functions = new List<Function>();

            UsedTypes = new List<ByteCode.Type>();
            AvailableTypes = BuiltInTypes.Get();
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
                EndMeasureTime(ElapsedTimeType.Tokenizing);
            }
            catch (ParseException ex)
            {
                return ex.ToError();
            }

            StartMeasureTime();
            Lexemes = Lexemizer.Lexemize(tokens, out error);
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