using Nevermind.ByteCode;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
using Nevermind.ByteCode.Instructions.ArithmeticIntsructions;
using Nevermind.ByteCode.InternalClasses;
using System.Collections.Generic;
using System.Linq;
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
        internal Token NearToken;

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
                    sb.AppendFormat(" {0}(", FunctionCall.StringValue);
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

        public static Variable GetVariable(List<NumeratedVariable> locals, ByteCode.ByteCode byteCode, Token token)
        {
            var operand = locals.Find(p => p.Variable.Name == token.StringValue)?.Variable;
            if (operand == null)
            {
                if (token.Constant != null)
                    operand = token.Constant.ToVariable(byteCode.Program);
                else
                    throw new ParseException(CompileErrorType.UndefinedReference, token);
            }

            return operand;
        }

        public static List<Instruction> GetInstructions(Function func, ByteCode.ByteCode byteCode, 
            ref int localVarIndex, List<ExpressionLineItem> list,
            out List<Variable> registers, List<NumeratedVariable> locals)
        {
            var instructions = new List<Instruction>();
            registers = new List<Variable>();

            foreach (var item in list)
            {
                OperatorResult result = null;
                Variable operand1 = item.RegOperand1 == -1 ? GetVariable(locals, byteCode, item.Operand1.CodeToken) : registers[item.RegOperand1];

                if (!item.IsUnary)
                {
                    Variable operand2 = item.RegOperand2 == -1 ? GetVariable(locals, byteCode, item.Operand2.CodeToken) : registers[item.RegOperand2];
                    result = item.Operator.BinaryFunc(new OperatorOperands(func, byteCode, -1, operand1, operand2, item));
                }
                else
                {
                    if (item.FunctionCall != null)
                    {
                        var funcToCall = func.Program.Program.Header.GetFunction(item.FunctionCall.StringValue);
                        if(funcToCall == null)
                            throw new ParseException(CompileErrorType.UndefinedReference, item.FunctionCall);

                        if(operand1.VariableType != VariableType.Tuple)
                        {
                            if(funcToCall.Parameters.Count != 1)
                                throw new ParseException(CompileErrorType.WrongParameterCount, item.FunctionCall);

                            if(!funcToCall.Parameters[0].Type.Equals(operand1.Type))
                                throw new ParseException(CompileErrorType.IncompatibleTypes, item.FunctionCall);

                            instructions.Add(new InstructionPush(operand1, func, byteCode, -1));
                            instructions.Add(new InstructionCall(funcToCall, func, byteCode, -1));
                        }
                        else
                        {

                        }

                        if (funcToCall.ReturnType != null)
                            registers.Add(new Variable(funcToCall.ReturnType, $"__reg{localVarIndex}", func.Scope, null, localVarIndex++, VariableType.Variable));

                        instructions.Add(new InstructionPop(registers.Last(), func, byteCode, -1));

                    }

                    if (item.UnaryOperator != null)
                        result = item.UnaryOperator.UnaryFunc(new OperatorOperands(func, byteCode, -1, operand1));
                }

                if (item.FunctionCall == null)
                {
                    if (result.Error != null)
                        throw new ParseException(result.Error.ErrorType, item.NearToken);

                    instructions.Add(result.Instruction);
                    var resultVar = new Variable(result.ResultType, $"__reg{localVarIndex}", func.Scope, null, localVarIndex++, VariableType.Variable);

                    (result.Instruction as ArithmeticIntsruction).Result = resultVar;
                    registers.Add(resultVar);
                }
                else
                {

                }
            }

            return instructions;
        }
    }
}