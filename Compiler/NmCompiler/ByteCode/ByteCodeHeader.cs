using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.InternalClasses;
using Nevermind.ByteCode.NMB;
using Nevermind.ByteCode.Types;
using Nevermind.Compiler.Formats.Constants;

namespace Nevermind.ByteCode
{
    internal class ByteCodeHeader
    {
        public NmProgram Program;

        public List<NumeratedType> UsedTypes;
        public List<NumeratedConstant> UsedConstants;

        public ByteCodeHeader(NmProgram program)
        {
            Program = program;
            var typeIndex = 0;
            var constIndex = 0;

            UsedConstants = program.Constants.Distinct().Select(p => new NumeratedConstant(constIndex++, p)).ToList();

            program.UsedTypes.AddRange(UsedConstants.Select(p => p.Constant.ToProgramType()));
            UsedTypes = program.UsedTypes.Distinct().Select(p => new NumeratedType(typeIndex++, p)).ToList();
        }

        public Function GetFunction(string name)
        {
            //todo: Search in modules
            return Program.Functions.Find(p => p.Name == name);
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
            ch.Data.AddRange(Chunk.Int32ToBytes(Program.Imports.Count));
            ch.Data.AddRange(Chunk.Int32ToBytes(Program.Program.Instructions.Count));
            foreach (var import in Program.Imports)
            {
                ch.Data.Add(import.Library ? (byte)1 : (byte)0);
                ch.Data.AddRange(Chunk.Int32ToBytes(import.Name.Length));
                ch.Data.AddRange(import.Name.Select(p => (byte)p));
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