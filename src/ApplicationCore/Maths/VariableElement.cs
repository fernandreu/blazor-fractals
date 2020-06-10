namespace ApplicationCore.Maths
{
    public class VariableElement : MathElement
    {
        public VariableElement(string name = "z")
        {
            Name = name;
        }

        public string Name { get; }
    }
}
