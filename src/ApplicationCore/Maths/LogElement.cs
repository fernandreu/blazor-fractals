namespace ApplicationCore.Maths
{
    public class LogElement : MathElement
    {
        public LogElement(MathElement argument)
        {
            Argument = argument;
        }

        public MathElement Argument { get; }
    }
}
