using System;
using System.Collections;
using System.Numerics;
using ApplicationCore.Maths;
using NUnit.Framework;

namespace CoreTests
{
    public class MathTests
    {
        private static void AssertEqual(Complex expected, Complex actual, double tolerance = 1e-3)
        {
            Assert.That(actual, Is.EqualTo(expected).Using<Complex>((a, b) => Complex.Abs(a - b) < tolerance));
        }

        public static IEnumerable CanParseExpressionData
        {
            get
            {
                yield return new TestCaseData("2+z", new Complex(3, 0), new Complex(5, 0));
                yield return new TestCaseData("2+3*z", new Complex(3, 0), new Complex(11, 0));
                yield return new TestCaseData("sin(z)", new Complex(0.5 * Math.PI, 0), Complex.One);
            }
        }

        [TestCaseSource(nameof(CanParseExpressionData))]
        public void CanEvaluateExpression(string expression, Complex argument, Complex expected)
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

        [TestCase("2+z", ExpectedResult = "(2)+(z)")]
        [TestCase("2+3*z", ExpectedResult = "(2)+((3)*(z))")]
        [TestCase("-sin(z)", ExpectedResult = "-sin(z)")]
        public string CanFormatExpression(string expression)
        {
            var element = MathElement.Parse(expression);
            return element.ToString();
        }
    }
}
