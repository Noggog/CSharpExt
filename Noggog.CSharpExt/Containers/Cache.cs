using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Noggog
{
    public class Cache<TObject, TKey> : ICache<TObject, TKey>
        where TKey : notnull
    {
        private readonly Func<TObject, TKey> _keySelector;
        private Dictionary<TKey, TObject> _dict = new Dictionary<TKey, TObject>();

        public TObject this[TKey key] => this._dict[key];

        public IEnumerable<TKey> Keys => this._dict.Keys;

        public int Count => this._dict.Count;

        public IEnumerable<TObject> Items => this._dict.Values;

        public IEnumerable<KeyValuePair<TKey, TObject>> KeyValues => this._dict;

        public Cache(Func<TObject, TKey> keySelector)
        {
            this._keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public void Clear() => this._dict.Clear();

        public bool ContainsKey(TKey key) => this._dict.ContainsKey(key);

        public bool Remove(TKey key) => this._dict.Remove(key);

        public void Remove(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                this._dict.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TObject value) => this._dict.TryGetValue(key, out value);
        
        public TObject? TryGetValue(TKey key)
        {
            if (_dict.TryGetValue(key, out var val))
            {
                return val;
            }

            return default;
        }
        
        public void Refresh()
        {
            var tmp = this._dict;
            this._dict = new Dictionary<TKey, TObject>();
            foreach (var item in tmp.Values)
            {
                this._dict[_keySelector(item)] = item;
            }
        }

        public void Refresh(IEnumerable<TKey> keys)
        {
            List<TObject> objs = new List<TObject>();
            foreach (var key in keys)
            {
                if (this._dict.TryGetValue(key, out var val))
                {
                    objs.Add(val);
                    this._dict.Remove(key);
                }
            }
            foreach (var obj in objs)
            {
                this._dict[this._keySelector(obj)] = obj;
            }
        }

        public void Refresh(TKey key)
        {
            if (!this.TryGetValue(key, out var val)) return;
            this._dict.Remove(key);
            this._dict[this._keySelector(val)] = val;
        }

        public IEnumerator<IKeyValue<TObject, TKey>> GetEnumerator()
        {
            foreach (var item in this._dict)
            {
                yield return new KeyValue<TObject, TKey>(item.Key, item.Value);
            }
        }

        IEnumerator<IKeyValue<TObject, TKey>> IEnumerable<IKeyValue<TObject, TKey>>.GetEnumerator() => this.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Set(TObject item) => this._dict[this._keySelector(item)] = item;

        public void Set(IEnumerable<TObject> items)
        {
            foreach (var item in items)
            {
                this._dict[this._keySelector(item)] = item;
            }
        }

        public void Add(TObject item)
        {
            this._dict.Add(this._keySelector(item), item);
        }

        public bool Remove(TObject obj) => this._dict.Remove(this._keySelector(obj));

        public void Remove(IEnumerable<TObject> objects)
        {
            foreach (var item in objects)
            {
                this._dict.Remove(this._keySelector(item));
            }
        }

        public TObject GetOrAdd(TKey key, Func<TKey, TObject> createFunc)
        {
            if (this._dict.TryGetValue(key, out var val)) return val;
            val = createFunc(key);
            this.Set(val);
            return val;
        }
    }
}
