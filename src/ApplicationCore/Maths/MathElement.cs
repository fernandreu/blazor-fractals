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

        public static MathElement Parse(string expression, string varName = "z")
        {
            expression = expression.ToUpperInvariant();
            varName = varName.ToUpperInvariant();
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

                if (Operator.All.TryGetValue(part, out var op))
                {
                    if (op.IsLeftBracket || op.IsRightBracket)
                    {
                        // This should not be here. If it is, it must be an incorrect expression
                        throw new ParseException("Mismatched open / close brackets");
                    }
                    
                    op.Processor(stack);
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
