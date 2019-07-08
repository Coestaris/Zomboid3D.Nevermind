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

            EmbeddedFunctions = new List<FunctionInstructions>();
        }

        public Function GetFunction(string name, Token nearToken)
        {
            var native = Program.Functions.Find(p => p.Name == name);

            if (native != null)
            {
                if(native.Modifier == FunctionModifier.Finalization ||
                   native.Modifier == FunctionModifier.Initialization)
                    throw new ParseException(CompileErrorType.ModuleFunctionCall, nearToken);

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
                        throw new ParseException(CompileErrorType.ImportFunctionShouldBePublic, nearToken);

                    if(func.Function.Modifier == FunctionModifier.Finalization ||
                       func.Function.Modifier == FunctionModifier.Initialization)
                        throw new ParseException(CompileErrorType.ModuleFunctionCall, nearToken);

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

        private void EmbedFunction(FunctionInstructions function, Token nearToken)
        {
            EmbeddedFunctions.Add(function);
            function.Function.Index = Program.Functions.Count + EmbeddedFunctions.Count;

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
                }

                if (instruction.Type == InstructionType.Call)
                {
                    var func = (instruction as InstructionCall).DestFunc;
                    var newFunction = function.Function.Program.ByteCode.Instructions.Find(p => p.Function == func);

                    //recursive call
                    if(newFunction == function)
                        continue;

                    if(newFunction == null)
                        throw new ArgumentNullException(nameof(newFunction));

                    if(newFunction.Function.Modifier == FunctionModifier.Finalization ||
                       newFunction.Function.Modifier == FunctionModifier.Initialization)
                        throw new ParseException(CompileErrorType.ModuleFunctionCall, nearToken);

                    EmbedFunction(newFunction, nearToken);
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

        public string ToSource()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(".type_count: {0}\n", UsedTypes.Count);
            sb.AppendLine("//Index : type : [base]");
            var counter = 0;
            foreach (var type in UsedTypes)
                sb.AppendFormat("{0}. {1} = {2}:{3}\n", counter++, type.Index, type.Type.ID, type.Type.GetBase());

            counter = 0;
            sb.AppendFormat("\n.const_count: {0}\n", UsedTypes.Count);
            sb.AppendLine("//index = (type index : [len]) value");
            foreach (var constant in UsedConstants)
            {
                var t = constant.Constant.ToProgramType();
                if (constant.Constant.Type == ConstantType.String)
                {
                    sb.AppendFormat("{0}. ^{1} = (t : {2}:{3}) {4}\n", counter++, constant.Index, GetTypeIndex(t),
                        constant.Constant.SValue.Count, constant.Constant.ToStringValue());
                }
                else
                {
                    sb.AppendFormat("{0}. ^{1} = (t : {2}) {3}\n", counter++, constant.Index, GetTypeIndex(t),
                        constant.Constant.ToStringValue());
                }
            }

            return sb.ToString();
        }

        public Chunk GetHeaderChunk()
        {
            var ch = new Chunk(ChunkType.HEAD);
            ch.Data.AddRange(Chunk.UInt16ToBytes(Codes.CurrentNMVersion));

            if(Program.Source.FileName == null)
                ch.Data.AddRange(new byte[16]);
            else
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(Program.Source.FileName))
                    {
                        ch.Data.AddRange(md5.ComputeHash(stream));
                    }
                }
            }

            ch.Data.AddRange(Chunk.Int32ToBytes(Program.Imports.Count));
            ch.Data.AddRange(Chunk.Int32ToBytes(Program.ByteCode.Instructions.Count));

            if(Program.EntrypointFunction == null)
                ch.Data.AddRange(Chunk.Int32ToBytes(-1));
            else
                ch.Data.AddRange(Chunk.Int32ToBytes(Program.EntrypointFunction.Index));

            foreach (var import in Program.Imports)
            {
                ch.Data.Add(import.Library ? (byte)1 : (byte)0);
                ch.Data.AddRange(Chunk.Int32ToBytes(import.Name.Length));
                ch.Data.AddRange(import.Name.Select(p => (byte)p));
            }

            return ch;
        }

        public Chunk GetDebugChunk()
        {
            var ch = new Chunk(ChunkType.DEBUG);
            if (Program.Source.FileName != null)
            {
                var name = new FileInfo(Program.Source.FileName).FullName;
                ch.Data.AddRange(Chunk.Int32ToBytes(name.Length));
                ch.Data.AddRange(name.Select(p => (byte) p));
            }
            else
            {
                ch.Data.AddRange(Chunk.Int32ToBytes(0));
            }

            foreach (var func in Program.Functions)
            {
                ch.Data.AddRange(Chunk.Int32ToBytes(func.Name.Length));
                ch.Data.AddRange(func.Name.Select(p => (byte)p));
                ch.Data.AddRange(Chunk.Int32ToBytes(func.Token.LineIndex));
                ch.Data.AddRange(Chunk.Int32ToBytes(func.Token.LineOffset));
                foreach (var variable in func.LocalVariables)
                {
                    ch.Data.AddRange(Chunk.Int32ToBytes(variable.Name.Length));
                    ch.Data.AddRange(variable.Name.Select(p => (byte)p));
                    ch.Data.AddRange(Chunk.Int32ToBytes(variable.Token.LineIndex));
                    ch.Data.AddRange(Chunk.Int32ToBytes(variable.Token.LineOffset));
                }
            }
            return ch;
        }

        public Chunk GetTypesChunk()
        {
            var ch = new Chunk(ChunkType.TYPE);
            ch.Data.AddRange(Chunk.Int32ToBytes(UsedTypes.Count));
            foreach (var type in UsedTypes)
            {
                ch.Data.AddRange(Chunk.Int16ToBytes(Codes.TypeIdDict[type.Type.ID]));
                ch.Data.Add((byte)type.Type.GetBase());
            }
            return ch;
        }

        public Chunk GetConstChunk()
        {
            var ch = new Chunk(ChunkType.CONST);
            ch.Data.AddRange(Chunk.Int32ToBytes(UsedConstants.Count));
            foreach (var constant in UsedConstants)
            {
                ch.Data.AddRange(Chunk.Int32ToBytes(GetTypeIndex(constant.Constant.ToProgramType())));
                ch.Data.AddRange(constant.Constant.Type == ConstantType.String
                    ? Chunk.Int32ToBytes(constant.Constant.SValue.Count)
                    : Chunk.Int32ToBytes(1));
                ch.Data.AddRange(constant.Constant.Serialize());
            }
            return ch;
        }
    }
}