using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class AsinElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Asin));

        public AsinElement(MathElement argument, bool isNegative = false)
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

        public override MathElement Negated() => new AsinElement(Argument, !IsNegative);
        
        public override MathElement Derive()
            => new FractionElement(
                Argument.Derive(),
                new SqrtElement(
                    new SumElement(
                        new ConstElement(1),
                        new PowerElement(this, new ConstElement(2), true))),
                IsNegative);

        protected override MathElement SimplifyInternal()
            => new AsinElement(Argument.Simplify(), IsNegative);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}asin({Argument.ToString(variableName)})";
    }
}
