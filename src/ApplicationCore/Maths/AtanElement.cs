using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class AtanElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Atan));

        public AtanElement(MathElement argument, bool isNegative = false)
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

        public override MathElement Negated() => new AcosElement(Argument, !IsNegative);
        
        public override MathElement Derive()
            => new FractionElement(
                Argument.Derive(),
                new SumElement(
                    new ConstElement(1),
                    new PowerElement(this, new ConstElement(2))),
                IsNegative);

        protected override MathElement SimplifyInternal()
            => new AtanElement(Argument.Simplify(), IsNegative);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}atan({Argument.ToString(variableName)})";
    }
}
