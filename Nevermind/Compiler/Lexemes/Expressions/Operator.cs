using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode;

namespace Nevermind.Compiler.Lexemes.Expressions
{
    internal class Operator
    {
        public readonly int Priority;
        public readonly List<TokenType> OperatorTypes;
        public readonly bool IsUnary;
        public readonly Function<IntegerType, IntegerType> UnaryFunc;
        public readonly Function<IntegerType, IntegerType, IntegerType> BinaryFunc;

        public Operator(List<TokenType> operatorTypes, Function<IntegerType, IntegerType> unaryFunc)
        {
            OperatorTypes = operatorTypes;
            Priority = -1;
            IsUnary = true;
            UnaryFunc = unaryFunc;
        }

        public Operator(List<TokenType> operatorTypes, int priority, Function<IntegerType, IntegerType, IntegerType> function)
        {
            OperatorTypes = operatorTypes;
            Priority = priority;
            IsUnary = false;
            BinaryFunc = function;
        }

        public override string ToString()
        {
            return string.Join("", OperatorTypes.Select(p => p.ToSource()));
        }

        public static readonly List<Operator> Operators = new List<Operator>
        {
            //Unary
            new Operator(new List<TokenType> { TokenType.ExclamationMark                          }, new Function<IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.Tilda                                    }, new Function<IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.MinusSign                                }, new Function<IntegerType, IntegerType>()),

            //Binary
            new Operator(new List<TokenType> { TokenType.MultiplySign                             }, 13, new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.DivideSign                               }, 13, new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.PercentSign                              }, 12, new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.PlusSign                                 }, 11, new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.MinusSign                                }, 11, new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.LessThanSign  }, 10, new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.GreaterSign   }, 10, new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.LessThanSign                             }, 9,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.LessThanSign,    TokenType.EqualSign     }, 9,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.GreaterSign                              }, 9,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.GreaterSign,     TokenType.EqualSign     }, 9,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.ExclamationMark, TokenType.EqualSign     }, 8,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.EqualSign,       TokenType.EqualSign     }, 8,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.AmpersandSign                            }, 7,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.CircumflexSign                           }, 6,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.OrSign                                   }, 5,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.AmpersandSign,   TokenType.AmpersandSign }, 4,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.OrSign,          TokenType.OrSign        }, 3,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.QuestingSign                             }, 2,  new Function<IntegerType, IntegerType, IntegerType>()),
            new Operator(new List<TokenType> { TokenType.Colon                                    }, 1,  new Function<IntegerType, IntegerType, IntegerType>()),
        };
    }
}