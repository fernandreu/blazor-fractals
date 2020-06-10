using System.Linq.Expressions;
using System.Numerics;

namespace ApplicationCore.Maths
{
    public class ConstElement : MathElement
    {
        public ConstElement(Complex value)
        {
            Value = value;
        }

        public Complex Value { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
            => NegateIfNeeded(Expression.Constant(Value));
    }
}
