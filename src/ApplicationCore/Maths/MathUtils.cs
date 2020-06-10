using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApplicationCore.Maths
{
    public static class MathUtils
    {
        private static readonly Regex WhitespaceRegex = new Regex(@"\s+");

        private static readonly HashSet<char> Separators = new HashSet<char> { '+', '-', '*', '/', '^', '(', ')', '_' };

        public static List<string> ToReversePolishNotation(string expression, string varName = "x")
        {
            // Part 1: Tokenize
            var queue = new Queue<string>();

            // Remove all whitespace characters
            expression = WhitespaceRegex.Replace(expression, "");

            var output = new List<string>();

            if (string.IsNullOrEmpty(expression))
            {
                return output;
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

            // Part 2: Apply the Shunting-yard algorithm
            var stack = new Stack<string>();
            while (queue.Count > 0)
            {
                var token = queue.Dequeue();
                if (double.TryParse(token, out _) || token == varName || token == "pi" || token == "i" || token == "e")
                {
                    // Number of variable found
                    output.Add(token);
                }

                switch (token)
                {
                    case "_":
                    case "^":
                    case "(":
                        stack.Push(token);
                        break;
                    case "*":
                    case "/":
                    {
                        while (stack.Count > 0)
                        {
                            var top = stack.Peek();
                            if (top == "^" || top == "_")
                            {
                                break;
                            }

                            output.Add(stack.Pop());
                        }

                        stack.Push(token);
                        break;
                    }
                    case "+":
                    case "-":
                    {
                        while (stack.Count > 0)
                        {
                            var top = stack.Peek();
                            if (top == "^" || top == "_" || top == "*" || top == "/")
                            {
                                break;
                            }

                            output.Add(stack.Pop());
                        }

                        stack.Push(token);
                        break;
                    }
                    case ")":
                    {
                        while (stack.Count > 0 && stack.Peek() != "(")
                        {
                            output.Add(stack.Pop());
                        }

                        stack.Pop();

                        if (stack.Count == 0)
                        {
                            break;
                        }

                        var top = stack.Peek();
                        // TODO: Review this condition, especially the top[0] part, as the index was 1 originally
                        if (!double.TryParse(top, out _) && top != varName && top != "pi" && top != "i" && top != "e" && !(top.Length == 1 && Separators.Contains(top[0])))
                        {
                            output.Add(stack.Pop());
                        }

                        break;
                    }
                    default:
                        // Only thing left: function token (or error)
                        stack.Push(token);
                        break;
                }
            }

            while (stack.Count > 0)
            {
                output.Add(stack.Pop());
            }

            return output;
        }
    }
}
