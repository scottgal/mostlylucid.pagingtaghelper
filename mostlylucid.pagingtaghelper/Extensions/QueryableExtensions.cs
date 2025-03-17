using System.Linq.Expressions;
using System.Reflection;

namespace mostlylucid.pagingtaghelper.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> OrderByField<T, TKey>(
        this IQueryable<T> source,
        Expression<Func<T, TKey>> keySelector,
        bool descending = false)
    {
        return descending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);
    }


    public static IQueryable<T> OrderByField<T>(
        this IQueryable<T> source,
        string sortBy,
        bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return source; // No sorting applied

        var property =
            typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property == null)
            throw new ArgumentException($"Property '{sortBy}' not found on type '{typeof(T)}'.");

        var param = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.MakeMemberAccess(param, property);
        var lambda = Expression.Lambda(propertyAccess, param);

        var methodName = descending ? "OrderByDescending" : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.PropertyType },
            source.Expression,
            Expression.Quote(lambda)
        );

        return source.Provider.CreateQuery<T>(resultExpression);
    }
    
    public static IEnumerable<T> OrderByField<T>(
        this IEnumerable<T> source,
        string sortBy,
        bool descending = false)
    {
        var property = typeof(T).GetProperty(sortPropertyName(sortBy), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property == null)
            throw new ArgumentException($"Property '{sortBy}' not found on type '{typeof(T)}'.");

        return descending
            ? source.OrderByDescending(x => property.GetValue(x, null))
            : source.OrderBy(x => property.GetValue(x, null));
    }

    // Helper methods for readability (optional)
    private static string sortPropertyName(string sortBy) => sortBy.Trim();
    private static string methodName(bool descending) => descending ? "OrderByDescending" : "OrderBy";
}