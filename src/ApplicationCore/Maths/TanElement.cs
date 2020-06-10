namespace ApplicationCore.Maths
{
    public class TanElement : MathElement
    {
        public TanElement(MathElement argument)
        {
            Argument = argument;
        }

        public MathElement Argument { get; }
    }
}
