using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class PowerElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Pow), new[] { typeof(Complex), typeof(Complex) });

        public PowerElement(MathElement @base, MathElement exponent, bool isNegative = false)
            : base(isNegative, @base.IsConstant && exponent.IsConstant)
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

        public override MathElement Negated() => new PowerElement(Base, Exponent, !IsNegative);
        
        // d/dx(u^v) = v u^(v-1) du/dx + log(u) u^v dv/dx
        public override MathElement Derive()
            => new SumElement(
                new ProductElement(
                    IsNegative,
                    Exponent,
                    new PowerElement(
                        Base,
                        new SumElement(
                            Exponent,
                            new ConstElement(-1))),
                    Base.Derive()),
                new ProductElement(
                    IsNegative,
                    new LogElement(Base),
                    this,
                    Exponent.Derive()));

        protected override MathElement SimplifyInternal()
            => new PowerElement(Base.Simplify(), Exponent.Simplify(), IsNegative);

        public override string ToString(string variableName)
        {
            return $"{(IsNegative ? "-" : "")}({Base.ToString(variableName)})^({Exponent.ToString(variableName)})";
        }
    }
}
