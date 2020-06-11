using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using ApplicationCore.Exceptions;
using ApplicationCore.Helpers;

namespace ApplicationCore.Maths
{
    public static class MathUtils
    {
        private static readonly Regex WhitespaceRegex = new Regex(@"\s+");

        private static readonly HashSet<char> Separators = new HashSet<char> { '+', '-', '*', '/', '^', '(', ')', '_' };

        public static List<string> ToReversePolishNotation(string expression, string varName = "x")
        {
            var queue = Tokenize(expression);
            return ShuntingYardAlgorithm(queue, varName);
        }

        private static Queue<string> Tokenize(string expression)
        {
            var queue = new Queue<string>();

            // Remove all whitespace characters
            expression = WhitespaceRegex.Replace(expression, "");

            if (string.IsNullOrEmpty(expression))
            {
                throw new ParseException("The expression cannot be empty");
            }

            var start = 0;
            for (var i = 0; i < expression.Length; ++i)
            {
                var c = expression[i];
                if (Separators.Contains(c))
                {
                    if (i > 0 && (c == '+' || c == '-') && expression[i - 1] == 'E')
                    {
                        // Expressions like: "1e-5", "2e10", "sin(-3)"
                        continue;
                    }

                    if (c == '-' && (i == 0 || Separators.Contains(expression[i - 1]) && expression[i - 1] != ')'))
                    {
                        // Detecting unary minus: If previous symbol is another operator or a left parenthesis, or it just doesn't exist (first element of string)
                        queue.Enqueue("_"); // TODO: Did I miss enqueueing the previous value here?
                        start = i + 1;
                        continue;
                    }

                    if (start != i)
                    {
                        queue.Enqueue(expression[start..i]);
                    }

                    queue.Enqueue(expression.Substring(i, 1));
                    start = i + 1;
                }
                else if (i > 0 && !(char.IsDigit(c) || c == '.' || c == 'E') && char.IsDigit(expression[i - 1]))
                {
                    // Expressions like 3i, 2z, 2sin(...)
                    if (start != i)
                    {
                        queue.Enqueue(expression[start..i]);
                    }

                    queue.Enqueue("*");
                    start = i;
                }
            }

            if (start != expression.Length)
            {
                queue.Enqueue(expression.Substring(start));
            }

            return queue;
        }
        
        private static List<string> ShuntingYardAlgorithm(Queue<string> queue, string varName = "x")
        {
            var output = new List<string>();
            var stack = new Stack<Operator>();
            while (queue.Count > 0)
            {
                var token = queue.Dequeue();
                if (double.TryParse(token, out _) || token == varName || token == "pi" || token == "i" || token == "e")
                {
                    // Number of variable found
                    output.Add(token);
                    continue;
                }

                if (!Operator.All.TryGetValue(token, out var op))
                {
                    throw new ParseException($"Unrecognized token: {token}");
                }

                if (token == "(")
                {
                    stack.Push(op);
                    continue;
                }
                
                if (token == ")")
                {
                    while (stack.Count > 0 && stack.Peek().Token != "(")
                    {
                        output.Add(stack.Pop().Token);
                    }

                    if (stack.Count == 0)
                    {
                        throw new ParseException("Mismatched open / close brackets");
                    }

                    stack.Pop();
                    
                    continue;
                }
                
                if (op.IsFunction)
                {
                    stack.Push(op);
                    continue;
                }
                
                // Otherwise, it is a *normal* operator
                while (stack.Count > 0)
                {
                    var top = stack.Peek();
                    if (!((top.Precedence < op.Precedence || top.Precedence == op.Precedence && op.IsLeftAssociative) && top.Token != "("))
                    {
                        break;
                    }
                        
                    output.Add(stack.Pop().Token);
                }

                stack.Push(op);
            }

            while (stack.Count > 0)
            {
                output.Add(stack.Pop().Token);
            }

            return output;
        }

        public static NewtonResult NewtonMethod(Func<Complex, Complex> func, NewtonOptions options)
        {
            var result = new NewtonResult
            {
                Status = SolutionStatus.MaxIterationsReached,
            };

            var nan = new Complex(double.NaN, double.NaN);
            var previousPoints = (nan, nan);
            var newPoint = options.StartingPoint;
            for (var iteration = 1; iteration <= options.MaxIterations; ++iteration)
            {
                result.Iterations = iteration;
                previousPoints = (previousPoints.Item2, newPoint);
                newPoint = func(newPoint);

                if (double.IsNaN(newPoint.Real) || double.IsNaN(newPoint.Imaginary))
                {
                    result.Status = SolutionStatus.NaN;
                    break;
                }

                // This must have higher precision to avoid false positives
                if (iteration > 2 && Complex.Abs(previousPoints.Item1 - newPoint) < options.Precision * 1e-3)
                {
                    result.Status = SolutionStatus.CyclicBehavior;
                    break;
                }

                if (Complex.Abs(previousPoints.Item2 - newPoint) < options.Precision)
                {
                    result.Status = SolutionStatus.Found;
                    break;
                }
            }

            result.Solution = newPoint;
            result.PreviousSolution = previousPoints.Item2;
            return result;
        }
    }
}
