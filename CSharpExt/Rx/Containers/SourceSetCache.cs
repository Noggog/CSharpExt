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
    public class SourceSetCache<V, K> : ISourceSetCache<V, K>
    {
        private readonly BehaviorSubject<bool> _hasBeenSet = new BehaviorSubject<bool>(false);
        private readonly SourceCache<V, K> _source;

        public SourceSetCache(Func<V, K> keySelector)
        {
            this._source = new SourceCache<V, K>(keySelector);
        }

        public IEnumerable<K> Keys => _source.Keys;

        public IEnumerable<V> Items => _source.Items;

        public IEnumerable<KeyValuePair<K, V>> KeyValues => _source.KeyValues;

        public int Count => _source.Count;

        public IObservable<int> CountChanged => _source.CountChanged;

        public IEnumerable<V> DefaultValue => Enumerable.Empty<V>();
        
        IEnumerable<V> IHasBeenSetItem<IEnumerable<V>>.Item { get => _source.Items; set => _source.SetTo(value); }
        IEnumerable<V> IHasItem<IEnumerable<V>>.Item { get => _source.Items; set => _source.SetTo(value); }
        IEnumerable<V> IHasItemGetter<IEnumerable<V>>.Item => _source.Items;

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

        IEnumerable<V> IReadOnlyDictionary<K, V>.Values => this.Items;

        public V this[K key] => this._source[key];

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
            val = opt.HasValue ? opt.Value : default;
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void IHasBeenSetItem<IEnumerable<V>>.Set(IEnumerable<V> item, bool hasBeenSet)
        {
            this.Edit((l) =>
            {
                l.AddOrUpdate(item);
            },
            hasBeenSet: hasBeenSet);
        }

        public bool ContainsKey(K key)
        {
            return this._source.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return this._source.GetEnumerator();
        }
    }
}
