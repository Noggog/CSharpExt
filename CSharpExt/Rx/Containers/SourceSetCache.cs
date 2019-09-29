using DynamicData;
using DynamicData.Kernel;
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
    public class SourceSetCache<TObject, TKey> : ISourceSetCache<TObject, TKey>, IEnumerable<KeyValuePair<TKey, TObject>>
    {
        private readonly BehaviorSubject<bool> _hasBeenSet = new BehaviorSubject<bool>(false);
        private readonly SourceCache<TObject, TKey> _source;

        public SourceSetCache(Func<TObject, TKey> keySelector)
        {
            this._source = new SourceCache<TObject, TKey>(keySelector);
        }

        public IEnumerable<TKey> Keys => _source.Keys;

        public IEnumerable<TObject> Items => _source.Items;

        public IEnumerable<KeyValuePair<TKey, TObject>> KeyValues => _source.KeyValues;

        public int Count => _source.Count;

        public IObservable<int> CountChanged => _source.CountChanged;

        public IEnumerable<TObject> DefaultValue => Enumerable.Empty<TObject>();
        
        IEnumerable<TObject> IHasBeenSetItem<IEnumerable<TObject>>.Item { get => _source.Items; set => _source.SetTo(value); }
        IEnumerable<TObject> IHasItem<IEnumerable<TObject>>.Item { get => _source.Items; set => _source.SetTo(value); }
        IEnumerable<TObject> IHasItemGetter<IEnumerable<TObject>>.Item => _source.Items;

        public bool HasBeenSet
        {
            get => _hasBeenSet.Value;
            set => _hasBeenSet.OnNext(value);
        }

        public IObservable<IEnumerable<TObject>> ItemObservable =>
            this._source
            .Connect()
            .QueryWhenChanged(q => q.Items);

        public IObservable<bool> HasBeenSetObservable => this._hasBeenSet;

        public TObject this[TKey key] => this._source[key];

        public IObservable<IChangeSet<TObject, TKey>> Connect(Func<TObject, bool> predicate = null)
        {
            return _source.Connect(predicate);
        }

        public void Dispose()
        {
            _source.Dispose();
        }
        
        public void Edit(Action<ISourceUpdater<TObject, TKey>> updateAction)
        {
            Edit(updateAction, hasBeenSet: true);
        }

        public void Edit(Action<ISourceUpdater<TObject, TKey>> updateAction, bool hasBeenSet)
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

        public Optional<TObject> Lookup(TKey key)
        {
            return _source.Lookup(key);
        }

        public IObservable<IChangeSet<TObject, TKey>> Preview(Func<TObject, bool> predicate = null)
        {
            return _source.Preview(predicate);
        }

        public bool TryGetValue(TKey key, out TObject val)
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

        public IObservable<Change<TObject, TKey>> Watch(TKey key)
        {
            return _source.Watch(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void IHasBeenSetItem<IEnumerable<TObject>>.Set(IEnumerable<TObject> item, bool hasBeenSet)
        {
            this.Edit((l) =>
            {
                l.AddOrUpdate(item);
            },
            hasBeenSet: hasBeenSet);
        }

        public bool ContainsKey(TKey key)
        {
            return this._source.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TObject>> GetEnumerator()
        {
            return this._source.GetEnumerator();
        }

        IEnumerator<IKeyValue<TObject, TKey>> IEnumerable<IKeyValue<TObject, TKey>>.GetEnumerator()
        {
            return this.KeyValues.Select<KeyValuePair<TKey, TObject>, IKeyValue<TObject, TKey>>(kv => new KeyValue<TObject, TKey>(kv.Key, kv.Value)).GetEnumerator();
        }
    }
}
