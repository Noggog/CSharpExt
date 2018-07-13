using Noggog;
using Noggog.Notifying;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Notifying
{
    public interface INotifyingKeyedCollectionGetter<K, V> : INotifyingDictionaryGetter<K, V>
    {
        IEnumerable<KeyValuePair<K, V>> KeyedValues { get; }
    }

    public interface INotifyingKeyedCollection<K, V> : INotifyingKeyedCollectionGetter<K, V>
    {
        Func<V, K> KeyGetter { get; }
        void Set(V val, NotifyingFireParameters cmds = null);
        void Set(IEnumerable<V> items, NotifyingFireParameters cmds = null);
        bool Remove(K key, NotifyingFireParameters cmds = null);
        void Unset(NotifyingUnsetParameters cmds = null);
        void Clear(NotifyingFireParameters cmds = null);
        bool Remove(V item, NotifyingFireParameters cmds = null);
        void SetTo(IEnumerable<V> enumer, NotifyingFireParameters cmds = null);
        new bool HasBeenSet { get; set; }
    }

    public class NotifyingKeyedCollection<K, V> : INotifyingKeyedCollection<K, V>
    {
        public Func<V, K> KeyGetter { get; private set; }
        private NotifyingDictionary<K, V> dict = new NotifyingDictionary<K, V>();

        public bool HasBeenSet
        {
            get { return dict.HasBeenSet; }
            set { dict.HasBeenSet = value; }
        }

        public INotifyingItemGetter<int> CountProperty => dict.CountProperty;
        public int Count => dict.Count;

        public ICollectionGetter<K> Keys => dict.Keys;
        public ICollectionGetter<V> Values => dict.Values;
        public IEnumerable<KeyValuePair<K, V>> KeyedValues => dict;

        IEnumerable<KeyValuePair<K, V>> IHasItemGetter<IEnumerable<KeyValuePair<K, V>>>.Item => dict;

        public V this[K key] => dict[key];

        public NotifyingKeyedCollection(Func<V, K> keyGetter)
        {
            this.KeyGetter = keyGetter;
        }

        public void Set(V val, NotifyingFireParameters cmds = null)
        {
            K key = KeyGetter(val);
            this.dict.Set(key, val, cmds);
        }

        public bool Remove(K key, NotifyingFireParameters cmds = null)
        {
            return this.dict.Remove(key, cmds);
        }

        public bool Remove(V val, NotifyingFireParameters cmds = null)
        {
            K key = KeyGetter(val);
            return this.dict.Remove(key, cmds);
        }

        public bool TryGetValue(K key, out V val)
        {
            return this.dict.TryGetValue(key, out val);
        }

        public void Subscribe<O>(O owner, NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>.NotifyingCollectionCallback<O> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.dict.Subscribe(owner, callback, cmds: cmds);
        }

        public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, KeyValuePair<K, V>> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.dict.Subscribe_Enumerable(owner, callback, cmds: cmds);
        }

        public void Unsubscribe(object owner)
        {
            this.dict.Unsubscribe(owner);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return this.dict.GetEnumerator();
        }

        public void Unset(NotifyingUnsetParameters cmds = null)
        {
            this.dict.Unset(cmds);
        }

        public void Clear(NotifyingFireParameters cmds = null)
        {
            this.dict.Clear(cmds);
        }

        public void SetTo(IEnumerable<V> enumer, NotifyingFireParameters cmds = null)
        {
            this.dict.SetTo(
                enumer.Select(
                    (v) => new KeyValuePair<K, V>(
                        KeyGetter(v),
                        v)),
                cmds);
        }

        public void Set(IEnumerable<V> items, NotifyingFireParameters cmds = null)
        {
            this.dict.Set(
                items.Select(
                    (v) => new KeyValuePair<K, V>(
                        KeyGetter(v),
                        v)),
                cmds);
        }

        public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, V> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.dict.Subscribe_Enumerable(owner, (o, c) => callback(o, c.Select((i) => new ChangeAddRem<V>(i.Item.Value, i.AddRem))), cmds: cmds);
        }

        public static bool ValuesEqual(INotifyingKeyedCollection<K, V> lhs, INotifyingKeyedCollection<K, V> rhs)
        {
            if (((INotifyingEnumerable<V>)lhs).CountProperty.Item != ((INotifyingEnumerable<V>)rhs).CountProperty.Item) return false;
            return lhs.Values.SequenceEqual(rhs.Values);
        }

        void ICollectionGetter<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<K, V>>)this.dict).CopyTo(array, arrayIndex);
        }

        void INotifyingDictionaryGetter<K, V>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<K, V>>)this.dict).CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return this.dict.Contains(item);
        }

        public bool ContainsKey(K key)
        {
            return this.dict.ContainsKey(key);
        }
    }
}

namespace System
{
    public static class INotifyingKeyedCollectionExt
    {
        public static void Set<K, V>(
            this INotifyingKeyedCollection<K, V> not,
            V val)
        {
            not.Set(val, NotifyingFireParameters.Typical);
        }

        public static void Set<K, V>(
            this INotifyingKeyedCollection<K, V> not,
            IEnumerable<V> vals)
        {
            not.Set(vals, NotifyingFireParameters.Typical);
        }

        public static bool Remove<K, V>(
            this INotifyingKeyedCollection<K, V> not,
            V val)
        {
            return not.Remove(val, NotifyingFireParameters.Typical);
        }

        public static bool Remove<K, V>(
            this INotifyingKeyedCollection<K, V> not,
            K key)
        {
            return not.Remove(key, NotifyingFireParameters.Typical);
        }

        public static void SetTo<K, V>(this INotifyingKeyedCollection<K, V> dict, IEnumerable<V> enumer)
        {
            dict.SetTo(enumer, NotifyingFireParameters.Typical);
        }

        public static void SetTo<K, V>(this INotifyingKeyedCollection<K, V> dict, IEnumerable<KeyValuePair<K, V>> enumer)
        {
            dict.SetTo(enumer.Select((kv) => kv.Value), NotifyingFireParameters.Typical);
        }

        public static void SetToWithDefault<K, V>(
            this INotifyingKeyedCollection<K, V> not,
            INotifyingKeyedCollectionGetter<K, V> rhs,
            INotifyingKeyedCollectionGetter<K, V> def,
            NotifyingFireParameters cmds)
        {
            if (rhs.HasBeenSet)
            {
                not.SetTo(rhs.Values, cmds);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(def.Values, cmds);
            }
            else
            {
                not.Unset(cmds.ToUnsetParams());
            }
        }

        public static void SetToWithDefault<K, V>(
            this INotifyingKeyedCollection<K, V> not,
            INotifyingKeyedCollectionGetter<K, V> rhs,
            INotifyingKeyedCollectionGetter<K, V> def,
            NotifyingFireParameters cmds,
            Func<V, V, V> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.SetTo(
                        rhs.Values.Select((t) => converter(t, default(V))),
                        cmds);
                }
                else
                {
                    not.SetTo(
                        rhs.Values.Select((t) => converter(t, def[not.KeyGetter(t)])),
                        cmds);
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(
                    def.Values.Select((t) => converter(t, default(V))),
                    cmds);
            }
            else
            {
                not.Unset(cmds.ToUnsetParams());
            }
        }

        public static void SetIfSucceeded<K, V>(
            this INotifyingKeyedCollection<K, V> not,
            TryGet<IEnumerable<V>> tryGet,
            NotifyingFireParameters cmds = null)
        {
            if (tryGet.Failed) return;
            not.SetTo(tryGet.Value, cmds);
        }

        public static void SetIfSucceededOrDefault<K, V>(
            this INotifyingKeyedCollection<K, V> not,
            TryGet<IEnumerable<V>> tryGet,
            NotifyingFireParameters cmds = null)
        {
            if (tryGet.Failed)
            {
                not.Unset();
                return;
            }
            not.SetTo(tryGet.Value, cmds);
        }
    }
}