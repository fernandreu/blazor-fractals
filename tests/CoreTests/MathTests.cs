using System;
using System.Collections;
using System.Numerics;
using ApplicationCore.Maths;
using NUnit.Framework;

namespace CoreTests
{
    public class MathTests
    {
        public static IEnumerable CanParseExpressionData
        {
            get
            {
                yield return new TestCaseData("2+z", new Complex(3, 0), new Complex(5, 0));
                yield return new TestCaseData("2+3*z", new Complex(3, 0), new Complex(11, 0));
                yield return new TestCaseData("sin(z)", new Complex(0.5 * Math.PI, 0), Complex.One);
            }
        }

        private static void AssertEqual(Complex expected, Complex actual, double tolerance = 1e-3)
        {
            Assert.That(actual, Is.EqualTo(expected).Using<Complex>((a, b) => Complex.Abs(a - b) < tolerance));
        }

        [TestCaseSource(nameof(CanParseExpressionData))]
        public void CanParseExpression(string expression, Complex argument, Complex expected)
        {
            // Arrange
            var element = MathElement.Parse(expression);
            var expr = element.ToExpression();
            var func = expr.Compile();

            // Act
            var actual = func(argument);

            // Assert
            AssertEqual(expected, actual);
        }
    }
}
