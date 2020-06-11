using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;

namespace ApplicationCore.Maths
{
    public class ConstElement : MathElement
    {
        public ConstElement(Complex value, bool isNegative = false)
        {
            Value = value;
            IsNegative = isNegative;
        }

        public Complex Value { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
            => NegateIfNeeded(Expression.Constant(Value));

        public override MathElement Clone() => new ConstElement(Value, IsNegative);

        public override string ToString(string variableName)
        {
            var value = IsNegative ? -Value : Value;
            var real = value.Real.ToString(CultureInfo.CurrentCulture);
            var imag = value.Imaginary.ToString(CultureInfo.CurrentCulture);
            
            if (imag == "0")
            {
                return real;
            }

            if (real == "0")
            {
                return $"{imag}i";
            }

            return $"{real}{(value.Imaginary > 0 ? "+" : "")}{imag}i";
        }
    }
}
