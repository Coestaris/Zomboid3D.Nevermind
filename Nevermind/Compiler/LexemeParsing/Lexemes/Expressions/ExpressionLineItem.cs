using Nevermind.ByteCode;
using Nevermind.ByteCode.Functions;
using System.Collections.Generic;
using System.Text;

namespace Nevermind.Compiler.LexemeParsing.Lexemes.Expressions
{
    internal class ExpressionLineItem
    {
        public Operator Operator;
        public ExpressionToken Operand1;
        public ExpressionToken Operand2;

        public int RegOperand1 = -1;
        public int RegOperand2 = -1;

        public int ResultRegisterIndex;

        private ExpressionLineItem(Operator @operator, int resultRegisterIndex)
        {
            Operator = @operator;
            ResultRegisterIndex = resultRegisterIndex;
        }

        public ExpressionLineItem(Operator @operator, ExpressionToken operand1, ExpressionToken operand2, int resultRegisterIndex)
            : this(@operator, resultRegisterIndex)
        {
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public ExpressionLineItem(Operator @operator, int operand1, int operand2, int resultRegisterIndex)
            : this(@operator, resultRegisterIndex)

        {
            RegOperand1 = operand1;
            RegOperand2 = operand2;
        }

        public ExpressionLineItem(Operator @operator, ExpressionToken operand1, int operand2, int resultRegisterIndex)
            : this(@operator, resultRegisterIndex)
        {
            Operand1 = operand1;
            RegOperand2 = operand2;
        }

        public ExpressionLineItem(Operator @operator, int operand1, ExpressionToken operand2, int resultRegisterIndex)
            : this(@operator, resultRegisterIndex)
        {
            RegOperand1 = operand1;
            Operand2 = operand2;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("%{0} <- ", ResultRegisterIndex);
            if (RegOperand1 != -1) sb.AppendFormat("%{0}", RegOperand1);
            else sb.AppendFormat("{0}", Operand1.CodeToken.ToSource());

            sb.AppendFormat(" {0} ", Operator);

            if (RegOperand2 != -1) sb.AppendFormat("%{0}", RegOperand2);
            else sb.AppendFormat("{0}", Operand2.CodeToken.ToSource());

            return sb.ToString();
        }

        public static List<ExpressionLineItem> OptimizeList(List<ExpressionLineItem> list)
        {
            return list; //todo!;
        }

        public static int RequiredRegistersCount(List<ExpressionLineItem> list)
        {
            //todo!
            return list.Count;
        }

        public static List<Instruction> GetInstructions(Function func, ByteCode.ByteCode byteCode, ref int labelStart, List<ExpressionLineItem> list)
        {
            var instructions = new List<Instruction>();
            foreach(var item in list)
            {
                var result = item.Operator.
            }
        }
    }
}