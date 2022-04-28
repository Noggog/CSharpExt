using System.Collections;

namespace Noggog;

public class SingleCollection<T> : IReadOnlyList<T>, ICollection<T>
{
    private readonly T _item;

    public SingleCollection(T item)
    {
        _item = item;
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        yield return _item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(T item)
    {
        return EqualityComparer<T>.Default.Equals(_item, item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        array[arrayIndex] = _item;
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    public int Count => 1;

    public bool IsReadOnly => true;

    public T this[int index]
    {
        get
        {
            if (index != 0) throw new IndexOutOfRangeException("Index was out of bounds");
            return _item;
        }
    }
}