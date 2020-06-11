using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace ApplicationCore.Maths
{
    public class SumElement : MathElement
    {
        public SumElement(params MathElement[] terms)
            : this(false, terms)
        {
        }

        public SumElement(bool isNegative, params MathElement[] terms)
            : base(isNegative, terms.All(x => x.IsConstant))
        {
            Terms = terms.ToList();
        }
        
        public IReadOnlyList<MathElement> Terms { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            var terms = Terms.Select(x => x.ToExpression(parameter)).ToList();
            var result = terms[0];
            foreach (var term in terms.Skip(1))
            {
                result = Expression.Add(result, term);
            }

            return NegateIfNeeded(result);
        }

        public override MathElement Negated() => new SumElement(!IsNegative, Terms.ToArray());
        
        public override MathElement Derive()
            => new SumElement(IsNegative, Terms.Select(x => x.Derive()).ToArray());

        private SumElement Combine()
        {
            var terms = new List<MathElement>();
            foreach (var term in Terms)
            {
                if (!(term is SumElement sum))
                {
                    terms.Add(term);
                    continue;
                }

                sum = sum.Combine();
                
                terms.AddRange(sum.Terms.Select(x => IsNegative ^ sum.IsNegative ? x.Negated() : x));
            }
            
            return new SumElement(terms.ToArray());
        }
        
        protected override MathElement SimplifyInternal()
        {
            var target = Combine();
            var terms = new List<MathElement>();
            var constant = Complex.Zero;
            foreach (var term in target.Terms)
            {
                var simplified = term.Simplify();
                
                if (!(simplified is ConstElement c))
                {
                    terms.Add(simplified);
                    continue;
                }

                constant += c.IsNegative ? -c.Value : c.Value;
            }

            if (constant != Complex.Zero)
            {
                terms.Insert(0, new ConstElement(constant));
            }
            
            if (terms.Count == 1)
            {
                return terms[0];
            }
            
            return new SumElement(terms.ToArray());
        }

        public override string ToString(string variableName)
        {
            var parts = Terms.Select((term, index) =>
            {
                var str = term.ToString(variableName);
                if (!term.IsNegative && index > 0)
                {
                    str = "+" + str;
                }

                // Negative terms should already have extra brackets if needed (e.g. '-(2+3)')
                return str;
            });
            
            var result = string.Join(string.Empty, parts);
            if (IsNegative)
            {
                result = $"-({result})";
            }

            return result;
        }
    }
}
