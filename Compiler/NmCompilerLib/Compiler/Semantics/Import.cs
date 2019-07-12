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

        public static bool HasModuleInStack(string name, NmProgram parent)
        {
            while (true)
            {
                if (parent.Imports.Find(p => p.Name == name) != null)
                    return true;
                parent = parent.ParentProgram;

                if (parent == null)
                    return false;
            }
        }

        public static CompileError CreateImport(out Import import, string name, string fileName, NmProgram program,
            Token token)
        {
            //todo: library modules
            /*
            var compilerBinaryName = new FileInfo(fileName).Directory.FullName + Path.DirectorySeparatorChar +
                                     new FileInfo(fileName).Name + "b";
            if (File.Exists(compilerBinaryName))
            {
                //load needed data from binary
            }
            */

            import = null;


            if(HasModuleInStack(name, program))
                return new CompileError(CompileErrorType.RecursiveImport, token);

            import = new Import
            {
                Name = name,
                Library = true,
                FileName = fileName,
            };

            return null;
        }

        internal CompileError Parse(NmProgram program, Token token)
        {
            var source = NmSource.FromFile(FileName);
            var newProgram = new NmProgram(source)
            {
                Verbose = program.Verbose,
                MeasureTime = false,
                IncludeDirectories = program.IncludeDirectories,
                ParentProgram = program
            };

            if(program.Verbose)
                Console.WriteLine("Compiling {0}", FileName);

            CompileError error;
            if ((error = newProgram.Parse()) != null)
            {
                Console.WriteLine(error);
                return new CompileError(CompileErrorType.InnerCompileException, token);
            }

            if ((error = newProgram.Expand()) != null)
            {
                Console.WriteLine(error);
                return new CompileError(CompileErrorType.InnerCompileException, token);
            }

            if(!newProgram.IsModule)
                return new CompileError(CompileErrorType.NotModuleImport, token);

            //todo: library modules
            /*if (newProgram.Module.IsLibrary)
            {
                newProgram.ByteCode.SaveToFile(compilerBinaryName);
                if (program.Verbose)
                    Console.WriteLine("File saved {0}", compilerBinaryName);
            }*/

            LinkedModule = newProgram.Module;

            return null;
        }
    }
}