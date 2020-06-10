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
    }
}
