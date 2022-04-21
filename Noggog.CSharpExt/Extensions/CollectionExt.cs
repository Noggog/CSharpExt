namespace Noggog;

public static class CollectionExt
{
    public static T AddReturn<T>(this ICollection<T> coll, T item)
    {
        coll.Add(item);
        return item;
    }
}