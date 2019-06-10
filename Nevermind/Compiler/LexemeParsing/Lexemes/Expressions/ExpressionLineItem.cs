using Nevermind.ByteCode;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
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

        public static List<Instruction> GetInstructions(Function func, ByteCode.ByteCode byteCode, 
            ref int localVarIndex, List<ExpressionLineItem> list,
            out List<Variable> registers)
        {
            var instructions = new List<Instruction>();
            registers = new List<Variable>();

            foreach (var item in list)
            {
                Variable operand1 = item.RegOperand1 == -1 ? null : registers[item.RegOperand1];
                Variable operand2 = item.RegOperand2 == -1 ? null : registers[item.RegOperand2];

                if(operand1 == null)
                {
                    operand1 = func.LocalVariables.Find(p => p.Name == item.Operand1.CodeToken.StringValue);
                    if (operand1 == null)
                    {
                        if (item.Operand1.CodeToken.Constant != null)
                            operand1 = item.Operand1.CodeToken.Constant.ToVariable(byteCode.Program);
                        else
                            throw new ParseException(item.Operand1.CodeToken, CompileErrorType.UndefinedReference);
                    }
                }

                if (operand2 == null)
                {
                    operand2 = func.LocalVariables.Find(p => p.Name == item.Operand2.CodeToken.StringValue);
                    if (operand2 == null)
                    {
                        if (item.Operand2.CodeToken.Constant != null)
                            operand2 = item.Operand2.CodeToken.Constant.ToVariable(byteCode.Program);
                        else
                            throw new ParseException(item.Operand2.CodeToken, CompileErrorType.UndefinedReference);
                    }
                }

                var result = item.Operator.BinaryFunc(
                    new OperatorOperands(func, byteCode, -1, operand1, operand2));

                if (result.Error != null)
                    throw new ParseException(item.Operand1.CodeToken, result.Error.ErrorType);

                instructions.Add(result.Instruction);
                var resultVar = new Variable(result.ResultType, $"__reg{localVarIndex}", func.Scope, null, localVarIndex++);
                if (result.Instruction is ArithmeticIntsruction)
                {
                    (result.Instruction as ArithmeticIntsruction).Result = resultVar;
                }

                registers.Add(resultVar);
            }

            return instructions;
        }
    }
}