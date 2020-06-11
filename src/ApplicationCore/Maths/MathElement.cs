using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using ApplicationCore.Exceptions;

namespace ApplicationCore.Maths
{
    public abstract class MathElement
    {
        protected MathElement(bool isNegative, bool isConstant)
        {
            IsNegative = isNegative;
            IsConstant = isConstant;
        }
        
        /// <summary>
        /// Gets whether the entire expression has a unary minus
        /// </summary>
        public bool IsNegative { get; }

        /// <summary>
        /// Gets whether the entire expression depends on a variable or not
        /// </summary>
        public bool IsConstant { get; }

        protected internal abstract Expression ToExpression(ParameterExpression parameter);

        public Func<Complex, Complex> ToFunc() => ToExpression().Compile();

        public abstract MathElement Negated();
        
        public abstract MathElement Derive();

        public MathElement Derive(int order)
        {
            if (order < 0)
            {
                return null;
            }

            var result = this;
            for (var i = 0; i < order; ++i)
            {
                result = result.Derive();
            }

            return result;
        }

        public MathElement Simplify() => IsConstant ? new ConstElement(Evaluate(Complex.Zero)) : SimplifyInternal();

        protected virtual MathElement SimplifyInternal() => this;
        
        /// <summary>
        /// Evaluates this element at the specific point.
        ///
        /// Note: this can be resource-intensive. If used repeatedly, cache the function first using ToFunc().
        /// </summary>
        /// <param name="point">The point to evaluate the element at</param>
        /// <returns></returns>
        public Complex Evaluate(Complex point) => ToFunc()(point);
        
        public Expression<Func<Complex, Complex>> ToExpression()
        {
            var parameter = Expression.Parameter(typeof(Complex), "z");
            var body = ToExpression(parameter);
            return Expression.Lambda<Func<Complex, Complex>>(body, parameter);
        }

        /// <summary>
        /// Generates a function ready to perform the Newton's method, i.e. z - m * f(z) / f'(z)
        /// </summary>
        /// <param name="multiplicity">The multiplicity (m)</param>
        /// <returns>The function generated</returns>
        public MathElement ToNewtonFunction(Complex multiplicity)
        {
            return new SumElement(
                new VariableElement(),
                new ProductElement(
                    new ConstElement(-multiplicity),
                    new FractionElement(this, Derive()))
                );
        }
        
        public override string ToString() => ToString("z");
        
        public abstract string ToString(string variableName);

        private static readonly IDictionary<string, Action<Stack<MathElement>>> Cache = new Dictionary<string, Action<Stack<MathElement>>>
        {
            ["^"] = Process((b, e) => new PowerElement(b, e)),
            ["*"] = Process((a, b) => new ProductElement(a, b)),
            ["/"] = Process((num, den) => new FractionElement(num, den)),
            ["+"] = Process((a, b) => new SumElement(a, b)),
            ["-"] = Process((a, b) => new SumElement(a, b.Negated())),
            ["_"] = Process(value => value.Negated()),
            ["sin"] = Process(arg => new SinElement(arg)),
            ["cos"] = Process(arg => new CosElement(arg)),
            ["tan"] = Process(arg => new TanElement(arg)),
            ["log"] = Process(arg => new LogElement(arg)),
            ["pi"] = Process(new Complex(Math.PI, 0)),
            ["i"] = Process(Complex.ImaginaryOne),
            ["e"] = Process(new Complex(Math.E, 0)),
        };

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

        public static MathElement Parse(string expression, string varName = "z")
        {
            var parts = MathUtils.ToReversePolishNotation(expression, varName);
            if (parts.Count == 0)
            {
                throw new ParseException("Nothing to parse");
            }

            var stack = new Stack<MathElement>();
            foreach (var part in parts)
            {
                if (part == varName)
                {
                    stack.Push(new VariableElement());
                    continue;
                }

                if (Cache.TryGetValue(part, out var processor))
                {
                    processor(stack);
                    continue;
                }

                if (!double.TryParse(part, out var d))
                {
                    throw new ParseException($"Unrecognized token: {part}");
                }

                stack.Push(new ConstElement(d));
            }

            if (stack.Count != 1)
            {
                // This would mean the expression was ill-formed
                throw new ParseException($"Expecting 1 item in stack at end of the parsing process but got {stack.Count} instead");
            }

            return stack.Pop();
        }

        protected Expression NegateIfNeeded(Expression result)
        {
            return !IsNegative ? result : Expression.Negate(result);
        }
    }
}
