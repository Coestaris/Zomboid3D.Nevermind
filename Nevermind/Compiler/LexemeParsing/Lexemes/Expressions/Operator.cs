using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode;
using Nevermind.ByteCode.Functions;

namespace Nevermind.Compiler.LexemeParsing.Lexemes.Expressions
{
    internal class Operator
    {
        public readonly int Priority;
        public readonly List<TokenType> OperatorTypes;
        public readonly bool IsUnary;
        public readonly InternalFunction<IntegerType, IntegerType> UnaryFunc;
        public readonly InternalFunction<IntegerType, IntegerType, IntegerType> BinaryFunc;

        public Operator(List<TokenType> operatorTypes, InternalFunction<IntegerType, IntegerType> unaryFunc)
        {
            OperatorTypes = operatorTypes;
            Priority = -1;
            IsUnary = true;
            UnaryFunc = unaryFunc;
        }

        public Operator(List<TokenType> operatorTypes, int priority, InternalFunction<IntegerType, IntegerType, IntegerType> internalFunction)
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
            new Operator(new List<TokenType> { TokenType.ExclamationMark                          }, new InternalFunction<IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.Tilda                                    }, new InternalFunction<IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.MinusSign                                }, new InternalFunction<IntegerType, IntegerType>()),

            //Arithmetical
            new Operator(new List<TokenType> { TokenType.MultiplySign                             }, 13, new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.DivideSign                               }, 13, new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.PercentSign                              }, 12, new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.PlusSign                                 }, 11, new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.MinusSign                                }, 11, new InternalFunction<IntegerType, IntegerType, IntegerType>()),

            //Logical comp
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.LessThanSign  }, 10, new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.GreaterSign   }, 10, new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.LessThanSign                             }, 9,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.EqualSign     }, 9,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.GreaterSign                              }, 9,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.EqualSign     }, 9,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.ExclamationMark, TokenType.EqualSign     }, 8,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.EqualSign,       TokenType.EqualSign     }, 8,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),

            //Binary operators
            new Operator(new List<TokenType> { TokenType.AmpersandSign                            }, 7,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.CircumflexSign                           }, 6,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.OrSign                                   }, 5,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),

            //Logical
            new Operator(new List<TokenType> { TokenType.AmpersandSign,   TokenType.AmpersandSign }, 4,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.OrSign,          TokenType.OrSign        }, 3,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.QuestingSign                             }, 2,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.Colon                                    }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),

            //Equal operators
            new Operator(new List<TokenType> { TokenType.PlusSign,        TokenType.EqualSign     }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.MinusSign,       TokenType.EqualSign     }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.MultiplySign,    TokenType.EqualSign     }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.DivideSign,      TokenType.EqualSign     }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.AmpersandSign,   TokenType.EqualSign     }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.CircumflexSign,  TokenType.EqualSign     }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.OrSign,          TokenType.EqualSign     }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.EqualSign                                }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.EqualSign                                }, 1,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.ComaSign                                 }, 0,  new InternalFunction<IntegerType, IntegerType, IntegerType>()),
        };
    }
}