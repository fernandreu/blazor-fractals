using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class SqrtElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Sqrt));

        public SqrtElement(MathElement argument, bool isNegative = false)
            : base(isNegative, argument.IsConstant)
        {
            Argument = argument;
        }

        public MathElement Argument { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            var result = Expression.Call(Method, Argument.ToExpression(parameter));
            return NegateIfNeeded(result);
        }

        public override MathElement Negated() => new SqrtElement(Argument, !IsNegative);

        public override MathElement Derive()
            => new ProductElement(
                new ConstElement(0.5, IsNegative), 
                new FractionElement(Argument.Derive(), this));

        protected override MathElement SimplifyInternal()
            => new SqrtElement(Argument.Simplify(), IsNegative);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}sqrt({Argument.ToString(variableName)})";
    }
}
