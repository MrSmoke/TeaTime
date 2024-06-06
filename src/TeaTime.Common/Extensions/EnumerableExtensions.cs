namespace TeaTime.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class
    {
        ArgumentNullException.ThrowIfNull(enumerable);

        foreach (var t in enumerable)
        {
            if (t is not null)
                yield return t;
        }
    }

    public static IEnumerable<TResult> WhereNotNull<T, TResult>(
        this IEnumerable<T> enumerable,
        Func<T, TResult?> selector)
        where TResult : class
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(selector);

        foreach (var t in enumerable.Select(selector))
        {
            if (t is not null)
                yield return t;
        }
    }

    public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source is IReadOnlyList<T> list)
            return list;

        return new ReadOnlyCollection<T>(source.ToList());
    }
}
