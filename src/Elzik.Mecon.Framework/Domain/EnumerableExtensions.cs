using System.Collections.Generic;
using System.Linq;

namespace Elzik.Mecon.Framework.Domain;

public static class EnumerableExtensions
{
    public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> values)
    {
        var valuesArray = values as T[] ?? values.ToArray();

        return source.Intersect(valuesArray).Count() == valuesArray.Distinct().Count();
    }
}