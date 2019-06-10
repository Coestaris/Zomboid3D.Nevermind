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

        public bool IsUnary;
        public Token FunctionCall;
        public Operator UnaryOperator;

        private ExpressionLineItem(Operator @operator, int resultRegisterIndex)
        {
            Operator = @operator;
            ResultRegisterIndex = resultRegisterIndex;
            IsUnary = false;
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

        public ExpressionLineItem(Operator unaryOperator, ExpressionToken operand, int resultRegisterIndex, Token functionCall)
        {
            Operand1 = operand;

            UnaryOperator = unaryOperator;
            ResultRegisterIndex = resultRegisterIndex;
            FunctionCall = functionCall;
            IsUnary = true;
        }

        public ExpressionLineItem(Operator unaryOperator, int operand, int resultRegisterIndex, Token functionCall)
        {
            RegOperand1 = operand;

            UnaryOperator = unaryOperator;
            ResultRegisterIndex = resultRegisterIndex;
            FunctionCall = functionCall;
            IsUnary = true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("%{0} <- ", ResultRegisterIndex);
            if (!IsUnary)
            {
                if (RegOperand1 != -1) sb.AppendFormat("%{0}", RegOperand1);
                else sb.AppendFormat("{0}", Operand1?.CodeToken?.ToSource() ?? "null");

                sb.AppendFormat(" {0} ", Operator);

                if (RegOperand2 != -1) sb.AppendFormat("%{0}", RegOperand2);
                else sb.AppendFormat("{0}", Operand2?.CodeToken?.ToSource() ?? "null");
            }
            else
            {
                if (FunctionCall != null)
                    sb.AppendFormat(" {0}(", FunctionCall);
                else
                    sb.AppendFormat(" {0} ", UnaryOperator);

                if (RegOperand1 != -1) sb.AppendFormat("%{0}", RegOperand1);
                else sb.AppendFormat("{0}", Operand1?.CodeToken?.ToSource() ?? "null");

                if (FunctionCall != null)
                    sb.Append(")");
            }
            return sb.ToString();
        }

        public static List<ExpressionLineItem> OptimizeList(List<ExpressionLineItem> list)
        {
            //todo!;
            return list; 
        }

        public static int RequiredRegistersCount(List<ExpressionLineItem> list)
        {
            //todo!
            return list.Count;
        }

        public static Variable GetVariable(Function func, ByteCode.ByteCode byteCode, Token token)
        {
            var operand = func.LocalVariables.Find(p => p.Name == token.StringValue);
            if (operand == null)
            {
                if (token.Constant != null)
                    operand = token.Constant.ToVariable(byteCode.Program);
                else
                    throw new ParseException(token, CompileErrorType.UndefinedReference);
            }

            return operand;
        }

        public static List<Instruction> GetInstructions(Function func, ByteCode.ByteCode byteCode, 
            ref int localVarIndex, List<ExpressionLineItem> list,
            out List<Variable> registers)
        {
            var instructions = new List<Instruction>();
            registers = new List<Variable>();

            foreach (var item in list)
            {
                OperatorResult result = null;
                Variable operand1 = item.RegOperand1 == -1 ? GetVariable(func, byteCode, item.Operand1.CodeToken) : registers[item.RegOperand1];

                if (!item.IsUnary)
                {
                    Variable operand2 = item.RegOperand2 == -1 ? GetVariable(func, byteCode, item.Operand2.CodeToken) : registers[item.RegOperand2];
                    result = item.Operator.BinaryFunc(new OperatorOperands(func, byteCode, -1, operand1, operand2));
                }
                else
                    result = item.UnaryOperator.UnaryFunc(new OperatorOperands(func, byteCode, -1, operand1));

                if (result.Error != null)
                    throw new ParseException(item.Operand1.CodeToken, result.Error.ErrorType);

                instructions.Add(result.Instruction);

                var resultVar = new Variable(result.ResultType, $"__reg{localVarIndex}", func.Scope, null, localVarIndex++);
                if (result.Instruction is BinaryArithmeticIntsruction)
                {
                    (result.Instruction as BinaryArithmeticIntsruction).Result = resultVar;
                }
                else
                {
                    (result.Instruction as UnaryArithmeticIntsruction).Result = resultVar;
                }

                registers.Add(resultVar);
            }

            return instructions;
        }
    }
}