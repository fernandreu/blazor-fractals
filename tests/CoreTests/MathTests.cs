using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ApplicationCore.Exceptions;
using ApplicationCore.Helpers;
using ApplicationCore.Maths;
using NUnit.Framework;

namespace CoreTests
{
    public class MathTests
    {
        private static void AssertEqual(Complex expected, Complex actual, double tolerance = 1e-3)
        {
            Assert.That(
                actual, 
                Is.EqualTo(expected).Using<Complex>((a, b) => Complex.Abs(a - b) < tolerance));
        }
        
        private static void AssertEqual(Complex expected, Complex actual, string message, double tolerance = 1e-3)
        {
            Assert.That(
                actual, 
                Is.EqualTo(expected).Using<Complex>((a, b) => Complex.Abs(a - b) < tolerance),
                message);
        }

        private static List<string> SampleFunctions { get; } = new List<string>
        {
            "5",
            "z",
            "i",
            "2+i",
            "6i",
            "2-z",
            "6*(-z)",
            "sin(z)",
            "-cos(z)",
            "tan(z)",
            "log(z)",
            "exp(z)",
            "sqrt(z)",
            "-asin(z)",
            "acos(z)",
            "-atan(z)",
            "-sinh(z)",
            "cosh(z)",
            "-tanh(z)",
            "5/z",
            "z^(-7)",
            "(1-z+6z^3-6sin(z))/(5*z - log(1/z))",
        };

        public static List<TestCaseData> SampleData { get; } = SampleFunctions.Select(x => new TestCaseData(x)).ToList();
        
        public static List<TestCaseData> EvaluateData { get; } = new List<TestCaseData>
        {
            new TestCaseData("2+z", new Complex(3, 0), new Complex(5, 0)),
            new TestCaseData("2+3*z", new Complex(3, 0), new Complex(11, 0)),
            new TestCaseData("sin(z)", new Complex(0.5 * Math.PI, 0), Complex.One),
        };

        [TestCaseSource(nameof(EvaluateData))]
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

        [TestCase("abc")]
        [TestCase("sin()")]
        [TestCase("1+2)")]
        [TestCase("(1+2")]
        public void CanDetectMalformedExpressions(string expression)
        {
            Assert.Throws<ParseException>(() => MathElement.Parse(expression));
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

        [TestCaseSource(nameof(SampleData))]
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

        [TestCaseSource(nameof(SampleData))]
        public void CanGenerateNewtonFunction(string expression)
        {
            var baseElement = MathElement.Parse(expression);
            
            // Making sure the base function can be created, since we are on it
            baseElement.ToFunc();

            var newtonElement = baseElement.ToNewtonFunction(Complex.One);
            newtonElement.ToFunc();
        }
        
        [TestCase("1*z*3*2", ExpectedResult = "(6)*(z)")]
        [TestCase("5+z-7", ExpectedResult = "-2+z")]
        [TestCase("1-2-3", ExpectedResult = "-4")]
        [TestCase("(2+6-6-2)/(z^2+log(z))", ExpectedResult = "0")]
        public string CanSimplify(string expression)
        {
            var original = MathElement.Parse(expression);
            var actual = original.Simplify();
            return actual.ToString();
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

        [TestCase("z^2-1", 1, 5, 1)]
        [TestCase("z^2-1", 1, -5, -1)]
        public void NewtonMethodTests(string expression, double multiplicity, double startingPoint, double expected)
        {
            // Only testing the real part, as it makes it simpler to pass compile-time
            // inputs. The outcome should be the same for complex numbers, especially
            // given that the Complex library already existed
            
            var element = MathElement.Parse(expression);
            var newton = element.ToNewtonFunction(new Complex(multiplicity, 0));
            var func = newton.ToFunc();

            var options = new NewtonOptions
            {
                MaxIterations = 50,
                Precision = 1e-3,
                StartingPoint = new Complex(startingPoint, 0),
            };

            var result = MathUtils.NewtonMethod(func, options);
            Assert.AreEqual(expected, result.Solution.Real, 1e-2);
        }
    }
}
