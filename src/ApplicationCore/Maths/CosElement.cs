using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class CosElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Cos));

        public CosElement(MathElement argument, bool isNegative = false)
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

        public override MathElement Negated() => new CosElement(Argument, !IsNegative);

        public override MathElement Derive()
            => new ProductElement(!IsNegative, new SinElement(Argument), Argument.Derive());

        protected override MathElement SimplifyInternal()
            => new CosElement(Argument.Simplify(), IsNegative);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}cos({Argument.ToString(variableName)})";
    }
}
