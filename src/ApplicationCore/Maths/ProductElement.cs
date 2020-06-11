using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace ApplicationCore.Maths
{
    public class ProductElement : MathElement
    {
        public ProductElement(params MathElement[] factors)
            : this(false, factors)
        {
        }

        public ProductElement(bool isNegative, params MathElement[] factors)
            : base(isNegative, factors.All(x => x.IsConstant))
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

        private ProductElement Combine()
        {
            var factors = new List<MathElement>();
            var isNegative = IsNegative;
            foreach (var factor in Factors)
            {
                if (!(factor is ProductElement product))
                {
                    factors.Add(factor);
                    continue;
                }

                product = product.Combine();
                
                factors.AddRange(product.Factors);
                isNegative ^= product.IsNegative;
            }
            
            return new ProductElement(isNegative, factors.ToArray());
        }
        
        protected override MathElement SimplifyInternal()
        {
            var target = Combine();
            var factors = new List<MathElement>();
            var constant = target.IsNegative ? -Complex.One : Complex.One;
            foreach (var factor in target.Factors)
            {
                var simplified = factor.Simplify();
                
                if (!(simplified is ConstElement c))
                {
                    factors.Add(simplified);
                    continue;
                }
                
                constant *= c.IsNegative ? -c.Value : c.Value;
            }

            if (constant == Complex.Zero)
            {
                return new ConstElement(Complex.Zero);
            }

            if (constant != Complex.One)
            {
                factors.Insert(0, new ConstElement(constant));
            }
            
            if (factors.Count == 1)
            {
                return factors[0];
            }
            
            return new ProductElement(factors.ToArray());
        }

        public override string ToString(string variableName)
        {
            var result = string.Join('*', Factors.Select(x => $"({x.ToString(variableName)})"));
            return $"{(IsNegative ? "-" : "")}{result}";
        }
    }
}
