using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApplicationCore.Maths
{
    public class ProductElement : MathElement
    {
        public ProductElement(params MathElement[] factors)
        {
            Factors = factors.ToList();
        }

        public IReadOnlyList<MathElement> Factors { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            var factors = Factors.Select(x => x.ToExpression(parameter)).ToList();
            var result = factors[0];
            foreach (var factor in factors.Skip(1))
            {
                result = Expression.Multiply(result, factor);
            }

            return NegateIfNeeded(result);
        }

        public override string ToString(string variableName)
        {
            var result = string.Join('*', Factors.Select(x => $"({x.ToString(variableName)})"));
            return $"{(IsNegative ? "-" : "")}{result}";
        }
    }
}
