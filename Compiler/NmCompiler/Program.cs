
using System;
using NevermindCompiler.CLParser;

namespace NevermindCompiler
{
    internal class Options
    {
        [Flag("verbose", 'v', "Prints all debug data")]
        public bool Verbose;

        [Flag("time", 't', "Prints elapsed time")]
        public bool PrintTime;

        [Flag("bytecode", 'b', "Print formatted result bytecode")]
        public bool PrintByteCode;

        [Flag("debug", 'd', "Save debug information into binary")]
        public bool UseDebugChunk;

        [Value("log-file", 'l', "Output log filename", null, false, true)]
        public string LogFileName;

        [InlineValue("source-fileName", "File to be compiled", true)]
        [Value("input", 'i', "File to be compiled", null, false, true)]
        public string InputFileName;

        [Value("output", 'o', "Output binary filename", null, true, false)]
        public string OutputFileName;

        [Value("metadata", 'm', "Input metadata JSON file", null, false, true)]
        public string MetadataFileName;

        [Value("binary-name", 'n', "Binary name")]
        public string BinaryName;

        [Value("binary-description", 'd', "Binary description")]
        public string BinaryDescription;

        [Value("binary-author", 'a', "Binary author")]
        public string BinaryAuthor;

        [Value("binary-version", 'e', "Binary Version in Format <Major Vers>.<Min Vers>")]
        public string BinaryVersion;
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            new ArgumentParser<Options>()
                .SetParameters(p =>
                {
                    p.AutoHelp = true;
                    p.AutoVersion = true;
                    p.HelpCommand = "help";
                    p.VersionCommand = "version";
                    p.Prefix = "-";
                    p.FullPrefix = "--";
                    p.BinaryName = "nmc";
                    p.Description = "Nevermind language compiler";
                })
                .ParseArguments(args).Run(Run);
        }

        private static void Run(Options options, bool b)
        {
            if(b) return;
        }
    }
}