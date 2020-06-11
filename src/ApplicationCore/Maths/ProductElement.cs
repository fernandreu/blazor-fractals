using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApplicationCore.Maths
{
    public class ProductElement : MathElement
    {
        public ProductElement(params MathElement[] factors)
            : this(false, factors)
        {
        }

        public ProductElement(bool isNegative, params MathElement[] factors)
            : base(isNegative)
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

        public override MathElement Negated() => new ProductElement(!IsNegative, Factors.ToArray());
        
        public override MathElement Derive()
        {
            var terms = Factors.Select((factor, index) =>
            {
                var list = Factors.ToArray();
                list[index] = factor.Derive();
                return new ProductElement(IsNegative, list);
            });
            return new SumElement(terms.Cast<MathElement>().ToArray());
        }

        public override string ToString(string variableName)
        {
            var result = string.Join('*', Factors.Select(x => $"({x.ToString(variableName)})"));
            return $"{(IsNegative ? "-" : "")}{result}";
        }
    }
}
