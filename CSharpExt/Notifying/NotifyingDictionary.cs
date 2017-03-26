using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;

namespace Noggog.Notifying
{
    public interface INotifyingDictionaryGetter<K, V> : INotifyingEnumerable<KeyValuePair<K, V>>
    {
        IEnumerable<K> Keys { get; }
        IEnumerable<V> Values { get; }
        V this[K key] { get; }
        bool TryGetValue(K key, out V val);
        void Subscribe<O>(O owner, NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>.NotifyingCollectionCallback<O> callback, bool fireInitial);
    }

    public interface INotifyingDictionary<K, V> : INotifyingDictionaryGetter<K, V>, INotifyingCollection<KeyValuePair<K, V>>
    {
        new V this[K key] { get; set; }
        void Set(K key, V val, NotifyingFireParameters? cmds);
        void Remove(K key, NotifyingFireParameters? cmds);
    }

    public class NotifyingDictionary<K, V> : NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>, INotifyingDictionary<K, V>
    {
        protected static ObjectDictionaryPool<K, V> pool = new ObjectDictionaryPool<K, V>(100);
        protected static ObjectPool<HashSet<K>> setToPool = new ObjectPool<HashSet<K>>(() => new HashSet<K>(), maxInstances: 100);
        protected static ObjectListPool<ChangeKeyed<K, V>> firePool = new ObjectListPool<ChangeKeyed<K, V>>(200);

        protected NotifyingItem<int> _count = new NotifyingItem<int>();
        public INotifyingItemGetter<int> Count { get { return _count; } }
        private Dictionary<K, V> dict = pool.Get();
        private Func<V, V> valConv;

        public IEnumerable<KeyValuePair<K, V>> Dict { get { return dict; } }
        public IEnumerable<K> Keys { get { return dict.Keys; } }
        public IEnumerable<V> Values { get { return dict.Values; } }

        public NotifyingDictionary(
            Func<V, V> valConv = null)
        {
            this.valConv = valConv ?? ((i) => i);
        }

        ~NotifyingDictionary()
        {
            if (dict != null)
            {
                pool.Return(dict);
                dict = null;
            }
        }

        public V this[K key]
        {
            get
            {
                return dict[key];
            }
            set
            {
                Set(key, value);
            }
        }

        public void Set(K key, V item, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                var prevCount = dict.Count;
                if (!dict.TryGetValue(key, out V old))
                {
                    old = default(V);
                }
                dict[key] = valConv(item);
                _count.Set(dict.Count, cmds);
                FireChange(
                    new ChangeKeyed<K, V>(
                        key,
                        old,
                        item,
                        prevCount == dict.Count ? AddRemoveModify.Modify : AddRemoveModify.Add).Single(),
                    cmds);
            }
            else
            {
                dict[key] = valConv(item);
                _count.Set(dict.Count, cmds);
            }
        }

        public void Set(IEnumerable<KeyValuePair<K, V>> items, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                using (var changes = firePool.Checkout())
                {
                    foreach (var item in items)
                    {
                        if (dict.TryGetValue(item.Key, out V oldVal))
                        {
                            changes.Item.Add(
                                new ChangeKeyed<K, V>(
                                    item.Key,
                                    oldVal,
                                    item.Value,
                                    AddRemoveModify.Modify));
                        }
                        else
                        {
                            changes.Item.Add(
                                new ChangeKeyed<K, V>(
                                    item.Key,
                                    default(V),
                                    item.Value,
                                    AddRemoveModify.Add));
                        }
                        dict[item.Key] = valConv(item.Value);
                    }
                    _count.Set(dict.Count, cmds);
                    FireChange(changes.Item, cmds);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    dict[item.Key] = valConv(item.Value);
                }
            }
        }

        public bool Remove(K key, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                if (!dict.TryGetValue(key, out V old))
                {
                    old = default(V);
                }
                if (dict.Remove(key))
                {
                    _count.Set(dict.Count, cmds);
                    if (!HasSubscribers()) return true;
                    FireChange(
                        new ChangeKeyed<K, V>(
                            key,
                            old,
                            default(V),
                            AddRemoveModify.Remove).Single(),
                        cmds);
                    return true;
                }
                return false;
            }
            else
            {
                if (dict.Remove(key))
                {
                    _count.Set(dict.Count, cmds);
                    return true;
                }
                return false;
            }
        }

        void INotifyingDictionary<K, V>.Remove(K key, NotifyingFireParameters? cmds)
        {
            this.Remove(key, cmds);
        }

        public void SetTo(IEnumerable<KeyValuePair<K, V>> items, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                using (var changes = firePool.Checkout())
                {
                    using (var set = setToPool.Checkout())
                    {
                        set.Item.Add(this.dict.Keys);
                        foreach (var item in items)
                        {
                            if (dict.TryGetValue(item.Key, out V oldVal))
                            {
                                changes.Item.Add(
                                    new ChangeKeyed<K, V>(
                                        item.Key,
                                        oldVal,
                                        item.Value,
                                        AddRemoveModify.Modify));
                            }
                            else
                            {
                                changes.Item.Add(
                                    new ChangeKeyed<K, V>(
                                        item.Key,
                                        default(V),
                                        item.Value,
                                        AddRemoveModify.Add));
                            }
                            dict[item.Key] = valConv(item.Value);
                            set.Item.Remove(item.Key);
                        }

                        foreach (var toRem in set.Item)
                        {
                            if (dict.TryGetValue(toRem, out V oldVal))
                            {
                                dict.Remove(toRem);
                                changes.Item.Add(
                                    new ChangeKeyed<K, V>(
                                        toRem,
                                        oldVal,
                                        default(V),
                                        AddRemoveModify.Remove));
                            }
                        }
                    }
                    _count.Set(dict.Count, cmds);
                    FireChange(changes.Item, cmds);
                }
            }
            else
            {
                dict.Clear();
                foreach (var item in items)
                {
                    dict[item.Key] = valConv(item.Value);
                }
                _count.Set(dict.Count, cmds);
            }
        }

        public void Clear(NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);

            if (this.dict.Count == 0 && !cmds.Value.ForceFire) return;

            if (HasSubscribers())
            { // Will be firing
                using (var changes = firePool.Checkout())
                {
                    foreach (var item in dict)
                    {
                        changes.Item.Add(
                            new ChangeKeyed<K, V>(
                                item.Key,
                                item.Value,
                                default(V),
                                AddRemoveModify.Remove));
                    }

                    dict.Clear();
                    _count.Set(0, cmds);
                    FireChange(changes.Item, cmds);
                }
            }
            else
            { // just internals
                dict.Clear();
                _count.Set(0, cmds);
            }
        }

        public void Unset(NotifyingUnsetParameters? cmds = null)
        {
            HasBeenSet = false;
            Clear(cmds.ToFireParams());
        }

        protected override ObjectPoolCheckout<List<ChangeKeyed<K, V>>> CompileCurrent()
        {
            var changes = firePool.Checkout();
            foreach (var item in dict)
            {
                changes.Item.Add(
                    new ChangeKeyed<K, V>(
                        item.Key,
                        default(V),
                        item.Value,
                        AddRemoveModify.Add));
            }
            return changes;
        }

        public void Subscribe<O>(O owner, NotifyingCollectionCallback<O> callback, bool fireInitial)
        {
            this.Subscribe_Internal(owner, callback, fireInitial);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerable<KeyValuePair<K, V>> Iterate()
        {
            return this;
        }

        protected override ObjectPoolCheckout<List<ChangeAddRem<KeyValuePair<K, V>>>> CompileCurrentEnumer()
        {
            var changes = fireEnumerPool.Checkout();
            foreach (var item in dict)
            {
                changes.Item.Add(
                    new ChangeAddRem<KeyValuePair<K, V>>(
                        item,
                        AddRemove.Add));
            }
            return changes;
        }

        protected void FireChange(IEnumerable<ChangeKeyed<K, V>> changes, NotifyingFireParameters? cmds)
        {
            List<Exception> exceptions = null;

            using (var fireSubscribers = this.subscribers.GetSubs())
            {
                foreach (var sub in fireSubscribers)
                {
                    foreach (var eventItem in sub.Value)
                    {
                        try
                        {
                            eventItem(sub.Key, changes);
                        }
                        catch (Exception ex)
                        {
                            if (exceptions == null)
                            {
                                exceptions = new List<Exception>();
                            }
                            exceptions.Add(ex);
                        }
                    }
                }
            }

            if (this.enumerSubscribers.HasSubs)
            {
                using (var enumerChanges = fireEnumerPool.Checkout())
                {
                    foreach (var change in changes)
                    {
                        switch (change.AddRem)
                        {
                            case AddRemoveModify.Add:
                                enumerChanges.Item.Add(new ChangeAddRem<KeyValuePair<K, V>>(new KeyValuePair<K, V>(change.Key, change.New), AddRemove.Add));
                                break;
                            case AddRemoveModify.Remove:
                                enumerChanges.Item.Add(new ChangeAddRem<KeyValuePair<K, V>>(new KeyValuePair<K, V>(change.Key, change.Old), AddRemove.Remove));
                                break;
                            case AddRemoveModify.Modify:
                                enumerChanges.Item.Add(new ChangeAddRem<KeyValuePair<K, V>>(new KeyValuePair<K, V>(change.Key, change.Old), AddRemove.Remove));
                                enumerChanges.Item.Add(new ChangeAddRem<KeyValuePair<K, V>>(new KeyValuePair<K, V>(change.Key, change.New), AddRemove.Add));
                                break;
                            default:
                                break;
                        }
                    }
                    using (var fireSubscribers = this.enumerSubscribers.GetSubs())
                    {
                        foreach (var sub in fireSubscribers)
                        {
                            foreach (var eventItem in sub.Value)
                            {
                                try
                                {
                                    eventItem(sub.Key, enumerChanges.Item);
                                }
                                catch (Exception ex)
                                {
                                    if (exceptions == null)
                                    {
                                        exceptions = new List<Exception>();
                                    }
                                    exceptions.Add(ex);
                                }
                            }
                        }
                    }
                }
            }

            if (exceptions != null
                && exceptions.Count > 0)
            {
                Exception ex;
                if (exceptions.Count == 1)
                {
                    ex = exceptions[0];
                }
                else
                {
                    ex = new AggregateException(exceptions.ToArray());
                }

                if (cmds?.ExceptionHandler == null)
                {
                    throw ex;
                }
                else
                {
                    cmds.Value.ExceptionHandler(ex);
                }
            }
        }

        bool INotifyingCollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item, NotifyingFireParameters? cmds)
        {
            return this.Remove(item.Key, cmds);
        }

        void INotifyingCollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item, NotifyingFireParameters? cmds)
        {
            this.Set(item.Key, item.Value, cmds);
        }

        void INotifyingCollection<KeyValuePair<K, V>>.Add(IEnumerable<KeyValuePair<K, V>> items, NotifyingFireParameters? cmds)
        {
            this.Set(items, cmds);
        }

        public bool TryGetValue(K key, out V val)
        {
            return this.dict.TryGetValue(key, out val);
        }
    }

    public static class INotifyingDictionaryGetterExt
    {
        public static void Subscribe<O, K, V>(this INotifyingDictionaryGetter<K, V> getter, O owner, NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>.NotifyingCollectionCallback<O> callback)
        {
            getter.Subscribe(owner, callback, true);
        }

        public static void Subscribe<O, K, V>(this INotifyingDictionaryGetter<K, V> getter, O owner, NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>.NotifyingCollectionSimpleCallback callback, bool fireInitial = true)
        {
            getter.Subscribe(owner, (o2, ch) => callback(ch), fireInitial);
        }

        public static void Set<K, V>(this INotifyingDictionary<K, V> getter, K key, V val)
        {
            getter.Set(key, val, null);
        }

        public static void Remove<K, V>(this INotifyingDictionary<K, V> getter, K key)
        {
            getter.Remove(key, null);
        }
    }
}