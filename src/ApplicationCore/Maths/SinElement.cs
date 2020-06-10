namespace ApplicationCore.Maths
{
    public class SinElement : MathElement
    {
        public SinElement(MathElement argument)
        {
            Argument = argument;
        }

        public MathElement Argument { get; }
    }
}
