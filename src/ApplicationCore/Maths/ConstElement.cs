using System;
using System.Linq.Expressions;
using System.Numerics;

namespace ApplicationCore.Maths
{
    public class ConstElement : MathElement
    {
        public ConstElement(Complex value)
        {
            Value = value;
        }

        public Complex Value { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
            => NegateIfNeeded(Expression.Constant(Value));

        public override string ToString(string variableName)
        {
            var value = IsNegative ? -Value : Value;
            if (value.Imaginary == 0)
            {
                return value.Real.ToString();
            }

            if (value.Real == 0)
            {
                return $"{value.Imaginary}i";
            }

            return $"{value.Real}{(value.Imaginary > 0 ? "+" : "")}{value.Imaginary}i";
        }
    }
}
