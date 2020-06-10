using System.Linq.Expressions;

namespace ApplicationCore.Maths
{
    public class VariableElement : MathElement
    {
        public VariableElement(string name = "z")
        {
            Name = name;
        }

        public string Name { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            return NegateIfNeeded(parameter);
        }
        
        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}{variableName}";
    }
}
