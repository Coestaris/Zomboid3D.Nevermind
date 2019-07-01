﻿
using System;
using System.IO;
using System.Text;
using Nevermind;
using Nevermind.Compiler;
using NevermindCompiler.CLParser;
#pragma warning disable 649

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
        public static string ToPrettyFormat(TimeSpan span)
        {
            if (span == TimeSpan.Zero) return "<0 ms";

            var sb = new StringBuilder();
            if (span.Days > 0)
                sb.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : string.Empty);
            if (span.Hours > 0)
                sb.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : string.Empty);
            if (span.Minutes > 0)
                sb.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : string.Empty);
            if (span.Seconds > 0)
                sb.AppendFormat("{0} second{1} ", span.Seconds, span.Seconds > 1 ? "s" : string.Empty);
            if (span.Milliseconds > 0)
                sb.AppendFormat("{0} ms", span.Milliseconds);
            return sb.ToString();
        }

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

        private static void Run(Options options, bool errors)
        {
            if(errors || options == null) return;

            var source = NmSource.FromFile(options.InputFileName);
            NmMetadata metadata = null;

            if (options.BinaryName != null ||
                options.BinaryAuthor != null ||
                options.BinaryDescription != null ||
                options.BinaryVersion != null)
            {
                var maj = 1;
                var min = 0;

                if(options.BinaryVersion != null)
                {
                    maj = int.Parse(options.BinaryVersion.Split('.')[0]);
                    min = int.Parse(options.BinaryVersion.Split('.')[1]);
                }

                metadata = new NmMetadata(options.BinaryName ?? "name",options.BinaryDescription ?? "descr",
                    options.BinaryAuthor ?? "author", DateTime.Now, (ushort)maj, (ushort)min);
            }

            var program = new NmProgram(source, metadata)
            {
                SaveDebugInfo = options.UseDebugChunk,
                Verbose = options.Verbose
            };
            CompileError error;
            if ((error = program.Parse()) != null)
            {
                Console.WriteLine("Parse error: {0}", error);
            }
            else if ((error = program.Expand()) != null)
            {
                Console.WriteLine("Expand error: {0}", error);
            }
            else
            {
                if (options.PrintTime)
                {
                    Console.WriteLine("\n\nElapsed Time (Total: {0}): ",
                        ToPrettyFormat(program.GetElapsedTime(ElapsedTimeType.Total)));
                    var i = 0;
                    foreach (ElapsedTimeType type in (ElapsedTimeType[]) Enum.GetValues(typeof(ElapsedTimeType)))
                    {
                        if (type == ElapsedTimeType.Total) continue;
                        Console.WriteLine("{0}. {1} - {2}", i++, type, ToPrettyFormat(program.GetElapsedTime(type)));
                    }
                }

                if (options.PrintByteCode)
                {
                    Console.WriteLine("\n\nASM Program:\n");
                    Console.WriteLine(program.Program.ToSource());
                }
                try
                {
                    using (var f = File.OpenRead(options.OutputFileName)) ;
                }
                catch
                {
                    Console.WriteLine("Unable to write file {0}", options.OutputFileName);
                    return;
                }

                program.Program.SaveToFile(options.OutputFileName);
                Console.WriteLine("\nFile saved!");
            }
        }
    }
}