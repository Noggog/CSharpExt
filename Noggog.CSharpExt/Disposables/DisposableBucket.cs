using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace Noggog
{
    public interface IDisposableDropoff : IDisposable
    {
        void Add(IDisposable disposable);
    }
    
    public interface IDisposableBucket : IDisposableDropoff, ICollection<IDisposable>
    {
        new void Add(IDisposable disposable);
    }
    
    public class DisposableBucket : IDisposableBucket
    {
        private readonly CompositeDisposable _compositeDisposable = new();

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return _compositeDisposable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_compositeDisposable).GetEnumerator();
        }

        public void Add(IDisposable item)
        {
            _compositeDisposable.Add(item);
        }

        public void Clear()
        {
            _compositeDisposable.Clear();
        }

        public bool Contains(IDisposable item)
        {
            return _compositeDisposable.Contains(item);
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            _compositeDisposable.CopyTo(array, arrayIndex);
        }

        public bool Remove(IDisposable item)
        {
            return _compositeDisposable.Remove(item);
        }

        public int Count => _compositeDisposable.Count;

        public bool IsReadOnly => _compositeDisposable.IsReadOnly;
    }
}