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

        [TestCase("2+z", ExpectedResult = "2+z")]
        [TestCase("2+3*z", ExpectedResult = "2+(3)*(z)")]
        [TestCase("-sin(z)", ExpectedResult = "-sin(z)")]
        [TestCase("2-z", ExpectedResult = "2-z")]
        public string CanFormatExpression(string expression)
        {
            var element = MathElement.Parse(expression);
            return element.ToString();
        }

        [TestCase("5")]
        [TestCase("z")]
        [TestCase("i")]
        [TestCase("2+i")]
        [TestCase("6i")]
        [TestCase("2-z")]
        [TestCase("6*(-z)")]
        [TestCase("sin(z)")]
        [TestCase("-cos(z)")]
        [TestCase("tan(z)")]
        [TestCase("log(z)")]
        [TestCase("5/z")]
        [TestCase("z^(-7)")]
        [TestCase("(1-z+6z^3-6sin(z))/(5*z - log(1/z))")]
        public void CanNegate(string expression)
        {
            var original = MathElement.Parse(expression);
            
            var copy = original.Negated();
            Assert.AreNotSame(original, copy);
            Assert.AreNotEqual(original.ToString(), copy.ToString());

            var reverted = copy.Negated();
            Assert.AreNotSame(original, reverted);
            Assert.AreEqual(original.ToString(), reverted.ToString());
        }

        [TestCase("1", ExpectedResult = "0")]
        [TestCase("z", ExpectedResult = "1")]
        [TestCase("2z", ExpectedResult = "(0)*(z)+(2)*(1)")]
        [TestCase("sin(z)", ExpectedResult = "(cos(z))*(1)")]
        public string CanDerive(string expression)
        {
            var element = MathElement.Parse(expression);
            var derived = element.Derive();
            return derived.ToString();
        }
    }
}
