using System.Linq.Expressions;
using System.Numerics;

namespace ApplicationCore.Maths
{
    public class FractionElement : MathElement
    {
        public FractionElement(MathElement numerator, MathElement denominator, bool isNegative = false)
            : base(isNegative, numerator.IsConstant && denominator.IsConstant)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public MathElement Numerator { get; }

        public MathElement Denominator { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            var result = Expression.Divide(Numerator.ToExpression(parameter), Denominator.ToExpression(parameter));
            return NegateIfNeeded(result);
        }

        public override MathElement Negated() => new FractionElement(Numerator, Denominator, !IsNegative);
        
        // d/dx(u/v) = 1/v du/dx - u/v2 dv/dx
        public override MathElement Derive()
            => new SumElement(
                new ProductElement(
                    new FractionElement(
                        new ConstElement(1, IsNegative), 
                        Denominator),
                    Numerator.Derive()),
                new ProductElement(
                    !IsNegative,
                    new FractionElement(
                        Numerator,
                        new PowerElement(
                            Denominator, 
                            new ConstElement(2))),
                    Denominator.Derive()));

        protected override MathElement SimplifyInternal()
        {
            var numerator = Numerator.Simplify();
            if (numerator is ConstElement c && c.Value == Complex.Zero)
            {
                return new ConstElement(Complex.Zero);
            }
            
            return new FractionElement(numerator, Denominator.Simplify(), IsNegative);
        }

        public override string ToString(string variableName)
        {
            return $"{(IsNegative ? "-" : "")}({Numerator.ToString(variableName)})/({Denominator.ToString(variableName)})";
        }
    }
}
