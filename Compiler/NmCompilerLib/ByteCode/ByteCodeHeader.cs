using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
using Nevermind.ByteCode.InternalClasses;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;
using Nevermind.Compiler.Formats.Constants;
using Type = Nevermind.ByteCode.Types.Type;

namespace Nevermind.ByteCode
{
    internal class ByteCodeHeader
    {
        public NmProgram Program;

        public List<NumeratedType> UsedTypes;
        public List<NumeratedConstant> UsedConstants;

        public List<FunctionInstructions> EmbeddedFunctions;

        public ByteCodeHeader(NmProgram program)
        {
            Program = program;
            var typeIndex = 0;
            var constIndex = 0;

            UsedConstants = program.Constants.Distinct().Select(p => new NumeratedConstant(constIndex++, p)).ToList();

            program.UsedTypes.AddRange(UsedConstants.Select(p => p.Constant.ToProgramType()));
            UsedTypes = program.UsedTypes.Distinct().Select(p => new NumeratedType(typeIndex++, p)).ToList();

            var varCounter = 0;
            program.ProgramGlobals.ForEach(p => p.Index = varCounter++);

            EmbeddedFunctions = new List<FunctionInstructions>();
        }

        public Function GetFunction(string name, Token nearToken)
        {
            var native = Program.Functions.Find(p => p.Name == name);

            if (native != null)
            {
                if(native.Modifier == FunctionModifier.Finalization ||
                   native.Modifier == FunctionModifier.Initialization)
                    throw new CompileException(CompileErrorType.ModuleFunctionCall, nearToken, native.Token);

                return native;
            }


            int index = 0;
            foreach (var import in Program.Imports)
            {
                //ммм, какая ахуенная рекурсия
                var func = import.LinkedModule.Program.ByteCode.Instructions.Find(p => p.Function.Name == name);
                if (func != null)
                {
                    if(func.Function.Modifier != FunctionModifier.Public)
                        throw new CompileException(CompileErrorType.PrivateExportFunction, nearToken, func.Function.Token);

                    if(func.Function.Modifier == FunctionModifier.Finalization ||
                       func.Function.Modifier == FunctionModifier.Initialization)
                        throw new CompileException(CompileErrorType.ModuleFunctionCall, nearToken, func.Function.Token);

                    if (import.LinkedModule.IsLibrary)
                    {
                        //just linking
                        func.Function.ModuleIndex = index;
                    }
                    else
                    {
                        //embedding
                        EmbedFunction(func, nearToken);
                    }
                    return func.Function;
                }

                index++;
            }

            return null;
        }

        public void EmbedFunction(FunctionInstructions function, Token nearToken)
        {
            EmbeddedFunctions.Add(function);
            function.Function.Index = Program.Functions.Count + EmbeddedFunctions.Count - 1;
            function.Function.Name = $"_{function.Function.Program.Module.Name}_{function.Function.Name}";

            //merging types from locals
            foreach (var local in function.Locals)
                if (GetTypeIndex(local.Type) == -1)
                    UsedTypes.Add(new NumeratedType(UsedTypes.Count, local.Type));

            //merging types from constants
            foreach (var instruction in function.Instructions)
            {
                var locals = instruction.FetchUsedVariables(-2);
                foreach (var local in locals)
                {
                    if (local.VariableType == VariableType.LinkToConst)
                    {
                        var constant = function.Function.Program.ByteCode.Header.
                            UsedConstants[local.ConstIndex].Constant;

                        if (GetTypeIndex(constant.ToProgramType()) == -1)
                            UsedTypes.Add(new NumeratedType(UsedTypes.Count, constant.ToProgramType()));

                        int index;
                        if((index = GetConstIndex(constant)) == -1)
                        {
                            index = UsedConstants.Count;
                            UsedConstants.Add(new NumeratedConstant(index, constant));
                        }

                        local.ConstIndex = index;
                    }
                    else
                    {
                        //is global
                        if (local.IndexFixed) continue;
                        if (instruction.Function.Program.ProgramGlobals.Contains(local))
                        {
                            local.Index = Program.ProgramGlobals.IndexOf(local);
                        }
                        else
                        {
                            local.Index =
                                local.Index - function.Function.Program.ProgramGlobals.Count + Program.ProgramGlobals.Count;
                        }

                        local.IndexFixed = true;
                    }
                }

                if (instruction.Type == InstructionType.Call)
                {
                    var call = instruction as InstructionCall;
                    var func = call.DestFunc;
                    var newFunction = function.Function.Program.ByteCode.Instructions.Find(p => p.Function == func);

                    //recursive call
                    if(newFunction == function)
                        continue;

                    if(newFunction == null)
                        throw new ArgumentNullException(nameof(newFunction));

                    if(EmbeddedFunctions.Any(p => p.Function == func))
                        continue; //already embedded

                    if(newFunction.Function.Modifier == FunctionModifier.Finalization ||
                       newFunction.Function.Modifier == FunctionModifier.Initialization)
                        throw new CompileException(CompileErrorType.ModuleFunctionCall, nearToken, newFunction.Function.Token);

                    EmbedFunction(newFunction, nearToken);
                    call.DestFunc = newFunction.Function;
                }
            }
        }

        public int GetTypeIndex(Type t)
        {
            return UsedTypes.FindIndex(p => p.Type.Equals(t));
        }

        public int GetConstIndex(Constant c)
        {
            return UsedConstants.FindIndex(p => p.Constant.Equals(c));
        }

        public Variable GetGlobalVariable(string name)
        {
            return Program.ProgramGlobals.Find(p => p.Name == name);
        }

        public string ToSource()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Type count: {0}\n", UsedTypes.Count);
            sb.AppendLine("╔═╤═════════╤══════════╤═════════╗");
            sb.AppendLine("║#│  Index  │   Type   │   Base  ║");
            sb.AppendLine("╟─┼─────────┼──────────┼─────────╢");
            var counter = 0;
            foreach (var type in UsedTypes)
                sb.AppendFormat("║{0}│ {1, 3}     │ {2, 8} │   {3, 3}   ║\n", counter++, type.Index, type.Type.ID, type.Type.GetBase());
            sb.AppendLine("╚═╧═════════╧══════════╧═════════╝");


            counter = 0;
            sb.AppendFormat("\nConstants count: {0}\n", UsedConstants.Count);

            sb.AppendLine("╔═╤═════════╤═════════╤═════╤══════════════════╗");
            sb.AppendLine("║#│  Index  │ TypeInd │ Len │       Value      ║");
            sb.AppendLine("╟─┼─────────┼─────────┼─────┼──────────────────╢");
            foreach (var constant in UsedConstants)
            {
                var t = constant.Constant.ToProgramType();
                var len = 1;

                if (constant.Constant.Type == ConstantType.String)
                {
                    len = constant.Constant.SValue.Count;
                }

                sb.AppendFormat("║{0}│  {1,3}    │  {2,3}    │ {3,3} │ {4,14}   ║\n", counter++, constant.Index, GetTypeIndex(t),
                    len, constant.Constant.ToStringValue());
            }
            sb.AppendLine("╚═╧═════════╧═════════╧═════╧══════════════════╝");

            counter = 0;
            if (Program.ProgramGlobals.Count != 0)
            {
                sb.AppendFormat("\nGlobals count: {0}\n", Program.ProgramGlobals.Count);
                sb.AppendLine("╔═╤═══════╤═════════════════╤══════╤══════════════╗");
                sb.AppendLine("║#│ Index │       Name      │ Type │   InitValue  ║");
                sb.AppendLine("╟─┼───────┼─────────────────┼──────┼──────────────╢");
                foreach (var global in Program.ProgramGlobals)
                    sb.AppendFormat("║{0}│ {1, 3}   │ {2, 15} │ {3, 3}  │     {4, 3}      ║\n", counter++, global.Index,
                        global.Name, GetTypeIndex(global.Type), global.ConstIndex);
                sb.AppendLine("╚═╧═══════╧═════════════════╧══════╧══════════════╝");
            }

            return sb.ToString();
        }

        public Chunk GetGlobalsChunk()
        {
            var ch = new Chunk(ChunkType.GLOBALS);
            ch.Add(Program.ProgramGlobals.Count);

            foreach (var global in Program.ProgramGlobals)
                ch.Add(GetTypeIndex(global.Type));

            foreach (var global in Program.ProgramGlobals)
                ch.Add(global.ConstIndex);

            return ch;
        }

        public Chunk GetHeaderChunk()
        {
            var ch = new Chunk(ChunkType.HEAD);
            ch.Add(Codes.CurrentNMVersion);

            if(Program.Source.FileName == null)
                ch.Add(new byte[16]);
            else
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(Program.Source.FileName))
                    {
                        ch.Add(md5.ComputeHash(stream));
                    }
                }
            }

            ch.Add(Program.Imports.Count);
            ch.Add(Program.ByteCode.Instructions.Count);

            if(Program.EntrypointFunction == null)
                ch.Add(-1);
            else
                ch.Add(Program.EntrypointFunction.Index);

            foreach (var import in Program.Imports)
            {
                ch.Add(import.Library ? (byte)1 : (byte)0);
                if (import.Library)
                {
                    ch.Add(import.Name.Length);
                    ch.Add(import.Name.Select(p => (byte)p));
                }
                else
                {
                    ch.Add(import.FileName.Length);
                    ch.Add(import.FileName.Select(p => (byte)p));
                }

            }

            return ch;
        }

        public Chunk GetDebugChunk()
        {
            var ch = new Chunk(ChunkType.DEBUG);
            if (Program.Source.FileName != null)
            {
                var name = new FileInfo(Program.Source.FileName).FullName;
                ch.Add(name.Length);
                ch.Add(name.Select(p => (byte) p));
            }
            else
            {
                ch.Add(0);
            }

            foreach (var variable in Program.ProgramGlobals)
            {
                ch.Add(variable.Name.Length);
                ch.Add(variable.Name.Select(p => (byte)p));
                ch.Add(variable.Token.LineIndex);
                ch.Add(variable.Token.LineOffset);
            }

            foreach (var func in Program.Functions)
            {
                ch.Add(func.Name.Length);
                ch.Add(func.Name.Select(p => (byte)p));
                ch.Add(func.Token.LineIndex);
                ch.Add(func.Token.LineOffset);
                foreach (var variable in func.LocalVariables)
                {
                    ch.Add(variable.Name.Length);
                    ch.Add(variable.Name.Select(p => (byte)p));
                    ch.Add(variable.Token.LineIndex);
                    ch.Add(variable.Token.LineOffset);
                }
            }
            return ch;
        }

        public Chunk GetTypesChunk()
        {
            var ch = new Chunk(ChunkType.TYPE);
            ch.Add(UsedTypes.Count);
            foreach (var type in UsedTypes)
            {
                ch.Add(Codes.TypeIdDict[type.Type.ID]);
                ch.Add((byte)type.Type.GetBase());
            }
            return ch;
        }

        public Chunk GetConstChunk()
        {
            var ch = new Chunk(ChunkType.CONST);
            ch.Add(UsedConstants.Count);
            foreach (var constant in UsedConstants)
            {
                ch.Add(GetTypeIndex(constant.Constant.ToProgramType()));
                ch.Add(constant.Constant.Type == ConstantType.String
                    ? constant.Constant.SValue.Count : 1);

                ch.Add(constant.Constant.Serialize());
            }
            return ch;
        }
    }
}