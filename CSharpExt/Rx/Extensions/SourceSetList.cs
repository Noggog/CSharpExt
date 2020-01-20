using DynamicData;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.Rx
{
    public class SourceSetList<T> : ISourceSetList<T>
    {
        private readonly BehaviorSubject<bool> _hasBeenSet = new BehaviorSubject<bool>(false);
        private readonly SourceList<T> _source;
        
        public SourceSetList(IObservable<IChangeSet<T>>? source = null)
        {
            this._source = new SourceList<T>(source);
        }

        public IObservable<int> CountChanged => _source.CountChanged;

        public IEnumerable<T> Items => _source.Items;

        public int Count => _source.Count;

        public IEnumerable<T> DefaultValue => Enumerable.Empty<T>();

        IEnumerable<T> IHasItemGetter<IEnumerable<T>>.Item
        {
            get => _source.Items;
        }

        public bool HasBeenSet
        {
            get => _hasBeenSet.Value;
            set => _hasBeenSet.OnNext(value);
        }

        IObservable<IEnumerable<T>> IHasBeenSetItemRxGetter<IEnumerable<T>>.ItemObservable => 
            this._source
            .Connect()
            .QueryWhenChanged(q => q);

        public IObservable<bool> HasBeenSetObservable => this._hasBeenSet;

        bool ICollection<T>.IsReadOnly => false;

        public T this[int index]
        {
            get => _source.Items.ElementAt(index);
            set => _source.Edit(l => l[index] = value);
        }

        public IObservable<IChangeSet<T>> Connect(Func<T, bool>? predicate = null)
        {
            return _source.Connect(predicate);
        }

        public void Dispose()
        {
            _source.Dispose();
        }

        public void Edit(Action<IExtendedList<T>> updateAction)
        {
            Edit(updateAction, hasBeenSet: true);
        }

        public virtual void Edit(Action<IExtendedList<T>> updateAction, bool hasBeenSet)
        {
            if (hasBeenSet)
            {
                _source.Edit(updateAction);
                this.HasBeenSet = true;
            }
            else
            {
                this.HasBeenSet = false;
                _source.Edit(updateAction);
            }
        }

        public IObservable<IChangeSet<T>> Preview(Func<T, bool>? predicate = null)
        {
            return _source.Preview(predicate);
        }

        public void Unset()
        {
            this.HasBeenSet = false;
            this.Edit(list => list.Clear(), hasBeenSet: false);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._source.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(T item) => SourceListEditConvenienceEx.Add(this, item);

        public void AddRange(IEnumerable<T> item) => SourceListEditConvenienceEx.AddRange(this, item);

        public int IndexOf(T item)
        {
            return this._source.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            this.Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        void ICollection<T>.Clear()
        {
            this.Clear();
        }

        public bool Contains(T item)
        {
            return this._source.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._source.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {
            return this.Remove(item);
        }
    }
}
