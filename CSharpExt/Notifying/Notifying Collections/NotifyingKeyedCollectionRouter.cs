using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog.Notifying
{
    public class NotifyingKeyedCollectionRouter<K, V> : INotifyingKeyedCollection<K, V>
    {
        INotifyingKeyedCollectionGetter<K, V> _base;
        INotifyingKeyedCollection<K, V> _child;

        public bool HasBeenSwapped { get; private set; }

        public INotifyingItemGetter<int> Count => ((INotifyingCollection<KeyValuePair<K, V>>)_child).Count;

        public bool HasBeenSet
        {
            get
            {
                return (HasBeenSwapped ? ((INotifyingEnumerable<V>)_child).HasBeenSet : ((INotifyingEnumerable<V>)_base).HasBeenSet);
            }
            set
            {
                SwapOver();
                ((INotifyingCollection<KeyValuePair<K, V>>)_child).HasBeenSet = value;
            }
        }

        public IEnumerable<KeyValuePair<K, V>> KeyedValues => _child.KeyedValues;

        public IEnumerable<K> Keys => _child.Keys;

        public IEnumerable<V> Values => _child.Values;

        public IEnumerable<KeyValuePair<K, V>> Value => _child.Value;

        public Func<V, K> KeyGetter => _child.KeyGetter;

        V INotifyingDictionaryGetter<K, V>.this[K key] => _child[key];

        public V this[K key] => _child[key];

        public NotifyingKeyedCollectionRouter(
            INotifyingKeyedCollectionGetter<K, V> _base,
            INotifyingKeyedCollection<K, V> _child)
        {
            this._base = _base;
            this._child = _child;
        }
        
        private void SwapOver()
        {
            if (HasBeenSwapped) return;
            ((INotifyingCollection<KeyValuePair<K, V>>)_base).Unsubscribe(this);
        }

        private void SwapBack(NotifyingUnsetParameters? cmds)
        {
            if (!HasBeenSwapped) return;
            _base.Subscribe(
                this,
                (changes) =>
                {
                    this._child.SetTo(_base.Values, cmds.ToFireParams());
                });
        }

        public void Set(V val, NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.Set(val, cmds);
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
            SwapBack(cmds);
            _child.Unset(cmds);
        }

        public void Clear(NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.Clear(cmds);
        }

        public bool TryGetValue(K key, out V val)
        {
            return _child.TryGetValue(key, out val);
        }

        public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, KeyValuePair<K, V>> callback, bool fireInitial)
        {
            _child.Subscribe_Enumerable<O>(owner, callback, fireInitial);
        }

        public void Unsubscribe(object owner)
        {
            ((INotifyingCollection<KeyValuePair<K, V>>)_child).Unsubscribe(owner);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return ((INotifyingCollection<KeyValuePair<K, V>>)_child).GetEnumerator();
        }

        public bool Remove(V item, NotifyingFireParameters? cmds)
        {
            SwapOver();
            return _child.Remove(item, cmds);
        }

        public void SetTo(IEnumerable<V> enumer, NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.SetTo(enumer, cmds);
        }

        public void Add(V item, NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.Add(item, cmds);
        }

        public void Add(IEnumerable<V> items, NotifyingFireParameters? cmds)
        {
            SwapOver();
            _child.Add(items, cmds);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
