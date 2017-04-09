using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Notifying
{
    public interface INotifyingKeyedCollectionGetter<K, V> : INotifyingDictionaryGetter<K, V>, INotifyingCollection<V>
    {
        IEnumerable<KeyValuePair<K, V>> KeyedValues { get; }
        new bool HasBeenSet { get; }
    }

    public interface INotifyingKeyedCollection<K, V> : INotifyingKeyedCollectionGetter<K, V>
    {
        void Set(V val, NotifyingFireParameters? cmds);
        void Remove(K key, NotifyingFireParameters? cmds);
    }

    public class NotifyingKeyedCollection<K, V> : INotifyingKeyedCollection<K, V>
    {
        private Func<V, K> keyGetter;
        private NotifyingDictionary<K, V> dict = new NotifyingDictionary<K, V>();

        public bool HasBeenSet
        {
            get { return dict.HasBeenSet; }
            set { dict.HasBeenSet = value; }
        }

        public INotifyingItemGetter<int> Count => dict.Count;

        bool INotifyingEnumerable<KeyValuePair<K, V>>.HasBeenSet => dict.HasBeenSet;

        public IEnumerable<K> Keys => dict.Keys;
        public IEnumerable<V> Values => dict.Values;
        public IEnumerable<KeyValuePair<K, V>> KeyedValues => dict;
        public V this[K key] => dict[key];

        public NotifyingKeyedCollection(Func<V, K> keyGetter)
        {
            this.keyGetter = keyGetter;
        }

        public void Set(V val, NotifyingFireParameters? cmds)
        {
            K key = keyGetter(val);
            this.dict.Set(key, val, cmds);
        }

        public void Remove(K key, NotifyingFireParameters? cmds)
        {
            this.dict.Remove(key, cmds);
        }

        public bool Remove(V val, NotifyingFireParameters? cmds)
        {
            K key = keyGetter(val);
            return this.dict.Remove(key, cmds);
        }

        public bool TryGetValue(K key, out V val)
        {
            return this.dict.TryGetValue(key, out val);
        }

        public void Subscribe<O>(O owner, NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>.NotifyingCollectionCallback<O> callback, bool fireInitial)
        {
            this.dict.Subscribe(owner, callback, fireInitial);
        }

        public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, KeyValuePair<K, V>> callback, bool fireInitial)
        {
            this.dict.Subscribe_Enumerable(owner, callback, fireInitial);
        }

        public void Unsubscribe(object owner)
        {
            this.dict.Unsubscribe(owner);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return this.dict.GetEnumerator();
        }

        public void Unset(NotifyingUnsetParameters? cmds)
        {
            this.dict.Unset(cmds);
        }

        public void Clear(NotifyingFireParameters? cmds)
        {
            this.dict.Clear(cmds);
        }

        public void SetTo(IEnumerable<V> enumer, NotifyingFireParameters? cmds)
        {
            this.dict.SetTo(
                enumer.Select(
                    (v) => new KeyValuePair<K, V>(
                        keyGetter(v),
                        v)),
                cmds);
        }

        public void Add(V item, NotifyingFireParameters? cmds)
        {
            K key = keyGetter(item);
            this.dict.Set(key, item, cmds);
        }

        public void Add(IEnumerable<V> items, NotifyingFireParameters? cmds)
        {
            this.dict.Set(
                items.Select(
                    (v) => new KeyValuePair<K, V>(
                        keyGetter(v),
                        v)),
                cmds);
        }

        public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, V> callback, bool fireInitial)
        {
            this.dict.Subscribe_Enumerable(owner, (o, c) => callback(o, c.Select((i) => new ChangeAddRem<V>(i.Item.Value, i.AddRem))), fireInitial);
        }

        public static bool ValuesEqual(INotifyingKeyedCollection<K, V> lhs, INotifyingKeyedCollection<K, V> rhs)
        {
            if (((INotifyingEnumerable<V>)lhs).Count.Value != ((INotifyingEnumerable<V>)rhs).Count.Value) return false;
            return lhs.Values.SequenceEqual(rhs.Values);
        }

        IEnumerator<V> IEnumerable<V>.GetEnumerator()
        {
            return this.dict.Select((kv) => kv.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
