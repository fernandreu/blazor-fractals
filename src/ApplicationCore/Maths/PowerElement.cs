using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class PowerElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Pow), new[] { typeof(Complex), typeof(Complex) });

        public PowerElement(MathElement @base, MathElement exponent)
        {
            Base = @base;
            Exponent = exponent;
        }

        public MathElement Base { get; }

        public MathElement Exponent { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            var result = Expression.Call(Method, Base.ToExpression(parameter), Exponent.ToExpression(parameter));
            return NegateIfNeeded(result);
        }

        public override string ToString(string variableName)
        {
            return $"{(IsNegative ? "-" : "")}({Base.ToString(variableName)})^({Exponent.ToString(variableName)})";
        }
    }
}
