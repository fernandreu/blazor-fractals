using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Maths
{
    public class ProductElement : MathElement
    {
        public ProductElement(params MathElement[] factors)
        {
            Factors = factors.ToList();
        }

        public IReadOnlyList<MathElement> Factors { get; }
    }
}
