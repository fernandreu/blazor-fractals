using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Maths
{
    public class SumElement : MathElement
    {
        public SumElement(params MathElement[] terms)
        {
            Terms = terms.ToList();
        }

        public IReadOnlyList<MathElement> Terms { get; }
    }
}
