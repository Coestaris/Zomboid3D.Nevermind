
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nevermind;
using Nevermind.Compiler;
using NevermindCompiler.CLParser;
using Newtonsoft.Json.Linq;
#pragma warning disable 649

namespace NevermindCompiler
{
    internal class Options
    {
        [Flag("verbose", 'v', "Prints all debug data")]
        public bool Verbose;

        [Flag("time", 't', "Prints elapsed time")]
        public bool PrintTime;

        [Flag("disable-optimization", Flag.NoName, "Disables all optimization during compilation")]
        public bool DisableOptimization;

        [Flag("no-auto-metadata", 'n', "Ignore .json files with possible metadata")]
        public bool IgnoreMetadataFiles;

        [Flag("bytecode", 'b', "Print formatted result bytecode")]
        public bool PrintByteCode;

        [Flag("debug", 'd', "Save debug information into binary")]
        public bool UseDebugChunk;

        [Value("log-file", 'l', "Output log filename")]
        public string LogFileName;

        [InlineValue("source-fileName", "File to be compiled", true)]
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

        [Value("include", 'i', "Comma separated include paths")]
        public string IncludeDirectories;
    }

    internal static class Program
    {
        private static string ToPrettyFormat(TimeSpan span)
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

        private static NmMetadata ParseJSON(string filename)
        {
            try
            {
                var obj = JObject.Parse(File.ReadAllText(filename).ToLower());

                JToken name, descr, author, vers;

                if(!obj.ContainsKey("name") || (name = obj.GetValue("name")).Type != JTokenType.String)
                {
                    Console.WriteLine("JSON file should have string field named \"name'\"");
                    return null;
                }

                if(!obj.ContainsKey("description") || (descr = obj.GetValue("description")).Type != JTokenType.String)
                {
                    Console.WriteLine("JSON file should have string field named \"description'\"");
                    return null;
                }

                if(!obj.ContainsKey("author") || (author = obj.GetValue("author")).Type != JTokenType.String)
                {
                    Console.WriteLine("JSON file should have string field named \"author'\"");
                    return null;
                }

                if(!obj.ContainsKey("version") || (vers = obj.GetValue("version")).Type != JTokenType.String)
                {
                    Console.WriteLine("JSON file should have string field named \"version'\"");
                    return null;
                }

                int maj = 1, min = 0;
                try
                {
                    var v = vers.ToObject<string>();
                    maj = int.Parse(v.Split('.')[0]);
                    min = int.Parse(v.Split('.')[1]);
                }
                catch
                {
                    Console.WriteLine("\"{0}\" is wrong version format", vers);
                    return null;
                }

                return new NmMetadata(
                    name.ToObject<string>(),
                    descr.ToObject<string>(),
                    author.ToObject<string>(),
                    DateTime.Now,
                    (ushort)min, (ushort)maj);
            }
            catch
            {
                Console.WriteLine("\"{0}\" is not valid JSON", filename);
                return null;
            }
        }

        private static void ResetOutput()
        {
            Console.Out.Close();
            var standardOutput = new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true};
            Console.SetOut(standardOutput);
        }

        private static void Run(Options options, bool errors)
        {
            if(errors || options == null) return;

            if (options.MetadataFileName == null && !options.IgnoreMetadataFiles)
            {
                var splitted = options.InputFileName.Split('.');
                var fn = string.Join(".", splitted.Take(splitted.Length - 1)) + ".json";
                if (File.Exists(fn))
                {
                    options.MetadataFileName = fn;
                    Console.WriteLine("Using \"{0}\" as metadata file", fn);
                }
            }

            var source = NmSource.FromFile(options.InputFileName);

            var metadata = options.MetadataFileName != null ? ParseJSON(options.MetadataFileName) : null;

            if (metadata == null &&
                (options.BinaryName != null ||
                options.BinaryAuthor != null ||
                options.BinaryDescription != null ||
                options.BinaryVersion != null))
            {
                var maj = 1;
                var min = 0;

                if(options.BinaryVersion != null)
                {
                    try
                    {
                        maj = int.Parse(options.BinaryVersion.Split('.')[0]);
                        min = int.Parse(options.BinaryVersion.Split('.')[1]);
                    }
                    catch
                    {
                        Console.WriteLine("\"{0}\" is wrong version format", options.BinaryVersion);
                    }
                }

                metadata = new NmMetadata(options.BinaryName ?? "name",options.BinaryDescription ?? "descr",
                    options.BinaryAuthor ?? "author", DateTime.Now, (ushort)maj, (ushort)min);
            }

            var includeList = new List<string>();
            if (options.IncludeDirectories != null)
                includeList.AddRange(options.IncludeDirectories.Split(','));
            else
            {
                var dirName = new FileInfo(options.InputFileName).Directory.FullName;
                includeList.Add(dirName);
                Console.WriteLine("Using \"{0}\" as include path", dirName);

                var possibleName = dirName + Path.DirectorySeparatorChar + "modules";
                if (Directory.Exists(possibleName))
                {
                    includeList.Add(possibleName);
                    Console.WriteLine("Using \"{0}\" as include path", possibleName);
                }

                possibleName = dirName + Path.DirectorySeparatorChar + "lib";
                if (Directory.Exists(possibleName))
                {
                    includeList.Add(possibleName);
                    Console.WriteLine("Using \"{0}\" as include path", possibleName);
                }
            }

            NmProgram.InitCompiler();
            var program = new NmProgram(source, metadata)
            {
                SaveDebugInfo = options.UseDebugChunk,
                Verbose = options.Verbose,
                DisableOptimization = options.DisableOptimization,
                IncludeDirectories = includeList,
                MeasureTime = options.PrintTime
            };

            if (options.LogFileName != null)
            {
                Stream f = File.OpenWrite(options.LogFileName);
                TextWriter writer = new StreamWriter(f);
                Console.SetOut(writer);
            }

            CompileError error;
            if ((error = program.Parse()) != null)
            {
                if (options.LogFileName != null)
                    ResetOutput();

                Console.WriteLine("Parse error: {0}", error);
            }
            else if ((error = program.Expand()) != null)
            {
                if (options.LogFileName != null)
                    ResetOutput();

                Console.WriteLine("Expand error: {0}", error);
            }
            else
            {
                if (options.LogFileName != null)
                    ResetOutput();

                if (options.PrintTime)
                {
                    Console.WriteLine("Elapsed Time (Total: {0}): ",
                        ToPrettyFormat(program.GetElapsedTime(ElapsedTimeType.Total)));
                    var i = 0;
                    foreach (var type in (ElapsedTimeType[]) Enum.GetValues(typeof(ElapsedTimeType)))
                    {
                        if (type == ElapsedTimeType.Total) continue;
                        var time = program.GetElapsedTime(type);
                        if(time != TimeSpan.Zero)
                            Console.WriteLine("{0}. {1} - {2}", i++, type, ToPrettyFormat(time));
                    }
                }

                if (options.PrintByteCode)
                {
                    Console.WriteLine("ASM Program:\n");
                    Console.WriteLine(program.ByteCode.ToSource());
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

                program.ByteCode.SaveToFile(options.OutputFileName);
                Console.WriteLine("File saved!");
            }
        }
    }
}