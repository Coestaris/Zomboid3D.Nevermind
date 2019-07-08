using System;
using System.Collections.Generic;
using System.IO;

namespace Nevermind.Compiler.Semantics
{
    internal class Import
    {
        public bool Library;

        public string Name;
        public string FileName;

        public Module LinkedModule;

        public static CompileError CreateImport(string name, string fileName, out Import import, NmProgram program)
        {
            var compilerBinaryName = new FileInfo(fileName).Directory.FullName + Path.DirectorySeparatorChar +
                                     new FileInfo(fileName).Name + "b";
            import = null;

            if (File.Exists(compilerBinaryName))
            {
                //load needed data from binary
            }

            var source = NmSource.FromFile(fileName);
            var newProgram = new NmProgram(source)
            {
                Verbose = program.Verbose,
                MeasureTime = false,
                IncludeDirectories = program.IncludeDirectories
            };

            //todo: pass token for error index
            if(program.Verbose)
                Console.WriteLine("Compiling {0}", fileName);
            CompileError error;
            if ((error = newProgram.Parse()) != null)
            {
                Console.WriteLine(error);
                return new CompileError(CompileErrorType.InnerCompileException);
            }

            if ((error = newProgram.Expand()) != null)
            {
                Console.WriteLine(error);
                return new CompileError(CompileErrorType.InnerCompileException);
            }

            if(!newProgram.IsModule)
                return new CompileError(CompileErrorType.NotModuleImport);

            if (newProgram.Module.IsLibrary)
            {
                newProgram.ByteCode.SaveToFile(compilerBinaryName);
                if (program.Verbose)
                    Console.WriteLine("File saved {0}", compilerBinaryName);
            }

            import = new Import
            {
                Name = name,
                Library = true,
                FileName = fileName,
                LinkedModule = newProgram.Module
            };

            return null;
        }
    }
}