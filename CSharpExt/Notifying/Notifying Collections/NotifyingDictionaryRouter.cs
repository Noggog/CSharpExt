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

        public ICollection<K> Keys => _child.Keys;

        public ICollection<V> Values => _child.Values;

        IEnumerable<KeyValuePair<K, V>> IHasItemGetter<IEnumerable<KeyValuePair<K, V>>>.Item => _child.Item;

        public bool IsReadOnly => _child.IsReadOnly;

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

        public void Set(K key, V val, NotifyingFireParameters cmds = null)
        {
            SwapOver();
            _child.Set(key, val, cmds);
        }

        public bool Remove(K key, NotifyingFireParameters cmds = null)
        {
            SwapOver();
            return _child.Remove(key, cmds);
        }

        public void Subscribe<O>(O owner, NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>.NotifyingCollectionCallback<O> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe(owner, callback, cmds);
        }

        public void Unset(NotifyingUnsetParameters cmds = null)
        {
            SwapBack();
            _child.Unset(cmds);
        }

        public void Clear(NotifyingFireParameters cmds = null)
        {
            SwapOver();
            _child.Clear(cmds);
        }

        public bool Remove(KeyValuePair<K, V> item, NotifyingFireParameters cmds = null)
        {
            SwapOver();
            return _child.Remove(item, cmds);
        }

        public void SetTo(IEnumerable<KeyValuePair<K, V>> enumer, NotifyingFireParameters cmds = null)
        {
            SwapOver();
            _child.SetTo(enumer, cmds);
        }

        public void Add(KeyValuePair<K, V> item, NotifyingFireParameters cmds = null)
        {
            SwapOver();
            _child.Add(item, cmds);
        }

        public void Add(IEnumerable<KeyValuePair<K, V>> items, NotifyingFireParameters cmds = null)
        {
            SwapOver();
            _child.Add(items, cmds);
        }

        public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, KeyValuePair<K, V>> callback, NotifyingSubscribeParameters cmds = null)
        {
            _child.Subscribe_Enumerable(owner, callback, cmds: cmds);
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

        #region IDictionary

        public bool ContainsKey(K key)
        {
            return _child.ContainsKey(key);
        }

        void IDictionary<K, V>.Add(K key, V value)
        {
            SwapOver();
            _child.Add(key, value);
        }

        bool IDictionary<K, V>.Remove(K key)
        {
            SwapOver();
            return _child.Remove(key);
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
        {
            SwapOver();
            _child.Add(item);
        }

        void ICollection<KeyValuePair<K, V>>.Clear()
        {
            SwapOver();
            _child.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return _child.Contains(item);
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            _child.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        {
            SwapOver();
            return _child.Remove(item);
        }
        #endregion
    }
}
