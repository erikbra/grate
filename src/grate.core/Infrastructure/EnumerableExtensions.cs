namespace grate.Infrastructure;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> Safe<T>(this T[]? source) => source ?? Enumerable.Empty<T>();
    public static IEnumerable<T> Safe<T>(this IEnumerable<T>? source) => source ?? Enumerable.Empty<T>();
}
