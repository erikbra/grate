using System.Collections.Generic;
using System.Linq;

namespace grate.Infrastructure;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Safe<T>(this T[]? source) => source ?? Enumerable.Empty<T>();
    public static IEnumerable<T> Safe<T>(this IEnumerable<T>? source) => source ?? Enumerable.Empty<T>();
}
