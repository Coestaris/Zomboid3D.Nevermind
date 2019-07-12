using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nevermind.ByteCode;
using Nevermind.ByteCode.Functions;
using Nevermind.Compiler.Formats;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.LexemeParsing.Lexemes;
using Type = Nevermind.ByteCode.Types.Type;

namespace Nevermind.Compiler.Semantics
{
    internal static class StructureParser
    {
        public static CompileError Parse(NmProgram program, bool prototypesOnly)
        {
            CompileError error;
            var moduleLexeme = (ModuleLexeme)program.Lexemes.Find(p => p.Type == LexemeType.Module);
            if (moduleLexeme != null)
            {
                if(!IdentifierFormat.Match(moduleLexeme.ModuleName.StringValue))
                    return new CompileError(CompileErrorType.WrongModuleNameFormat, moduleLexeme.ModuleName);

                program.IsModule = true;
                program.Module = new Module(moduleLexeme.ModuleName.StringValue, program);
            }

            var functionIndex = 0;
            foreach (var lex in program.Lexemes)
            {
                if (lex.Type == LexemeType.Function)
                {
                    var lexeme = (FunctionLexeme) lex;
                    if (!IdentifierFormat.Match(lexeme.Name.StringValue))
                        return new CompileError(CompileErrorType.WrongFunctionNameFormat, lexeme.Name);

                    foreach (var parameter in lexeme.Parameters)
                        if (!IdentifierFormat.Match(parameter.Name.StringValue))
                            return new CompileError(CompileErrorType.WrongFunctionParameterNameFormat, parameter.Name);

                    if (program.Functions.Count(p => p.Name == lexeme.Name.StringValue) != 0)
                        return new CompileError(CompileErrorType.MultipleFunctionsWithSameName, lexeme.Name);

                    Function func;
                    if ((error = lexeme.ToFunc(program, out func)) != null)
                        return error;

                    func.Index = functionIndex++;
                    program.Functions.Add(func);

                    if (!prototypesOnly)
                    {
                        if ((error = program.Functions.Last().ResolveLexemes()) != null)
                            return error;
                    }

                    if (lexeme.Modifier == FunctionModifier.Initialization)
                    {
                        if (program.Module.InitializationFunc != null)
                            return new CompileError(CompileErrorType.MultipleInitializationFunctions, lexeme.Name);
                        program.Module.InitializationFunc = program.Functions.Last();
                    }

                    if (lexeme.Modifier == FunctionModifier.Finalization)
                    {
                        if (program.Module.FinalizationFunc != null)
                            return new CompileError(CompileErrorType.MultipleFinalizationFunctions, lexeme.Name);
                        program.Module.FinalizationFunc = program.Functions.Last();
                    }

                    if (lexeme.Modifier == FunctionModifier.Entrypoint)
                    {
                        if (program.EntrypointFunction != null)
                            return new CompileError(CompileErrorType.MultipleEntrypointFunctions, lexeme.Name);
                        program.EntrypointFunction = program.Functions.Last();
                    }
                }
                else if(lex.Type == LexemeType.Var)
                {
                    var lexeme = (VarLexeme)lex;
                    if (!IdentifierFormat.Match(lexeme.VarName.StringValue))
                        return new CompileError(CompileErrorType.WrongIdentifierFormat, lexeme.VarName);

                    if(program.ProgramGlobals.Find(p => p.Name == lexeme.VarName.StringValue) != null)
                        return new CompileError(CompileErrorType.VariableRedeclaration, lexeme.VarName);

                    Type t;
                    if ((error = Type.GetType(program, lexeme.TypeName, out t)) != null) return error;
                    program.ProgramGlobals.Add(new Variable(t, lexeme.VarName.StringValue, -1, lexeme.VarName, -1, VariableType.Variable));

                }
                else if(lex.Type == LexemeType.Import)
                {
                    var importName = (lex as ImportLexeme).ImportName.StringValue;
                    var files = new List<string>();

                    foreach (var directory in program.IncludeDirectories)
                        files.AddRange(Directory.GetFiles(directory, "*.nm"));

                    var matchedFile = files.Find(p => new FileInfo(p).Name == importName + ".nm");
                    if(matchedFile == null)
                        return new CompileError(CompileErrorType.UnknownModuleName, lex.Tokens?[0]);

                    Import import;
                    if((error = Import.CreateImport(out import, importName, matchedFile, program, lex.Tokens[0])) != null)
                        return error;

                    program.Imports.Add(import);

                    if ((error = import.Parse(program, lex.Tokens[0])) != null)
                        return error;
                }
                else if (lex.Type == LexemeType.Module)
                {
                    program.IsModule = true;
                    program.Module = new Module((lex as ModuleLexeme).ModuleName.StringValue, program)
                    {
                        IsLibrary = (lex as ModuleLexeme).IsLibrary
                    };
                }
                else
                {
                    return new CompileError(CompileErrorType.UnexpectedLexeme, lex.Tokens?[0]);
                }
            }


            if (program.IsModule)
            {
                if (program.Module.InitializationFunc == null)
                    return new CompileError(CompileErrorType.NoInitializationFunction, new Token(program.Source.FileName));
                if (program.Module.FinalizationFunc == null)
                    return new CompileError(CompileErrorType.NoFinalizationFunction, new Token(program.Source.FileName));
                if(program.EntrypointFunction != null)
                    return new CompileError(CompileErrorType.ModuleWithEntrypointFunction, new Token(program.Source.FileName));
            }
            else
            {
                if(program.EntrypointFunction == null)
                    return new CompileError(CompileErrorType.NoEntrypointFunction, new Token(program.Source.FileName));
            }

            return null;
        }
    }
}