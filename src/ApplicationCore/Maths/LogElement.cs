﻿using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApplicationCore.Maths
{
    public class LogElement : MathElement
    {
        private static readonly MethodInfo Method = typeof(Complex).GetMethod(nameof(Complex.Log), new[] {typeof(Complex)});

        public LogElement(MathElement argument, bool isNegative = false)
            : base(isNegative, argument.IsConstant)
        {
            Argument = argument;
        }

        public MathElement Argument { get; }

        protected internal override Expression ToExpression(ParameterExpression parameter)
        {
            var result = Expression.Call(Method, Argument.ToExpression(parameter));
            return NegateIfNeeded(result);
        }

        public override MathElement Negated() => new LogElement(Argument, !IsNegative);
        
        public override MathElement Derive() => new FractionElement(Argument.Derive(), Argument, IsNegative);

        protected override MathElement SimplifyInternal()
            => new LogElement(Argument.Simplify(), IsNegative);

        public override string ToString(string variableName)
            => $"{(IsNegative ? "-" : "")}log({Argument.ToString(variableName)})";
    }
}
