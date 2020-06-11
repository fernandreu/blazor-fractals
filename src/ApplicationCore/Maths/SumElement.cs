using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationCore.Maths
{
    public class SumElement : MathElement
    {
        public SumElement(params MathElement[] terms)
            : this(false, terms)
        {
        }

        public SumElement(bool isNegative, params MathElement[] terms)
        {
            IsNegative = isNegative;
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

        public override MathElement Clone() => new SumElement(IsNegative, Terms.Select(x => x.Clone()).ToArray());

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
