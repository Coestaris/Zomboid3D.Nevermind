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
            if (!CheckNumericTypes(A.Type, B.Type))
                return new CompileError(CompileErrorType.ExpectedNumericOperands, A.Token);

            type = ResolveNumericType();
            if (type == null)
                return new CompileError(CompileErrorType.WrongResultType, A.Token);

            return null;
        }

        public Type ResolveNumericType()
        {
            if (A.Type.ID == TypeID.Float && B.Type.ID == TypeID.Float)
                return ResolveFloatTypes((FloatType)A.Type, (FloatType)B.Type);
            else if (A.Type.ID == TypeID.Integer && B.Type.ID == TypeID.Integer)
                return ResolveIntegerTypes((IntegerType)A.Type, (IntegerType)B.Type);
            else
            {
                if (A.Type.ID == TypeID.Float)
                    return ResolveMixedTypes((IntegerType)B.Type, (FloatType)A.Type);
                else
                    return ResolveMixedTypes((IntegerType)A.Type, (FloatType)B.Type);
            }
        }

        private static IntegerType ResolveIntegerTypes(IntegerType a, IntegerType b)
        {
            bool resultSigned = a.Signed || b.Signed;
            int typeBase = a.TypeBase > b.TypeBase ? a.TypeBase : b.TypeBase;
            return BuiltInTypes.GetIntegerType(typeBase, resultSigned);
        }

        private static FloatType ResolveFloatTypes(FloatType a, FloatType b)
        {
            int typeBase = a.TypeBase > b.TypeBase ? a.TypeBase : b.TypeBase;
            return BuiltInTypes.GetFloatType(typeBase);
        }

        private static FloatType ResolveMixedTypes(IntegerType a, FloatType b)
        {
            int typeBase = a.TypeBase > b.TypeBase ? a.TypeBase : b.TypeBase;
            return BuiltInTypes.GetFloatType(typeBase);
        }

        public static bool CheckNumericTypes(Type a, Type b)
        {
            return (a.ID == TypeID.Integer || a.ID == TypeID.Float) &&
                   (b.ID == TypeID.Integer || b.ID == TypeID.Float);
        }
    }
}