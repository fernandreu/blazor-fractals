using System.Linq.Expressions;

namespace ApplicationCore.Maths
{
    public class VariableElement : MathElement
    {
        public VariableElement(bool isNegative = false)
        {
            IsNegative = isNegative;
        }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            return NegateIfNeeded(parameter);
        }

        public override MathElement Clone() => new VariableElement(IsNegative);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}{variableName}";
    }
}
