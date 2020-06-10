namespace ApplicationCore.Maths
{
    public class CosElement : MathElement
    {
        public CosElement(MathElement argument)
        {
            Argument = argument;
        }

        public MathElement Argument { get; }
    }
}
