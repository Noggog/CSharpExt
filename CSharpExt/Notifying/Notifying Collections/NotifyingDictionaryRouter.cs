using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog.Notifying
{
    public class NotifyingDictionaryRouter<K, V> : INotifyingDictionary<K, V>
    {
        INotifyingDictionaryGetter<K, V> _base;
        INotifyingDictionary<K, V> _child;

        public bool HasBeenSwapped { get; private set; }

        public INotifyingItemGetter<int> CountProperty => _child.CountProperty;
        public int Count => _child.Count;

        public bool HasBeenSet
        {
            get
            {
                return (HasBeenSwapped ? _child.HasBeenSet : _base.HasBeenSet);
            }

            set
            {
                SwapOver();
                _child.HasBeenSet = value;
            }
        }

        public IEnumerable<K> Keys => _child.Keys;

        public IEnumerable<V> Values => _child.Values;

        IEnumerable<KeyValuePair<K, V>> IHasItemGetter<IEnumerable<KeyValuePair<K, V>>>.Item => _child.Item;

        V INotifyingDictionaryGetter<K, V>.this[K key] => _child[key];

        public V this[K key]
        {
            get
            {
                return _child[key];
            }

            set
            {
                SwapOver();
                _child[key] = value;
            }
        }

        public NotifyingDictionaryRouter(
            INotifyingDictionaryGetter<K, V> _base,
            INotifyingDictionary<K, V> _child)
        {
            this._base = _base;
            this._child = _child;
        }
        
        private void SwapOver()
        {
            if (HasBeenSwapped) return;
            _base.Unsubscribe(this);
        }

        private void SwapBack()
        {
            if (!HasBeenSwapped) return;
            _base.Subscribe(
                this,
                (changes) =>
                {
                    this._child.SetTo(_base);
                });
        }

        public void Set(K key, V val, NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.Set(key, val, cmds);
        }

        public void Remove(K key, NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.Remove(key, cmds);
        }

        public void Subscribe<O>(O owner, NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>.NotifyingCollectionCallback<O> callback, bool fireInitial)
        {
            _child.Subscribe(owner, callback, fireInitial);
        }

        public void Unset(NotifyingUnsetParameters? cmds)
        {
            SwapBack();
            _child.Unset(cmds);
        }

        public void Clear(NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.Clear(cmds);
        }

        public bool Remove(KeyValuePair<K, V> item, NotifyingFireParameters? cmds)
        {
            SwapOver();
            return _child.Remove(item, cmds);
        }

        public void SetTo(IEnumerable<KeyValuePair<K, V>> enumer, NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.SetTo(enumer, cmds);
        }

        public void Add(KeyValuePair<K, V> item, NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.Add(item, cmds);
        }

        public void Add(IEnumerable<KeyValuePair<K, V>> items, NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.Add(items, cmds);
        }

        public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, KeyValuePair<K, V>> callback, bool fireInitial)
        {
            _child.Subscribe_Enumerable(owner, callback, fireInitial);
        }

        public void Unsubscribe(object owner)
        {
            _child.Unsubscribe(owner);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return _child.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _child.GetEnumerator();
        }

        public bool TryGetValue(K key, out V val)
        {
            return _child.TryGetValue(key, out val);
        }
    }
}
