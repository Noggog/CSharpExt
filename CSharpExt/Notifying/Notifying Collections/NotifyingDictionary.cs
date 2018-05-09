using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Noggog.Notifying
{
    public interface INotifyingDictionaryGetter<K, V> : INotifyingEnumerable<KeyValuePair<K, V>>, IDictionaryGetter<K, V>
    {
        new int Count { get; }
        new bool Contains(KeyValuePair<K, V> item);
        new void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex);
        void Subscribe<O>(O owner, NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>.NotifyingCollectionCallback<O> callback, NotifyingSubscribeParameters cmds = null);
    }

    public interface INotifyingDictionary<K, V> : INotifyingDictionaryGetter<K, V>, INotifyingCollection<KeyValuePair<K, V>>, IDictionary<K, V>
    {
        new ICollectionGetter<K> Keys { get; }
        new ICollectionGetter<V> Values { get; }
        new int Count { get; }
        new V this[K key] { get; set; }
        void Set(K key, V val, NotifyingFireParameters cmds = null);
        bool Remove(K key, NotifyingFireParameters cmds = null);
        new bool TryGetValue(K key, out V val);
    }

    public class NotifyingDictionary<K, V> : NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>, INotifyingDictionary<K, V>
    {
        protected NotifyingItem<int> _count = new NotifyingItem<int>();
        public INotifyingItemGetter<int> CountProperty => _count;
        public int Count => _count.Item;
        private readonly Dictionary<K, V> dict = new Dictionary<K, V>();
        private readonly Func<V, V> valConv;

        public IEnumerable<KeyValuePair<K, V>> Dict => dict;
        public ICollectionGetter<K> Keys => new CollectionGetterWrapper<K>(dict.Keys);
        public ICollectionGetter<V> Values => new CollectionGetterWrapper<V>(dict.Values);

        IEnumerable<KeyValuePair<K, V>> IHasItemGetter<IEnumerable<KeyValuePair<K, V>>>.Item => dict;

        public NotifyingDictionary(
            Func<V, V> valConv = null)
        {
            this.valConv = valConv ?? ((i) => i);
        }

        public V this[K key]
        {
            get => dict[key];
            set => Set(key, value);
        }

        public void Set(K key, V item, NotifyingFireParameters cmds = null)
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

        public void Set(IEnumerable<KeyValuePair<K, V>> items, NotifyingFireParameters cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                List<ChangeKeyed<K, V>> changes = new List<ChangeKeyed<K, V>>();
                foreach (var item in items)
                {
                    if (dict.TryGetValue(item.Key, out V oldVal))
                    {
                        changes.Add(
                            new ChangeKeyed<K, V>(
                                item.Key,
                                oldVal,
                                item.Value,
                                AddRemoveModify.Modify));
                    }
                    else
                    {
                        changes.Add(
                            new ChangeKeyed<K, V>(
                                item.Key,
                                default(V),
                                item.Value,
                                AddRemoveModify.Add));
                    }
                    dict[item.Key] = valConv(item.Value);
                }
                _count.Set(dict.Count, cmds);
                FireChange(changes, cmds);
            }
            else
            {
                foreach (var item in items)
                {
                    dict[item.Key] = valConv(item.Value);
                }
            }
        }

        public bool Remove(K key, NotifyingFireParameters cmds = null)
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

        public void SetTo(IEnumerable<KeyValuePair<K, V>> items, NotifyingFireParameters cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                List<ChangeKeyed<K, V>> changes = new List<ChangeKeyed<K, V>>();
                HashSet<K> set = new HashSet<K>();
                set.Add(this.dict.Keys);
                foreach (var item in items)
                {
                    if (dict.TryGetValue(item.Key, out V oldVal))
                    {
                        changes.Add(
                            new ChangeKeyed<K, V>(
                                item.Key,
                                oldVal,
                                item.Value,
                                AddRemoveModify.Modify));
                    }
                    else
                    {
                        changes.Add(
                            new ChangeKeyed<K, V>(
                                item.Key,
                                default(V),
                                item.Value,
                                AddRemoveModify.Add));
                    }
                    dict[item.Key] = valConv(item.Value);
                    set.Remove(item.Key);
                }

                foreach (var toRem in set)
                {
                    if (dict.TryGetValue(toRem, out V oldVal))
                    {
                        dict.Remove(toRem);
                        changes.Add(
                            new ChangeKeyed<K, V>(
                                toRem,
                                oldVal,
                                default(V),
                                AddRemoveModify.Remove));
                    }
                }
                _count.Set(dict.Count, cmds);
                FireChange(changes, cmds);
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

        public void Clear(NotifyingFireParameters cmds = null)
        {
            cmds = ProcessCmds(cmds);

            if (this.dict.Count == 0 && !cmds.ForceFire) return;

            if (HasSubscribers())
            { // Will be firing
                var changes = new List<ChangeKeyed<K, V>>(dict.Count);
                foreach (var item in dict)
                {
                    changes.Add(
                        new ChangeKeyed<K, V>(
                            item.Key,
                            item.Value,
                            default(V),
                            AddRemoveModify.Remove));
                }

                dict.Clear();
                _count.Set(0, cmds);
                FireChange(changes, cmds);
            }
            else
            { // just internals
                dict.Clear();
                _count.Set(0, cmds);
            }
        }

        public void Unset(NotifyingUnsetParameters cmds = null)
        {
            HasBeenSet = false;
            Clear(cmds.ToFireParams());
        }

        protected override ICollection<ChangeKeyed<K, V>> CompileCurrent()
        {
            var changes = new List<ChangeKeyed<K, V>>(dict.Count);
            foreach (var item in dict)
            {
                changes.Add(
                    new ChangeKeyed<K, V>(
                        item.Key,
                        default(V),
                        item.Value,
                        AddRemoveModify.Add));
            }
            return changes;
        }

        [DebuggerStepThrough]
        public void Subscribe<O>(O owner, NotifyingCollectionCallback<O> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Subscribe_Internal(
                owner: owner, 
                callback: (own, change) => callback((O)own, change), 
                cmds: cmds);
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

        protected override ICollection<ChangeAddRem<KeyValuePair<K, V>>> CompileCurrentEnumer()
        {
            var changes = new List<ChangeAddRem<KeyValuePair<K, V>>>(dict.Count);
            foreach (var item in dict)
            {
                changes.Add(
                    new ChangeAddRem<KeyValuePair<K, V>>(
                        item,
                        AddRemove.Add));
            }
            return changes;
        }

        protected void FireChange(IEnumerable<ChangeKeyed<K, V>> changes, NotifyingFireParameters cmds)
        {
            List<Exception> exceptions = null;

            if (this.subscribers != null)
            {
                foreach (var sub in this.subscribers.GetSubs())
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

            if (this.enumerSubscribers != null)
            {
                if (this.enumerSubscribers.HasSubs)
                {
                    var enumerChanges = new List<ChangeAddRem<KeyValuePair<K, V>>>();
                    foreach (var change in changes)
                    {
                        switch (change.AddRem)
                        {
                            case AddRemoveModify.Add:
                                enumerChanges.Add(new ChangeAddRem<KeyValuePair<K, V>>(new KeyValuePair<K, V>(change.Key, change.New), AddRemove.Add));
                                break;
                            case AddRemoveModify.Remove:
                                enumerChanges.Add(new ChangeAddRem<KeyValuePair<K, V>>(new KeyValuePair<K, V>(change.Key, change.Old), AddRemove.Remove));
                                break;
                            case AddRemoveModify.Modify:
                                enumerChanges.Add(new ChangeAddRem<KeyValuePair<K, V>>(new KeyValuePair<K, V>(change.Key, change.Old), AddRemove.Remove));
                                enumerChanges.Add(new ChangeAddRem<KeyValuePair<K, V>>(new KeyValuePair<K, V>(change.Key, change.New), AddRemove.Add));
                                break;
                            default:
                                break;
                        }
                    }
                    foreach (var sub in this.enumerSubscribers.GetSubs())
                    {
                        foreach (var eventItem in sub.Value)
                        {
                            try
                            {
                                eventItem(sub.Key, enumerChanges);
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
                    cmds.ExceptionHandler(ex);
                }
            }
        }

        bool INotifyingCollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item, NotifyingFireParameters cmds)
        {
            return this.Remove(item.Key, cmds);
        }

        void INotifyingCollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item, NotifyingFireParameters cmds)
        {
            this.Set(item.Key, item.Value, cmds);
        }

        void INotifyingCollection<KeyValuePair<K, V>>.Add(IEnumerable<KeyValuePair<K, V>> items, NotifyingFireParameters cmds)
        {
            this.Set(items, cmds);
        }

        public bool TryGetValue(K key, out V val)
        {
            return this.dict.TryGetValue(key, out val);
        }

        #region IDictionary
        bool ICollection<KeyValuePair<K, V>>.IsReadOnly => false;

        ICollection<K> IDictionary<K, V>.Keys => this.dict.Keys;

        ICollection<V> IDictionary<K, V>.Values => this.dict.Values;

        public bool ContainsKey(K key)
        {
            return dict.ContainsKey(key);
        }

        void IDictionary<K, V>.Add(K key, V value)
        {
            if (this.ContainsKey(key))
            {
                throw new ArgumentException("Dictionary already contained key.");
            }
            this.Set(key, value);
        }

        bool IDictionary<K, V>.Remove(K key)
        {
            return this.Remove(key);
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
        {
            ((IDictionary<K, V>)this).Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<K, V>>.Clear()
        {
            this.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return this.dict.Contains(item);
        }

        void ICollectionGetter<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<K, V>>)this.dict).CopyTo(array, arrayIndex);
        }

        void INotifyingDictionaryGetter<K, V>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<K, V>>)this.dict).CopyTo(array, arrayIndex);
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<K, V>>)this.dict).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        {
            return this.Remove(item.Key);
        }
        #endregion
    }

    public static class INotifyingDictionaryGetterExt
    {
        public static void Subscribe<O, K, V>(this INotifyingDictionaryGetter<K, V> getter, O owner, NotifyingCollection<KeyValuePair<K, V>, ChangeKeyed<K, V>>.NotifyingCollectionSimpleCallback callback, NotifyingSubscribeParameters cmds = null)
        {
            getter.Subscribe(owner, (o2, ch) => callback(ch), cmds);
        }

        public static void Set<K, V>(this INotifyingDictionary<K, V> getter, K key, V val)
        {
            getter.Set(key, val, null);
        }

        public static void Remove<K, V>(this INotifyingDictionary<K, V> getter, K key)
        {
            getter.Remove(key, null);
        }

        public static V TryGetValue<K, V>(this INotifyingDictionaryGetter<K, V> dict, K key)
        {
            dict.TryGetValue(key, out V val);
            return val;
        }

        public static void SetToWithDefault<K, V>(
            this INotifyingDictionary<K, V> not,
            IHasBeenSetItemGetter<IEnumerable<KeyValuePair<K, V>>> rhs,
            INotifyingDictionaryGetter<K, V> def,
            NotifyingFireParameters cmds,
            Func<K, V, V, KeyValuePair<K, V>> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.SetTo(
                        rhs.Item.Select((t) => converter(t.Key, t.Value, default(V))),
                        cmds);
                }
                else
                {
                    not.SetTo(
                        rhs.Item.Select((t) => converter(t.Key, t.Value, def.TryGetValue(t.Key))),
                        cmds);
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(
                    def.Item.Select((t) => converter(t.Key, t.Value, default(V))),
                    cmds);
            }
            else
            {
                not.Unset(cmds.ToUnsetParams());
            }
        }
    }
}