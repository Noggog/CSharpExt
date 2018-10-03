using DynamicData;
using DynamicData.Kernel;
using Noggog.Notifying;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.Rx
{
    public class SouceSetCache<V, K> : ISourceSetCache<V, K>
    {
        private readonly BehaviorSubject<bool> _hasBeenSet = new BehaviorSubject<bool>(false);
        private readonly SourceCache<V, K> _source;

        public SouceSetCache(Func<V, K> keySelector)
        {
            this._source = new SourceCache<V, K>(keySelector);
        }

        public IEnumerable<K> Keys => _source.Keys;

        public IEnumerable<V> Items => _source.Items;

        public IEnumerable<KeyValuePair<K, V>> KeyValues => _source.KeyValues;

        public int Count => _source.Count;

        public IObservable<int> CountChanged => _source.CountChanged;

        public IEnumerable<V> DefaultValue => Enumerable.Empty<V>();

        public IEnumerable<V> Item
        {
            get => _source.Items;
            set => _source.SetTo(value);
        }

        public bool HasBeenSet
        {
            get => _hasBeenSet.Value;
            set => _hasBeenSet.OnNext(value);
        }

        public IObservable<IEnumerable<V>> ItemObservable =>
            this._source
            .Connect()
            .QueryWhenChanged(q => q.Items);

        public IObservable<bool> HasBeenSetObservable => this._hasBeenSet;

        public IObservable<IChangeSet<V, K>> Connect(Func<V, bool> predicate = null)
        {
            return _source.Connect(predicate);
        }

        public void Dispose()
        {
            _source.Dispose();
        }


        public void Edit(Action<ISourceUpdater<V, K>> updateAction)
        {
            Edit(updateAction, hasBeenSet: true);
        }

        public void Edit(Action<ISourceUpdater<V, K>> updateAction, bool hasBeenSet)
        {
            if (!hasBeenSet)
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

        public Optional<V> Lookup(K key)
        {
            return _source.Lookup(key);
        }

        public void OnCompleted()
        {
            _source.OnCompleted();
        }

        public void OnError(Exception exception)
        {
            _source.OnError(exception);
        }

        public bool TryGetValue(K key, out V val)
        {
            var opt = _source.Lookup(key);
            val = opt.Value;
            return opt.HasValue;
        }

        public void Unset()
        {
            this.HasBeenSet = false;
            this.Clear();
        }

        public IObservable<Change<V, K>> Watch(K key)
        {
            return _source.Watch(key);
        }

        public IEnumerator<V> GetEnumerator()
        {
            return this._source.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void IHasBeenSetItem<IEnumerable<V>>.Set(IEnumerable<V> item, bool hasBeenSet)
        {
            this.Edit((l) =>
            {
                l.Load(item);
            },
            hasBeenSet: hasBeenSet);
        }
    }
}
