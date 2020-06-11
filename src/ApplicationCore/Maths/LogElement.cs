using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class LogElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Log));

        public LogElement(MathElement argument, bool isNegative = false)
        {
            Argument = argument;
            IsNegative = isNegative;
        }

        public MathElement Argument { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            var result = Expression.Call(Method, Argument.ToExpression(parameter));
            return NegateIfNeeded(result);
        }

        public override MathElement Clone() => new LogElement(Argument.Clone(), IsNegative);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}log({Argument.ToString(variableName)})";
    }
}
