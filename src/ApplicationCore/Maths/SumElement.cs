using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApplicationCore.Maths
{
    public class SumElement : MathElement
    {
        public SumElement(params MathElement[] terms)
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
    }
}
