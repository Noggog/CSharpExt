using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public interface ISplitEnumerableIntoSubtypes
{
    object Split<TItem>(
        ISpecimenContext context,
        Type paramType);
}

public class SplitEnumerableIntoSubtypes : ISplitEnumerableIntoSubtypes
{
    public object Split<TItem>(
        ISpecimenContext context,
        Type paramType)
    {
        if (paramType == typeof(IEnumerable<TItem>))
        {
            return context.Create<IEnumerable<TItem>>();
        }

        if (typeof(TItem[]).IsAssignableFrom(paramType))
        {
            return context.Create<IEnumerable<TItem>>().ToArray();
        }

        if (typeof(IReadOnlyList<TItem>).IsAssignableFrom(paramType)
            || typeof(IList<TItem>).IsAssignableFrom(paramType))
        {
            return context.Create<IEnumerable<TItem>>().ToList();
        }

        if (typeof(IReadOnlySet<TItem>).IsAssignableFrom(paramType)
            || typeof(ISet<TItem>).IsAssignableFrom(paramType))
        {
            return context.Create<IEnumerable<TItem>>().ToHashSet();
        }

        return new NoSpecimen();
    }
}