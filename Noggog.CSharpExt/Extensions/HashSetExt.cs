namespace Noggog;

public static class HashSetExt
{
    public static void Add<T>(this HashSet<T> set, IEnumerable<T> enumer)
    {
        foreach (var e in enumer)
        {
            set.Add(e);
        }
    }

    public static void Add<T>(this HashSet<T> set, params T[] objs)
    {
        foreach (var e in objs)
        {
            set.Add(e);
        }
    }

    public static void Remove<T>(this HashSet<T> set, IEnumerable<T> vals)
    {
        foreach (var item in vals)
        {
            set.Remove(item);
        }
    }
 
    public static bool SetEquals<T>(this HashSet<T> set, params T[] rhs) 
    { 
        return set.SetEquals((IEnumerable<T>)rhs); 
    }
}