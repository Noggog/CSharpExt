using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Noggog;

public static class ExceptionExt
{
    [Pure]
    [return: NotNullIfNotNull("lhs")]
    [return: NotNullIfNotNull("rhs")]
    public static Exception? Combine(this Exception? lhs, Exception? rhs)
    {
        if (lhs == null && rhs == null) return null;
        if (lhs == null) return rhs;
        if (rhs == null) return lhs;
        var lhsAgg = lhs as AggregateException;
        var rhsAgg = rhs as AggregateException;
        if (lhsAgg != null && rhsAgg != null)
        {
            return new AggregateException(
                lhsAgg.InnerExceptions.Select(i => i).And(rhsAgg.InnerExceptions));
        }
        if (lhsAgg != null)
        {
            return new AggregateException(
                lhsAgg.InnerExceptions.And(rhs));
        }
        if (rhsAgg != null)
        {
            return new AggregateException(
                rhsAgg.InnerExceptions.And(lhs));
        }
        return new AggregateException(
            lhs,
            rhs);
    }

    [Pure]
    [return: NotNullIfNotNull("lhs")]
    [return: NotNullIfNotNull("rhs")]
    public static IEnumerable<T>? Combine<T>(this IEnumerable<T>? lhs, IEnumerable<T>? rhs)
    {
        if (lhs == null && rhs == null) return null;
        if (rhs == null) return lhs;
        if (lhs == null) return rhs;
        return lhs.And(rhs);
    }
}