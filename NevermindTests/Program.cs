using System;
using System.IO;
using Nevermind;

namespace NevermindTests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var source = new NmSource(new FileInfo("../../file.nm"));
            var program = new NmProgram(source);

            var error = program.Compile();
            if (error != null)
            {
                Console.WriteLine(error);
            }
        }
    }
}