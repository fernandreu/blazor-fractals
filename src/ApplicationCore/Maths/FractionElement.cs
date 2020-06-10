namespace ApplicationCore.Maths
{
    public class FractionElement : MathElement
    {
        public FractionElement(MathElement numerator, MathElement denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public MathElement Numerator { get; }

        public MathElement Denominator { get; }
    }
}
