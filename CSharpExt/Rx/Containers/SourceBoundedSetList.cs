using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace CSharpExt.Rx
{
    // ToDo
    // Eventually redo to a SourceArray object
    public class SourceBoundedSetList<T> : SourceSetList<T>
    {
        private int _MaxValue;
        public int MaxValue
        {
            get => _MaxValue;
            set => SetMaxValue(value);
        }

        public SourceBoundedSetList(int max = int.MaxValue, IObservable<IChangeSet<T>> source = null)
            : base(source)
        {
            this.MaxValue = max;
        }

        public override void Edit(Action<IExtendedList<T>> updateAction, bool hasBeenSet)
        {
            base.Edit(updateAction, hasBeenSet);
            if (this.Count > this.MaxValue)
            {
                throw new ArgumentException($"Executed an edit on a list that would make it bigger than the allowed value {this.Count} > {_MaxValue}");
            }
        }

        private void SetMaxValue(int max)
        {
            if (max < this.Count)
            {
                throw new ArgumentException($"Max was set on a list that was bigger than the allowed value {this.Count} > {max}");
            }
            this._MaxValue = max;
        }
    }

    // ToDo
    // Eventually redo to a SourceArray object
    public class SourceBoundedList<T> : ISourceList<T>
    {
        private SourceList<T> _source;

        private int _MaxValue;
        public int MaxValue
        {
            get => _MaxValue;
            set => SetMaxValue(value);
        }

        public int Count => _source.Count;

        public IObservable<int> CountChanged => _source.CountChanged;

        public IEnumerable<T> Items => _source.Items;

        public bool IsReadOnly => ((ISourceList<T>)_source).IsReadOnly;

        T IList<T>.this[int index] { get => ((IList<T>)_source)[index]; set => ((IList<T>)_source)[index] = value; }

        public T this[int index] => ((IList<T>)_source)[index];

        public SourceBoundedList(int max = int.MaxValue, IObservable<IChangeSet<T>> source = null)
        {
            _source = new SourceList<T>(source);
            this.MaxValue = max;
        }

        private void SetMaxValue(int max)
        {
            if (max < this.Count)
            {
                throw new ArgumentException($"Max was set on a list that was bigger than the allowed value {this.Count} > {max}");
            }
            this._MaxValue = max;
        }

        public void Edit(Action<IExtendedList<T>> updateAction)
        {
            _source.Edit(updateAction);
            if (this.Count > this.MaxValue)
            {
                throw new ArgumentException($"Executed an edit on a list that would make it bigger than the allowed value {this.Count} > {_MaxValue}");
            }
        }

        public IObservable<IChangeSet<T>> Connect(Func<T, bool> predicate = null)
        {
            return _source.Connect(predicate);
        }

        public void Dispose()
        {
            _source.Dispose();
        }

        public int IndexOf(T item)
        {
            return _source.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((ISourceList<T>)_source).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((ISourceList<T>)_source).RemoveAt(index);
        }

        public void Add(T item)
        {
            ((ISourceList<T>)_source).Add(item);
        }

        public void Clear()
        {
            ((ISourceList<T>)_source).Clear();
        }

        public bool Contains(T item)
        {
            return _source.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _source.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((ISourceList<T>)_source).Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        public IObservable<IChangeSet<T>> Preview(Func<T, bool> predicate = null)
        {
            return _source.Preview(predicate);
        }
    }
}
