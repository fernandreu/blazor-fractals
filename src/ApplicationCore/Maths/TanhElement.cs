using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class TanhElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Tanh));

        public TanhElement(MathElement argument, bool isNegative = false)
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

        public override MathElement Negated() => new TanhElement(Argument, !IsNegative);
        
        public override MathElement Derive()
            => new ProductElement(
                IsNegative,
                new SumElement(
                    new ConstElement(1),
                    new PowerElement(this, new ConstElement(2), true)),
                Argument.Derive());

        protected override MathElement SimplifyInternal()
            => new TanhElement(Argument.Simplify(), IsNegative);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}tanh({Argument.ToString(variableName)})";
    }
}
