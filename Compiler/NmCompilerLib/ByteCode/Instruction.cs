using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.NMB;

namespace Nevermind.ByteCode
{
    internal abstract class Instruction
    {
        public int Label;
        public readonly ByteCode ByteCode;
        public readonly Function Function;

        public Instruction(Function func, ByteCode byteCode, int label)
        {
            Label = label;
            ByteCode = byteCode;
            Function = func;
        }

        public abstract List<byte> Serialize();

        public abstract string InstructionName { get; }
        public abstract int ParameterCount { get; }
        public abstract InstructionType Type { get; }
        public abstract string SourceValue();
        public abstract bool UsesVariable(int index);
        public abstract List<Variable> FetchUsedVariables(int index);

        protected List<Variable> InnerFetch(int index, params Variable[] variables)
        {
            var result = new List<Variable>();
            if (variables == null) return result;

            if (index == -1)
                return variables.ToList();

            result.AddRange(variables.Where(variable => variable.Index == index));

            return result;
        }

        public string ToSource()
        {
            return $"   _{Function.Name + ':' + Label,-6}:  {SourceValue()}";
        }

        protected string ToSourceValue(params object[] objects)
        {
            return $"{InstructionName,-6} {string.Join(", ", objects)}";
        }

        protected List<byte> ToBytes(params IEnumerable<byte>[] lists)
        {
            var totalList = new List<byte>();
            foreach (var list in lists)
                totalList.AddRange(list);
            return totalList;
        }

        protected string ToFunctionLabel(int index)
        {
            return $"_{Function.Name}{index}";
        }

        public UInt16 GetInstructionCode()
        {
            if (Type == InstructionType._Binary)
                return Codes.ABInstructionDict[(this as BinaryArithmeticInstruction).AType];

            if (Type == InstructionType._Unary)
                return Codes.AUInstructionDict[(this as UnaryArithmeticInstruction).AType];

            return Codes.InstructionDict[Type];
        }
    }
}