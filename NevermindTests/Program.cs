using System;
using System.IO;
using Nevermind;
using Nevermind.Compiler;

namespace NevermindTests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var source = NmSource.FromFile("../../../Examples/sample.nm");
            var program = new NmProgram(source);

            CompileError error;
            if((error = program.Parse()) != null) { Console.WriteLine("Parse error: {0}", error); return; }
            if((error = program.Expand()) != null) { Console.WriteLine("Expand error: {0}", error); return; }

            Console.WriteLine(program.Program.ToSource());

            Console.ReadKey();
        }
    }
}