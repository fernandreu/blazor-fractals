using System.Linq.Expressions;

namespace ApplicationCore.Maths
{
    public class VariableElement : MathElement
    {
        public VariableElement(bool isNegative = false)
            : base(isNegative, false)
        {
        }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            return NegateIfNeeded(parameter);
        }

        public override MathElement Negated() => new VariableElement(!IsNegative);
        
        public override MathElement Derive() => new ConstElement(1);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}{variableName}";
    }
}
