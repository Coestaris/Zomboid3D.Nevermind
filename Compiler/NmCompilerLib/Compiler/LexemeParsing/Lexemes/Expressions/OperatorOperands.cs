using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Types;
using Nevermind.ByteCode.Types.Scalar;

namespace Nevermind.Compiler.LexemeParsing.Lexemes.Expressions
{
    internal class OperatorOperands
    {
        public Function Function;
        public ByteCode.ByteCode ByteCode;
        public int Label;

        public Variable A;
        public Variable B;

        public ExpressionLineItem LineItem;

        public OperatorOperands(Function function, ByteCode.ByteCode byteCode, int label, Variable a, Variable b, ExpressionLineItem lineItem)
        {
            LineItem = lineItem;
            Function = function;
            ByteCode = byteCode;
            Label = label;
            A = a;
            B = b;
        }

        public OperatorOperands(Function function, ByteCode.ByteCode byteCode, int label, Variable a)
        {
            Function = function;
            ByteCode = byteCode;
            Label = label;
            A = a;
        }

        public CompileError CheckNumericAndGetType(out Type type)
        {
            type = null;
            if (CheckNumericTypes(A.Type, B.Type))
                return new CompileError(CompileErrorType.ExpectedNumericOperands, A.Token);

            type = ResolveNumericType();
            if (type == null)
                return new CompileError(CompileErrorType.WrongResultType, A.Token);
            return null;
        }
    }
}