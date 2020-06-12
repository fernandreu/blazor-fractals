using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<(T item, int index)> Enumerated<T>(this IEnumerable<T> self)
        {
            return self.Select((x, i) => (x, i));
        }
    }
}