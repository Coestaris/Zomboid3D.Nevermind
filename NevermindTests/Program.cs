using System;
using System.IO;
using System.Text;
using Nevermind;
using Nevermind.Compiler;

namespace NevermindTests
{
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
            var source = NmSource.FromFile("../../../Examples/sample.nm");
            var metadata = new NmMetadata("Sample Binary", "Example Nevermind Binary",
                "Coestaris", DateTime.Now, 0, 1);
            var program = new NmProgram(source, metadata);

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
                Console.WriteLine("\n\nElapsed Time (Total: {0}): ", ToPrettyFormat(program.GetElapsedTime(ElapsedTimeType.Total)));
                var i = 0;
                foreach (ElapsedTimeType type in (ElapsedTimeType[])Enum.GetValues(typeof(ElapsedTimeType)))
                {
                    if (type == ElapsedTimeType.Total) continue;
                    Console.WriteLine("{0}. {1} - {2}", i++, type, ToPrettyFormat(program.GetElapsedTime(type)));
                }
                Console.WriteLine("\n\nASM Program:\n");
                Console.WriteLine(program.Program.ToSource());


                program.Program.SaveToFile("../../../Examples/sample.nmb");
                Console.WriteLine("File saved!");
            }

            Console.ReadKey();
        }
    }
}