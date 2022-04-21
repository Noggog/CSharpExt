using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using DynamicData;
using DynamicData.Binding;

namespace Noggog.WPF.Containers;

public interface ISourceListUiFunnel<T> : IObservableCollection<T>
{
    ISourceList<T> SourceList { get; }
}
    
public class SourceListUiFunnel<T> : ISourceListUiFunnel<T>
{
    private readonly IObservableCollection<T> _coll;
        
    public ISourceList<T> SourceList { get; }

    public SourceListUiFunnel(ISourceList<T> sourceList, IDisposableDropoff disposableBucket)
    {
        SourceList = sourceList;
        _coll = sourceList.Connect()
            .ObserveOnGui()
            .ToObservableCollection(disposableBucket);
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => _coll.CollectionChanged += value;
        remove => _coll.CollectionChanged -= value;
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => _coll.PropertyChanged += value;
        remove => _coll.PropertyChanged -= value;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _coll.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_coll).GetEnumerator();
    }

    public void Add(T item)
    {
        _coll.Add(item);
    }

    public void Clear()
    {
        _coll.Clear();
    }

    public bool Contains(T item)
    {
        return _coll.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _coll.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return _coll.Remove(item);
    }

    public int Count => _coll.Count;

    public bool IsReadOnly => _coll.IsReadOnly;

    public int IndexOf(T item)
    {
        return _coll.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        _coll.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _coll.RemoveAt(index);
    }

    public T this[int index]
    {
        get => _coll[index];
        set => _coll[index] = value;
    }

    public IDisposable SuspendCount()
    {
        return _coll.SuspendCount();
    }

    public IDisposable SuspendNotifications()
    {
        return _coll.SuspendNotifications();
    }

    public void Load(IEnumerable<T> items)
    {
        _coll.Load(items);
    }

    public void Move(int oldIndex, int newIndex)
    {
        _coll.Move(oldIndex, newIndex);
    }
}