using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using ApplicationCore.Exceptions;
using ApplicationCore.Extensions;
using ApplicationCore.Helpers;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

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
    
        public static FractalResult Fractal(Func<Complex, Complex> func, FractalOptions options)
        {
            var result = new FractalResult
            {
                Contents = new Hsv[options.PixelSize.Width, options.PixelSize.Height],
            };
            
            var rnd = new Random();
            
            // Stored as a double to avoid castings later
            double totalPoints = options.PixelSize.Width * options.PixelSize.Height;

            var colors = new List<Hsv>();
            var roots = new List<Complex>();

            var logT = MathF.Log((float) options.Precision);
            
            for (var px = 0; px < options.PixelSize.Width; ++px)
            {
                var x = options.DomainSize.MinX + (options.DomainSize.MaxX - options.DomainSize.MinX) * px / (options.PixelSize.Width - 1);
                for (var py = 0; py < options.PixelSize.Height; ++py)
                {
                    var y = options.DomainSize.MinY + (options.DomainSize.MaxY - options.DomainSize.MinY) * py / (options.PixelSize.Height - 1);
                    var newtonOptions = new NewtonOptions
                    {
                        Precision = options.Precision,
                        StartingPoint = new Complex(x, y),
                        MaxIterations = options.MaxIterations,
                    };
                    var solution = NewtonMethod(func, newtonOptions);
                    result.MeanIterations += solution.Iterations / totalPoints;
                    result.StDevIterations += solution.Iterations * solution.Iterations / totalPoints;

                    if (solution.Status != SolutionStatus.Found)
                    {
                        result.Contents[px, py] = options.FillColor;
                        continue;
                    }

                    var found = FindRoot(roots, solution.Solution, options.Precision * 10);
                    Complex foundRoot;
                    Hsv color;
                    if (found == null)
                    {
                        foundRoot = solution.Solution;
                        roots.Add(solution.Solution);
                        color = new Hsv(rnd.NextFloat() * 360F, 1F, 1F);
                        colors.Add(color);
                    }
                    else
                    {
                        foundRoot = found.Value.root;
                        color = colors[found.Value.index];
                    }

                    if (options.Depth == 0)
                    {
                        result.Contents[px, py] = color;
                        continue;
                    }

                    var logD0 = MathF.Log((float) Complex.Abs(solution.PreviousSolution - foundRoot));
                    var logD1 = MathF.Log((float) Complex.Abs(solution.Solution - foundRoot));
                    var value = color.V;
                    
                    if (solution.Iterations > options.Threshold)
                    {
                        if (options.Gradient == 0)
                        {
                            var factor = (solution.Iterations - 1 - options.Threshold) * options.Depth * 0.01F;
                            if (options.Depth > 0)
                            {
                                value /= 1 + factor;
                            }
                            else
                            {
                                value += (1 - value) * (1 - 1 / (1 - factor));
                            }
                        }
                        else
                        {
                            var factor = (solution.Iterations - 1 - options.Threshold) * options.Depth * 0.01F;
                            var factorPlus = (solution.Iterations - options.Threshold) * options.Depth * 0.01F;
                            float lowValue;
                            float highValue;
                            if (options.Depth > 0)
                            {
                                lowValue = value / (1 + factor);
                                highValue = value / (1 + factorPlus);
                            }
                            else
                            {
                                lowValue = value + (1 - value) * (1 - 1 / (1 - factor));
                                highValue = value + (1 - value) * (1 - 1 / (1 - factorPlus));
                            }

                            if (Math.Abs(logD1 - logD0) < 0.001)
                            {
                                value = highValue;
                            }
                            else
                            {
                                value = highValue + options.Gradient * (highValue - lowValue) * (logT - logD0) / (logD1 - logD0);
                            }
                        }
                    }

                    value = MathF.Max(0.025F, MathF.Min(0.975F, value));
                    result.Contents[px, py] = new Hsv(color.H, color.S, value);
                }
            }

            result.StDevIterations = Math.Sqrt(result.StDevIterations - result.MeanIterations * result.MeanIterations);
            
            return result;
        }

        private static (Complex root, int index)? FindRoot(IEnumerable<Complex> list, Complex root, double precision)
        {
            foreach (var (item, index) in list.Enumerated())
            {
                if (Complex.Abs(item - root) < precision)
                {
                    return (item, index);
                }
            }

            return null;
        }
    }
}
