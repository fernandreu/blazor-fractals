using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Maths
{
    public class Operator
    {
        private static readonly Operator[][] FullList =
        {
            new[]
            {
                new Operator("_"), 
            },
            new[]
            {
                new Operator("^"), 
            },
            new[]
            {
                new Operator("*"), 
                new Operator("/"), 
            },
            new[]
            {
                new Operator("+"), 
                new Operator("-"), 
            },
            new[]
            {
                new Operator("("), 
                new Operator(")"), 
                new Operator("sin", arguments: 1, isFunction: true), 
                new Operator("cos", arguments: 1, isFunction: true), 
                new Operator("tan", arguments: 1, isFunction: true), 
                new Operator("log", arguments: 1, isFunction: true), 
            },
        };

        public static readonly IReadOnlyDictionary<string, Operator> All = ProcessList();
        
        public Operator(string token, int arguments = 2, bool isLeftAssociative = true, bool isFunction = false)
        {
            Token = token;
            Arguments = arguments;
            IsLeftAssociative = isLeftAssociative;
            IsFunction = isFunction;
        }
        
        public string Token { get; }

        public int Arguments { get; }
        
        public int Precedence { get; private set; }
        
        public bool IsLeftAssociative { get; }
        
        public bool IsFunction { get; }

        private static IReadOnlyDictionary<string, Operator> ProcessList()
        {
            var result = new Dictionary<string, Operator>();
            foreach (var (list, index) in FullList.Select((x, i) => (x, i)))
            {
                foreach (var op in list)
                {
                    op.Precedence = index;
                    result[op.Token] = op;
                }
            }

            return result;
        }
    }
}