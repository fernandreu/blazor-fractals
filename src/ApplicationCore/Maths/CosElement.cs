using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class CosElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Cos));

        public CosElement(MathElement argument, bool isNegative = false)
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

        public override MathElement Clone() => new CosElement(Argument.Clone(), IsNegative);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}cos({Argument.ToString(variableName)})";
    }
}
