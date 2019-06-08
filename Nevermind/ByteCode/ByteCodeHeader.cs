using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nevermind.Compiler.Formats.Constants;

namespace Nevermind.ByteCode
{
    internal class NumeratedType
    {
        public int Index;
        public Type Type;

        public NumeratedType(int index, Type type)
        {
            Index = index;
            Type = type;
        }
    }

    internal class NumeratedConstant
    {
        public int Index;
        public Constant Constant;

        public NumeratedConstant(int index, Constant constant)
        {
            Index = index;
            Constant = constant;
        }
    }

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
            //todo: unique constants and types
            UsedTypes = program.UsedTypes.Select(p => new NumeratedType(typeIndex++, p)).ToList();
            UsedConstants = program.Constants.Select(p => new NumeratedConstant(constIndex++, p)).ToList();
            UsedTypes.AddRange(UsedConstants.Select(p => p.Constant.ToProgramType()).Select(p => new NumeratedType(typeIndex++, p)));
        }

        public int GetTypeIndex(Type t)
        {
            return UsedTypes.FindIndex(p => p.Type == t);
        }

        public int GetConstIndex(Constant c)
        {
            return UsedConstants.FindIndex(p => p.Constant == c);
        }

        public string ToSource()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(".type_count: {0}\n", UsedTypes.Count);
            sb.AppendLine("//Index : type : [base]");
            foreach (var type in UsedTypes)
                sb.AppendFormat("{0}:{1}:{2}\n", type.Index, type.Type.ID, type.Type.GetBase());

            sb.AppendFormat("\n.const_count: {0}\n", UsedTypes.Count);
            sb.AppendLine("//index : type index : [len] : value");
            foreach (var constant in UsedConstants)
            {
                var t = constant.Constant.ToProgramType();
                if (constant.Constant.Type == ConstantType.String)
                {
                    sb.AppendFormat("{0}:{1}:{2}:{3}\n", constant.Index, GetTypeIndex(t),
                        constant.Constant.SValue.Count, constant.Constant.ToStringValue());
                }
                else
                {
                    sb.AppendFormat("{0}:{1}:{2}\n", constant.Index, GetTypeIndex(t),
                        constant.Constant.ToStringValue());
                }
            }

            return sb.ToString();
        }

        public byte[] ToBinary()
        {
            return null;
        }
    }
}