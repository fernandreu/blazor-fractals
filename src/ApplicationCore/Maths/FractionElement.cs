using System.Linq.Expressions;

namespace ApplicationCore.Maths
{
    public class FractionElement : MathElement
    {
        public FractionElement(MathElement numerator, MathElement denominator, bool isNegative = false)
        {
            Numerator = numerator;
            Denominator = denominator;
            IsNegative = isNegative;
        }

        public MathElement Numerator { get; }

        public MathElement Denominator { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            var result = Expression.Divide(Numerator.ToExpression(parameter), Denominator.ToExpression(parameter));
            return NegateIfNeeded(result);
        }

        public override MathElement Clone() => new FractionElement(Numerator.Clone(), Denominator.Clone(), IsNegative);

        public override string ToString(string variableName)
        {
            return $"{(IsNegative ? "-" : "")}({Numerator.ToString(variableName)})/({Denominator.ToString(variableName)})";
        }
    }
}
