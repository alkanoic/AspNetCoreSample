using System.Linq.Expressions;
using System.Reflection;

using LinqKit;

using Microsoft.EntityFrameworkCore;

namespace CodeGen.Result;

public static class QueryableExtensions
{
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
    {
        return ApplyOrder(source, propertyName, "OrderBy");
    }

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
    {
        return ApplyOrder(source, propertyName, "OrderByDescending");
    }

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
    {
        return ApplyOrder(source, propertyName, "OrderBy");
    }

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
    {
        return ApplyOrder(source, propertyName, "OrderByDescending");
    }

    private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, string methodName)
    {
        var parameter = Expression.Parameter(typeof(T));
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);
        var method = typeof(Queryable).GetMethods().First(m => m.Name == methodName
                                                              && m.GetParameters().Length == 2)
                                       .MakeGenericMethod(typeof(T), property.Type);
        var result = method.Invoke(null, [source, lambda]);
        if (result != null)
        {
            return (IOrderedQueryable<T>)result;
        }
        else
        {
            throw new InvalidOperationException("The invoked method returned null.");
        }
    }

    public static IQueryable<T> WhereAllColumns<T>(this IQueryable<T> query, string searchTerm) where T : class
    {
        var parameter = Expression.Parameter(typeof(T));

        var predicate = PredicateBuilder.New<T>();

        foreach (var property in typeof(T).GetProperties())
        {
            Expression? expr = null;

            if (property.PropertyType == typeof(string))
            {
                expr = GetContainsMethod(parameter, property, searchTerm);
            }
            else if (property.PropertyType == typeof(int) ||
                    property.PropertyType == typeof(long) ||
                    property.PropertyType == typeof(bool))
            {
                expr = GetEqualMethod(parameter, property, searchTerm);
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                expr = GetDateMethod(parameter, property, searchTerm);
            }

            if (expr != null)
            {
                predicate = predicate.Or(Expression.Lambda<Func<T, bool>>(expr, parameter));
            }
        }

        return query.Where(predicate);
    }

    public static IQueryable<T> WhereColumns<T>(this IQueryable<T> query, SearchBuilder searchBuilder) where T : class
    {
        var parameter = Expression.Parameter(typeof(T));

        var predicate = PredicateBuilder.New<T>();

        foreach (var c in searchBuilder.SearchConditions)
        {
            Expression? expr = null;

            var p = typeof(T).GetProperty(c.Data);
            if (p is null) continue;

            if (c.Type == SearchColumnType.Num)
            {
                switch (c.NumCondition)
                {
                    case SearchNumCondition.Equals:
                        expr = GetEqualMethod(parameter, p, c.Value);
                        break;
                }
            }
            else
            {
                switch (c.StrCondition)
                {

                }
            }

            if (expr != null)
            {
                if (searchBuilder.Logic == SearchLogic.AND)
                {
                    predicate = predicate.And(Expression.Lambda<Func<T, bool>>(expr, parameter));
                }
                else
                {
                    predicate = predicate.Or(Expression.Lambda<Func<T, bool>>(expr, parameter));
                }
            }
        }

        return query.Where(predicate);
    }

    private static UnaryExpression GetNotContainsMethod(ParameterExpression parameter, PropertyInfo property, object searchTerm)
    {
        return Expression.Not(GetContainsMethod(parameter, property, searchTerm));
    }

    private static MethodCallExpression GetContainsMethod(ParameterExpression parameter, PropertyInfo property, object searchTerm)
    {
        return Expression.Call(
          Expression.Property(parameter, property),
          "Contains",
          null,
          Expression.Constant(searchTerm)
        );
    }

    private static BinaryExpression? GetEqualMethod(ParameterExpression parameter, PropertyInfo property, object searchTerm)
    {
        if (!int.TryParse(searchTerm.ToString(), out var result))
        {
            return null;
        }
        return Expression.Equal(
            Expression.Property(parameter, property),
            Expression.Constant(result)
        );
    }

    private static BinaryExpression? GetDateMethod(ParameterExpression parameter, PropertyInfo property, object searchTerm)
    {
        if (!DateTime.TryParse(searchTerm.ToString(), out var result))
        {
            return null;
        }

        return Expression.GreaterThanOrEqual(
            Expression.Property(parameter, property),
            Expression.Constant(result)
        );
    }
}
