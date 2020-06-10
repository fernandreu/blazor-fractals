namespace ApplicationCore.Maths
{
    public class PowerElement : MathElement
    {
        public PowerElement(MathElement @base, MathElement exponent)
        {
            Base = @base;
            Exponent = exponent;
        }

        public MathElement Base { get; }

        public MathElement Exponent { get; }
    }
}
