using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
using Nevermind.ByteCode.Instructions.ArithmeticIntsructions;

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
        
        private static OperatorResult OperatorFunc(OperatorOperands a, BinaryArithmeticIntsructionType operatorType)
        {
            CompileError error = null;
            if ((error = a.CheckNumericAndGetType(out var type)) != null)
                return new OperatorResult(error);
            return new OperatorResult(new BinaryArithmeticIntsruction(operatorType,
                null, a.A, a.B, a.Function, a.ByteCode, a.Label), type);
        }

        private static OperatorResult UnaryOperatorFunc(OperatorOperands a, UnaryArithmeticIntsructionType operatorType)
        {
            if (a.A.Type.ID != ByteCode.TypeID.Float && a.A.Type.ID != ByteCode.TypeID.Integer)
                return new OperatorResult(new CompileError(CompileErrorType.ExpectedNumericOperands, a.A.Token));

            return new OperatorResult(new UnaryArithmeticIntsruction(operatorType, null, a.A, a.Function, a.ByteCode, a.Label), a.A.Type);
        }

        public static readonly List<Operator> Operators = new List<Operator>
        {
            //Unary
            new Operator(new List<TokenType> { TokenType.ExclamationMark                          },  (a) => UnaryOperatorFunc(a, UnaryArithmeticIntsructionType.A_Not)),
            new Operator(new List<TokenType> { TokenType.Tilda                                    },  (a) => UnaryOperatorFunc(a, UnaryArithmeticIntsructionType.A_BNeg)),
            new Operator(new List<TokenType> { TokenType.MinusSign                                },  (a) => UnaryOperatorFunc(a, UnaryArithmeticIntsructionType.A_Neg)),

            //Arithmetical
            new Operator(new List<TokenType> { TokenType.MultiplySign                             }, 13, (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Mul)),

            new Operator(new List<TokenType> { TokenType.DivideSign                               }, 13, (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Div)),
            new Operator(new List<TokenType> { TokenType.PercentSign                              }, 12, (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_EDiv)),
            new Operator(new List<TokenType> { TokenType.PlusSign                                 }, 11, (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.MinusSign                                }, 11, (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Sub)),
            
            //Logical comp
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.LessThanSign  }, 10, (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.GreaterSign   }, 10, (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.LessThanSign                             }, 9,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_lseq)),
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.EqualSign     }, 9,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_ls)),
            new Operator(new List<TokenType> { TokenType.GreaterSign                              }, 9,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_gr)),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.EqualSign     }, 9,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_greq)),
            new Operator(new List<TokenType> { TokenType.ExclamationMark, TokenType.EqualSign     }, 8,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_neq)),
            new Operator(new List<TokenType> { TokenType.EqualSign,       TokenType.EqualSign     }, 8,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_eq)),
            
            //Binary operators
            new Operator(new List<TokenType> { TokenType.AmpersandSign                            }, 7,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.CircumflexSign                           }, 6,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.OrSign                                   }, 5,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            
            //Logical
            new Operator(new List<TokenType> { TokenType.AmpersandSign,   TokenType.AmpersandSign }, 4,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.OrSign,          TokenType.OrSign        }, 3,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.QuestingSign                             }, 2,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.Colon                                    }, 1,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            
            //Equal operators
            new Operator(new List<TokenType> { TokenType.PlusSign,        TokenType.EqualSign     }, 1,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.MinusSign,       TokenType.EqualSign     }, 1,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.MultiplySign,    TokenType.EqualSign     }, 1,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.DivideSign,      TokenType.EqualSign     }, 1,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.AmpersandSign,   TokenType.EqualSign     }, 1,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.CircumflexSign,  TokenType.EqualSign     }, 1,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.OrSign,          TokenType.EqualSign     }, 1,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
            new Operator(new List<TokenType> { TokenType.EqualSign                                }, 1,  (a) =>
            {
                 return new OperatorResult(new InstructionLdi(a.B, a.A, a.Function, a.ByteCode, a.Label), a.A.Type);
            }),
            new Operator(new List<TokenType> { TokenType.ComaSign                                 }, 0,  (a) => OperatorFunc(a, BinaryArithmeticIntsructionType.A_Add)),
        };
    }
}