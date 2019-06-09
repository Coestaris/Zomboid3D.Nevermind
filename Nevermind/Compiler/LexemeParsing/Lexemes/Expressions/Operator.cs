using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.Arithmetic;

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
        
        public static readonly List<Operator> Operators = new List<Operator>
        {
            //Unary
            new Operator(new List<TokenType> { TokenType.ExclamationMark                          }, (var) => null),
            new Operator(new List<TokenType> { TokenType.Tilda                                    }, (var) => null),
            new Operator(new List<TokenType> { TokenType.MinusSign                                }, (var) => null),

            //Arithmetical
            new Operator(new List<TokenType> { TokenType.MultiplySign                             }, 13,
                (a) =>
                {
                    CompileError error = null;
                    if((error = a.CheckNumericAndGetType(out var type)) != null)
                        return new OperatorResult(error);
                    return new OperatorResult(new InstructionMul(null, a.A, a.B, a.Function, a.ByteCode, a.Label), type);
                }),

            new Operator(new List<TokenType> { TokenType.DivideSign                               }, 13, (a) => null),
            new Operator(new List<TokenType> { TokenType.PercentSign                              }, 12, (a) => null),
            new Operator(new List<TokenType> { TokenType.PlusSign                                 }, 11, 
                (a) => 
                {
                    CompileError error = null;
                    if((error = a.CheckNumericAndGetType(out var type)) != null)
                        return new OperatorResult(error);
                    return new OperatorResult(new InstructionAdd(null, a.A, a.B, a.Function, a.ByteCode, a.Label), type);
                }),

            new Operator(new List<TokenType> { TokenType.MinusSign                                }, 11, (a) => null),

            //Logical comp
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.LessThanSign  }, 10, (a) => null),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.GreaterSign   }, 10, (a) => null),
            new Operator(new List<TokenType> { TokenType.LessThanSign                             }, 9,  (a) => null),
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.EqualSign     }, 9,  (a) => null),
            new Operator(new List<TokenType> { TokenType.GreaterSign                              }, 9,  (a) => null),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.EqualSign     }, 9,  (a) => null),
            new Operator(new List<TokenType> { TokenType.ExclamationMark, TokenType.EqualSign     }, 8,  (a) => null),
            new Operator(new List<TokenType> { TokenType.EqualSign,       TokenType.EqualSign     }, 8,  (a) => null),
   
            //Binary operators
            new Operator(new List<TokenType> { TokenType.AmpersandSign                            }, 7,  (a) => null),
            new Operator(new List<TokenType> { TokenType.CircumflexSign                           }, 6,  (a) => null),
            new Operator(new List<TokenType> { TokenType.OrSign                                   }, 5,  (a) => null),
            
            //Logical
            new Operator(new List<TokenType> { TokenType.AmpersandSign,   TokenType.AmpersandSign }, 4,  (a) => null),
            new Operator(new List<TokenType> { TokenType.OrSign,          TokenType.OrSign        }, 3,  (a) => null),
            new Operator(new List<TokenType> { TokenType.QuestingSign                             }, 2,  (a) => null),
            new Operator(new List<TokenType> { TokenType.Colon                                    }, 1,  (a) => null),
            
            //Equal operators
            new Operator(new List<TokenType> { TokenType.PlusSign,        TokenType.EqualSign     }, 1,  (a) => null),
            new Operator(new List<TokenType> { TokenType.MinusSign,       TokenType.EqualSign     }, 1,  (a) => null),
            new Operator(new List<TokenType> { TokenType.MultiplySign,    TokenType.EqualSign     }, 1,  (a) => null),
            new Operator(new List<TokenType> { TokenType.DivideSign,      TokenType.EqualSign     }, 1,  (a) => null),
            new Operator(new List<TokenType> { TokenType.AmpersandSign,   TokenType.EqualSign     }, 1,  (a) => null),
            new Operator(new List<TokenType> { TokenType.CircumflexSign,  TokenType.EqualSign     }, 1,  (a) => null),
            new Operator(new List<TokenType> { TokenType.OrSign,          TokenType.EqualSign     }, 1,  (a) => null),
            new Operator(new List<TokenType> { TokenType.EqualSign                                }, 1,  (a) => null),
            new Operator(new List<TokenType> { TokenType.EqualSign                                }, 1,  (a) => null),
            new Operator(new List<TokenType> { TokenType.ComaSign                                 }, 0,  (a) => null),
        };
    }
}