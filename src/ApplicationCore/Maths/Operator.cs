using System;
using System.Collections.Generic;
using System.Numerics;
using ApplicationCore.Exceptions;
using ApplicationCore.Extensions;

namespace ApplicationCore.Maths
{
    public class Operator
    {
        private static readonly Operator[][] FullList =
        {
            new[]
            {
                new Operator("PI", Process(new Complex(Math.PI, 0)), isConstant: true),
                new Operator("I", Process(Complex.ImaginaryOne), isConstant: true),
                new Operator("E", Process(new Complex(Math.E, 0)), isConstant: true),
            },
            new[]
            {
                new Operator("_", Process(value => value.Negated())), 
            },
            new[]
            {
                new Operator("^", Process((b, e) => new PowerElement(b, e))), 
            },
            new[]
            {
                new Operator("*", Process((a, b) => new ProductElement(a, b))), 
                new Operator("/", Process((num, den) => new FractionElement(num, den))), 
            },
            new[]
            {
                new Operator("+", Process((a, b) => new SumElement(a, b))), 
                new Operator("-", Process((a, b) => new SumElement(a, b.Negated()))), 
            },
            new[]
            {
                new Operator("(", null, isLeftBracket: true), 
                new Operator(")", null, isRightBracket: true), 
                new Operator("SIN", Process(arg => new SinElement(arg)), isFunction: true), 
                new Operator("COS", Process(arg => new CosElement(arg)), isFunction: true), 
                new Operator("TAN", Process(arg => new TanElement(arg)), isFunction: true), 
                new Operator("LOG", Process(arg => new LogElement(arg)), isFunction: true), 
                new Operator("SQRT", Process(arg => new SqrtElement(arg)), isFunction: true), 
                new Operator("EXP", Process(arg => new ExpElement(arg)), isFunction: true), 
                new Operator("ASIN", Process(arg => new AsinElement(arg)), isFunction: true), 
                new Operator("ACOS", Process(arg => new AcosElement(arg)), isFunction: true), 
                new Operator("ATAN", Process(arg => new AtanElement(arg)), isFunction: true), 
                new Operator("SINH", Process(arg => new SinhElement(arg)), isFunction: true), 
                new Operator("COSH", Process(arg => new CoshElement(arg)), isFunction: true), 
                new Operator("TANH", Process(arg => new TanhElement(arg)), isFunction: true), 
            },
        };

        public static readonly IReadOnlyDictionary<string, Operator> All = ProcessList();
        
        private Operator(
            string token, 
            Action<Stack<MathElement>> processor, 
            bool isLeftAssociative = true, 
            bool isLeftBracket = false, 
            bool isRightBracket = false, 
            bool isFunction = false, 
            bool isConstant = false)
        {
            Token = token;
            IsLeftAssociative = isLeftAssociative;
            IsLeftBracket = isLeftBracket;
            IsRightBracket = isRightBracket;
            IsFunction = isFunction;
            IsConstant = isConstant;
            Processor = processor;
        }
        
        public string Token { get; }

        public int Precedence { get; private set; }
        
        public bool IsLeftAssociative { get; }
        
        public bool IsLeftBracket { get; }
        
        public bool IsRightBracket { get; }
        
        public bool IsFunction { get; }
        
        public bool IsConstant { get; }

        public Action<Stack<MathElement>> Processor { get; }
        
        private static IReadOnlyDictionary<string, Operator> ProcessList()
        {
            var result = new Dictionary<string, Operator>();
            foreach (var (list, index) in FullList.Enumerated())
            {
                foreach (var op in list)
                {
                    op.Precedence = index;
                    result[op.Token] = op;
                }
            }

            return result;
        }
        
        private static Action<Stack<MathElement>> Process(Complex number)
        {
            return stack =>
            {
                stack.Push(new ConstElement(number));
            };
        }

        private static Action<Stack<MathElement>> Process<T>(Func<MathElement, T> generator)
            where T : MathElement
        {
            return stack =>
            {
                if (stack.Count < 1)
                {
                    throw new ParseException($"Expected 1 item in stack but found {stack.Count}");
                }

                stack.Push(generator(stack.Pop()));
            };
        }

        private static Action<Stack<MathElement>> Process<T>(Func<MathElement, MathElement, T> generator)
            where T : MathElement
        {
            return stack =>
            {
                if (stack.Count < 2)
                {
                    throw new ParseException($"Expected 2 items in stack but found {stack.Count}");
                }

                var temp = stack.Pop();
                stack.Push(generator(stack.Pop(), temp));
            };
        }
    }
}