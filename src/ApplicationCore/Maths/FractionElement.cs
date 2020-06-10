using System.Linq.Expressions;

namespace ApplicationCore.Maths
{
    public class FractionElement : MathElement
    {
        public FractionElement(MathElement numerator, MathElement denominator)
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
    }
}
