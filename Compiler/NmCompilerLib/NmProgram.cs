using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.InternalClasses;
using Nevermind.ByteCode.Types;
using Nevermind.Compiler;
using Nevermind.Compiler.Formats.Constants;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.LexemeParsing.Lexemes;
using Nevermind.Compiler.Semantics;

namespace Nevermind
{
    public partial class NmProgram
    {
        internal NmSource Source;

        internal bool IsModule;
        internal Module Module;

        public NmMetadata Metadata { get; }
        public bool SaveDebugInfo { get; set; }
        public bool Verbose { get; set; }
        public bool DisableOptimization { get; set; }
        public List<string> IncludeDirectories { get; set; }
        public ByteCode.ByteCode ByteCode { get; set; }
        public bool MeasureTime { get; set; }

        internal bool PrototypesOnly { get; set; }
        internal NmProgram ParentProgram { get; set; }

        internal List<Lexeme> Lexemes;
        internal Function EntrypointFunction;
        internal readonly List<Import> Imports;
        internal readonly List<Variable> ProgramGlobals;
        internal readonly List<Constant> Constants;
        internal readonly List<Function> Functions;
        internal readonly List<ByteCode.Types.Type> UsedTypes;

        private readonly Dictionary<ElapsedTimeType, TimeSpan> _time = new Dictionary<ElapsedTimeType, TimeSpan>();
        private DateTime _start;

        public static void InitCompiler()
        {
            Tokenizer.InitTokenizer();
        }

        private void StartMeasureTime()
        {
            if(MeasureTime)
                _start = DateTime.Now;
        }

        private void EndMeasureTime(ElapsedTimeType type)
        {
            if(MeasureTime)
                _time[type] = TimeSpan.FromMilliseconds((DateTime.Now - _start).TotalMilliseconds);
        }

        public TimeSpan GetElapsedTime(ElapsedTimeType type)
        {
            if(!MeasureTime)
                return TimeSpan.Zero;

            if(type == ElapsedTimeType.Total)
            {
                return TimeSpan.FromMilliseconds(_time.Values.ToList().Sum(p => p.TotalMilliseconds));
            }
            else
            {
                TimeSpan result;
                return !_time.TryGetValue(type, out result) ? TimeSpan.Zero : result;
            }
        }

        public NmProgram(NmSource source, NmMetadata metadata = null)
        {
            if(source == null)
                throw new ArgumentNullException(nameof(source));

            Source = source;

            ProgramGlobals = new List<Variable>();
            Constants = new List<Constant>();
            Functions = new List<Function>();

            UsedTypes = new List<ByteCode.Types.Type>();
            Imports = new List<Import>();

            Metadata = metadata;
        }

        public CompileError Parse()
        {
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
            catch (CompileException ex)
            {
                return ex.ToError();
            }

            StartMeasureTime();
            Lexemes = Lexemizer.Lexemize(tokens, out error, Verbose, PrototypesOnly);
            EndMeasureTime(ElapsedTimeType.Lexemizing);
            if (error != null)
                return error;

            StartMeasureTime();
            if ((error = StructureParser.Parse(this, PrototypesOnly)) != null)
                return error;
            EndMeasureTime(ElapsedTimeType.StructureParsing);

            return null;
        }

        public CompileError Expand()
        {
            try
            {
                StartMeasureTime();
                ByteCode = new ByteCode.ByteCode(this);
                ByteCode.Proceed();
                EndMeasureTime(ElapsedTimeType.Expanding);

                //Optimizing only main files
                if (!DisableOptimization && EntrypointFunction != null)
                {
                    StartMeasureTime();
                    ByteCode.Optimize();
                    EndMeasureTime(ElapsedTimeType.Optimizing);
                }
            }
            catch (CompileException ex)
            {
                return ex.ToError();
            }

            return null;
        }
    }
}