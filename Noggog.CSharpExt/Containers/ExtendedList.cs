namespace Noggog;

public class ExtendedList<T> : List<T>, IExtendedList<T>, IShallowCloneable
{
    public ExtendedList()
        : base()
    {
    }

    public ExtendedList(IEnumerable<T> collection)
        : base(collection)
    {
    }

    public void InsertRange(IEnumerable<T> collection, int index)
    {
        foreach (var item in collection.Reverse())
        {
            Insert(index, item);
        }
    }

    public void Move(int original, int destination)
    {
        var item = this[original];
        RemoveAt(original);
        Insert(destination, item);
    }

    public object ShallowClone()
    {
        return new ExtendedList<T>(this);
    }
}