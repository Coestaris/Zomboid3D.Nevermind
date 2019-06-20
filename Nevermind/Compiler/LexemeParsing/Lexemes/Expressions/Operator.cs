using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.Types;
using Type = Nevermind.ByteCode.Types.Type;

namespace Nevermind.Compiler.LexemeParsing.Lexemes.Expressions
{
    internal class Operator
    {
        public readonly int Priority;
        public readonly List<TokenType> OperatorTypes;
        public readonly bool IsUnary;
        public readonly Func<OperatorOperands, OperatorResult> UnaryFunc;
        public readonly Func<OperatorOperands, OperatorResult> BinaryFunc;
        
        public Operator(List<TokenType> operatorTypes, Func<OperatorOperands, OperatorResult> unaryFunc)
        {
            OperatorTypes = operatorTypes;
            Priority = -1;
            IsUnary = true;
            UnaryFunc = unaryFunc;
        }

        public Operator(List<TokenType> operatorTypes, int priority, Func<OperatorOperands, OperatorResult> internalFunction)
        {
            OperatorTypes = operatorTypes;
            Priority = priority;
            IsUnary = false;
            BinaryFunc = internalFunction;
        }

        public override string ToString()
        {
            return string.Join("", OperatorTypes.Select(p => p.ToSource()));
        }
        
        private static OperatorResult OperatorFunc(OperatorOperands operands, BinaryArithmeticInstructionType operatorType)
        {
            CompileError error = null;
            Type type;
            if ((error = operands.CheckNumericAndGetType(out type)) != null)
                return new OperatorResult(error);
            return new OperatorResult(new BinaryArithmeticInstruction(operatorType,
                null, operands.A, operands.B, operands.Function, operands.ByteCode, operands.Label), type);
        }

        private static OperatorResult SetFunc(OperatorOperands operands, BinaryArithmeticInstructionType operatorType)
        {
            if(!operands.A.Type.Equals(operands.B.Type))
                return new OperatorResult(new CompileError(CompileErrorType.IncompatibleTypes, operands.A.Token));
            
            if(operands.LineItem.Operand1 == null)
                return new OperatorResult(new CompileError(CompileErrorType.WrongAssignmentOperation, operands.A.Token));

            if (operands.A.VariableType != VariableType.Variable)
                return new OperatorResult(new CompileError(CompileErrorType.WrongAssignmentOperation, operands.A.Token));

            return new OperatorResult(new BinaryArithmeticInstruction(operatorType,
                null, operands.A, operands.B, operands.Function, operands.ByteCode, operands.Label), operands.A.Type);
        }

        private static OperatorResult UnaryOperatorFunc(OperatorOperands operands, UnaryArithmeticInstructionType operatorType)
        {
            if (operands.A.Type.ID != TypeID.Float && operands.A.Type.ID != TypeID.Integer)
                return new OperatorResult(new CompileError(CompileErrorType.ExpectedNumericOperands, operands.A.Token));

            return new OperatorResult(new UnaryArithmeticInstruction(operatorType, null, operands.A, operands.Function, operands.ByteCode, operands.Label), operands.A.Type);
        }

        public static readonly List<Operator> Operators = new List<Operator>
        {
            //Unary
            new Operator(new List<TokenType> { TokenType.ExclamationMark                          },  (a) => UnaryOperatorFunc(a, UnaryArithmeticInstructionType.A_Not)),
            new Operator(new List<TokenType> { TokenType.Tilda                                    },  (a) => UnaryOperatorFunc(a, UnaryArithmeticInstructionType.A_BNeg)),
            new Operator(new List<TokenType> { TokenType.MinusSign                                },  (a) => UnaryOperatorFunc(a, UnaryArithmeticInstructionType.A_Neg)),

            //Arithmetical
            new Operator(new List<TokenType> { TokenType.MultiplySign                             }, 13, (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_Mul)),

            new Operator(new List<TokenType> { TokenType.DivideSign                               }, 13, (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_Div)),
            new Operator(new List<TokenType> { TokenType.PercentSign                              }, 12, (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_EDiv)),
            new Operator(new List<TokenType> { TokenType.PlusSign                                 }, 11, (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.MinusSign                                }, 11, (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_Sub)),
            
            //Logical comp
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.LessThanSign  }, 10, (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_lsh)),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.GreaterSign   }, 10, (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_rlh)),
            new Operator(new List<TokenType> { TokenType.LessThanSign                             }, 9,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_lseq)),
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.EqualSign     }, 9,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_ls)),
            new Operator(new List<TokenType> { TokenType.GreaterSign                              }, 9,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_gr)),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.EqualSign     }, 9,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_greq)),
            new Operator(new List<TokenType> { TokenType.ExclamationMark, TokenType.EqualSign     }, 8,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_neq)),
            new Operator(new List<TokenType> { TokenType.EqualSign,       TokenType.EqualSign     }, 8,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_eq)),
            
            //Binary operators
            new Operator(new List<TokenType> { TokenType.AmpersandSign                            }, 7,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_And)),
            new Operator(new List<TokenType> { TokenType.CircumflexSign                           }, 6,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_Xor)),
            new Operator(new List<TokenType> { TokenType.OrSign                                   }, 5,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_Or)),
            
            //Logical
            new Operator(new List<TokenType> { TokenType.AmpersandSign,   TokenType.AmpersandSign }, 4,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_LAnd)),
            new Operator(new List<TokenType> { TokenType.OrSign,          TokenType.OrSign        }, 3,  (a) => OperatorFunc(a, BinaryArithmeticInstructionType.A_LOr)),

            //ternary?
            //new Operator(new List<TokenType> { TokenType.QuestingSign                             }, 2,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            //new Operator(new List<TokenType> { TokenType.Colon                                    }, 1,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            
            //Equal operators
            new Operator(new List<TokenType> { TokenType.PlusSign,        TokenType.EqualSign     }, 1,  (a) => SetFunc(a, BinaryArithmeticInstructionType.A_SetAdd)),
            new Operator(new List<TokenType> { TokenType.MinusSign,       TokenType.EqualSign     }, 1,  (a) => SetFunc(a, BinaryArithmeticInstructionType.A_SetSub)),
            new Operator(new List<TokenType> { TokenType.MultiplySign,    TokenType.EqualSign     }, 1,  (a) => SetFunc(a, BinaryArithmeticInstructionType.A_SetMul)),
            new Operator(new List<TokenType> { TokenType.DivideSign,      TokenType.EqualSign     }, 1,  (a) => SetFunc(a, BinaryArithmeticInstructionType.A_SetDiv)),
            new Operator(new List<TokenType> { TokenType.PercentSign,     TokenType.EqualSign     }, 1,  (a) => SetFunc(a, BinaryArithmeticInstructionType.A_SetEDiv)),
            new Operator(new List<TokenType> { TokenType.AmpersandSign,   TokenType.EqualSign     }, 1,  (a) => SetFunc(a, BinaryArithmeticInstructionType.A_SetAnd)),
            new Operator(new List<TokenType> { TokenType.CircumflexSign,  TokenType.EqualSign     }, 1,  (a) => SetFunc(a, BinaryArithmeticInstructionType.A_SetXor)),
            new Operator(new List<TokenType> { TokenType.OrSign,          TokenType.EqualSign     }, 1,  (a) => SetFunc(a, BinaryArithmeticInstructionType.A_SetOr)),
            new Operator(new List<TokenType> { TokenType.EqualSign                                }, 1,  (a) => SetFunc(a, BinaryArithmeticInstructionType.A_Set)),

            new Operator(new List<TokenType> { TokenType.ComaSign                                 }, 0,  (a) =>
            {
                if(!a.LineItem.ParentHasFunction)
                    return new OperatorResult(new CompileError(CompileErrorType.UnexpectedCommaOperator, a.A.Token));

                var result = new List<Variable>();

                if(a.A.VariableType == VariableType.Tuple)
                    result.AddRange(a.A.Tuple);
                else
                    result.Add(a.A);
                result.Add(a.B);

               return new OperatorResult(result);
            }),
        };
    }
}