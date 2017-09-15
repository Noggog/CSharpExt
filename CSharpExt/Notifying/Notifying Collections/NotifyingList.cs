using Noggog;
using Noggog.Containers.Pools;
using Noggog.Notifying;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Noggog.Notifying
{
    public interface INotifyingListGetter<T> : INotifyingEnumerable<T>
    {
        T this[int index] { get; }
        int IndexOf(T item);
        void Subscribe<O>(O owner, NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionCallback<O> callback, bool fireInitial);
        void Subscribe(NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionSimpleCallback callback, bool fireInitial);
    }

    public interface INotifyingList<T> : INotifyingListGetter<T>, INotifyingCollection<T>
    {
        new T this[int index] { get; set; }
        void Insert(int index, T item, NotifyingFireParameters? cmds);
        void Set(int index, T item, NotifyingFireParameters? cmds);
        void RemoveAt(int index, NotifyingFireParameters? cmds);
    }

    public class NotifyingList<T> : NotifyingCollection<T, ChangeIndex<T>>, INotifyingList<T>
    {
        protected static ObjectListPool<T> pool = new ObjectListPool<T>(100);

        protected static ObjectListPool<ChangeIndex<T>> firePool = new ObjectListPool<ChangeIndex<T>>(200);

        protected NotifyingItem<int> _count = new NotifyingItem<int>();

        public INotifyingItemGetter<int> CountProperty => _count;
        public int Count => _count.Item;

        private List<T> list = pool.Get();

        public IEnumerable<T> List => list;

        ~NotifyingList()
        {
            if (list == null)
            {
                pool.Return(list);
                list = null;
            }
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                Set(index, value);
            }
        }

        public void Set(int index, T item, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                var prevCount = list.Count;
                T old;
                if (index < this.list.Count)
                {
                    old = this.list[index];
                }
                else
                {
                    old = default(T);
                }
                this.list[index] = item;
                _count.Set(list.Count, cmds);
                if (!HasSubscribers()) return;
                FireChange(
                    new ChangeIndex<T>(
                        old,
                        item,
                        prevCount == list.Count ? AddRemoveModify.Modify : AddRemoveModify.Add,
                        index).Single(),
                    cmds);
            }
            else
            {
                this.list[index] = item;
                _count.Set(list.Count, cmds);
            }
        }

        public void Insert(int index, T item, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            list.Insert(index, item);
            _count.Set(_count.Item + 1, cmds);
            if (!HasSubscribers()) return;
            FireChange(
                new ChangeIndex<T>(default(T), item, AddRemoveModify.Add, index).Single(),
                cmds);
        }

        public void Add(T item, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            list.Add(item);
            _count.Set(list.Count, cmds);
            if (!HasSubscribers()) return;
            FireChange(
                new ChangeIndex<T>(default(T), item, AddRemoveModify.Add, list.Count - 1).Single(),
                cmds);
        }

        public void Add(IEnumerable<T> items, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                list.AddRange(items);
                using (var changes = firePool.Checkout())
                {
                    int curCount = list.Count;
                    foreach (var item in items)
                    {
                        changes.Item.Add(new ChangeIndex<T>(default(T), item, AddRemoveModify.Add, curCount++));
                    }
                    _count.Set(list.Count, cmds);
                    FireChange(changes.Item, cmds);
                }
            }
            else
            {
                list.AddRange(items);
                _count.Set(list.Count, cmds);
            }
        }

        public void RemoveAt(int index, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            list.RemoveAt(index, out T item);
            _count.Set(list.Count, cmds);
            if (!HasSubscribers()) return;
            FireChange(
                new ChangeIndex<T>[]
                {
                    new ChangeIndex<T>(item, default(T), AddRemoveModify.Remove, index)
                },
                cmds);
        }

        public bool Remove(T item, NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (list.Remove(item, out int index))
            {
                _count.Set(list.Count, cmds);
                if (!HasSubscribers()) return true;
                FireChange(
                    new ChangeIndex<T>(item, default(T), AddRemoveModify.Remove, index).Single(),
                    cmds);
                return true;
            }
            return false;
        }

        public void SetTo(IEnumerable<T> enumer, NotifyingFireParameters? cmds = null)
        {
            if (cmds == null)
            {
                cmds = NotifyingFireParameters.Typical;
            }

            if (cmds.Value.MarkAsSet)
            {
                HasBeenSet = true;
            }
            List<Exception> exceptions = null;
            if (HasSubscribers())
            { // Will be firing
                using (var changes = firePool.Checkout())
                {
                    int i = 0;
                    foreach (var item in enumer)
                    {
                        if (i >= this.list.Count)
                        {
                            this.list.Add(item);

                            changes.Item.Add(
                                new ChangeIndex<T>(default(T), item, AddRemoveModify.Add, i));
                        }
                        else if (!object.ReferenceEquals(this.list[i], item))
                        {
                            var cur = this.list[i];
                            this.list[i] = item;
                            changes.Item.Add(
                                new ChangeIndex<T>(cur, item, AddRemoveModify.Modify, i));
                        }
                        i++;
                    }

                    for (int j = this.list.Count - 1; j >= 0 && j >= i; j--)
                    { // Remove later indices going backwards
                        changes.Item.Add(
                            new ChangeIndex<T>(this.list[j], default(T), AddRemoveModify.Remove, j));
                    }
                    this.list.RemoveEnd(i);

                    _count.Set(list.Count, cmds);
                    FireChange(changes.Item, cmds);
                }
            }
            else
            { // just internals
                int i = 0;
                foreach (var item in enumer)
                {
                    if (i >= this.list.Count)
                    {
                        this.list.Add(item);
                    }
                    else
                    {
                        this.list[i] = item;
                    }
                    i++;
                }

                this.list.RemoveEnd(i);

                _count.Set(list.Count, cmds);
            }

            if (exceptions != null)
            {
                throw new AggregateException(exceptions.ToArray());
            }
        }

        public void Clear(NotifyingFireParameters? cmds = null)
        {
            cmds = ProcessCmds(cmds);

            if (this.list.Count == 0 && !cmds.Value.ForceFire) return;

            if (HasSubscribers())
            { // Will be firing
                using (var changes = firePool.Checkout())
                {
                    for (int i = 0; i < this.list.Count; i++)
                    { // Remove later indices going backwards
                        changes.Item.Add(
                            new ChangeIndex<T>(this.list[i], default(T), AddRemoveModify.Remove, i));
                    }

                    list.Clear();
                    _count.Set(0, cmds);
                    FireChange(changes.Item, cmds);
                }
            }
            else
            { // just internals
                list.Clear();
                _count.Set(0, cmds);
            }
        }

        public void Unset(NotifyingUnsetParameters? cmds = null)
        {
            HasBeenSet = false;
            Clear(cmds.ToFireParams());
        }

        protected override ObjectPoolCheckout<List<ChangeIndex<T>>> CompileCurrent()
        {
            var changes = firePool.Checkout();
            for (int i = 0; i < list.Count; i++)
            {
                changes.Item.Add(new ChangeIndex<T>(default(T), list[i], AddRemoveModify.Add, i));
            }
            return changes;
        }

        public void Subscribe<O>(O owner, NotifyingCollectionCallback<O> callback, bool fireInitial)
        {
            this.Subscribe_Internal(owner, callback, fireInitial);
        }

        public void Subscribe(NotifyingCollectionSimpleCallback callback, bool fireInitial)
        {
            this.Subscribe_Internal<object>(null, (o2, ch) => callback(ch), fireInitial);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerable<T> Iterate()
        {
            return this;
        }

        protected override ObjectPoolCheckout<List<ChangeAddRem<T>>> CompileCurrentEnumer()
        {
            var changes = fireEnumerPool.Checkout();
            for (int i = 0; i < list.Count; i++)
            {
                changes.Item.Add(new ChangeAddRem<T>(list[i], AddRemove.Add));
            }
            return changes;
        }

        protected void FireChange(IEnumerable<ChangeIndex<T>> changes, NotifyingFireParameters? cmds)
        {
            List<Exception> exceptions = null;

            if (this.subscribers != null && this.subscribers.HasSubs)
            {
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
            }

            if (this.enumerSubscribers != null && this.enumerSubscribers.HasSubs)
            {
                using (var enumerChanges = fireEnumerPool.Checkout())
                {
                    foreach (var change in changes)
                    {
                        switch (change.AddRem)
                        {
                            case AddRemoveModify.Add:
                                enumerChanges.Item.Add(new ChangeAddRem<T>(change.New, AddRemove.Add));
                                break;
                            case AddRemoveModify.Remove:
                                enumerChanges.Item.Add(new ChangeAddRem<T>(change.Old, AddRemove.Remove));
                                break;
                            case AddRemoveModify.Modify:
                                enumerChanges.Item.Add(new ChangeAddRem<T>(change.Old, AddRemove.Remove));
                                enumerChanges.Item.Add(new ChangeAddRem<T>(change.New, AddRemove.Add));
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

        #region IList
        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        IEnumerable<T> IHasBeenSetItemGetter<IEnumerable<T>>.Item => list;
        #endregion
    }
}

namespace System
{

    public static class INotifyingListGetterExt
    {
        #region Cast
        public static INotifyingListGetter<R> Cast_List<T, R>(this INotifyingListGetter<T> getter)
            where T : R
        {
            return Cast_List(getter, (t) => (R)t);
        }

        public static INotifyingListGetter<R> Cast_List<T, R>(this INotifyingListGetter<T> getter, Func<T, R> converter)
        {
            if (getter is INotifyingListGetter<R> rhs)
            {
                return rhs;
            }
            return new INotifyingListCastFunc<T, R>()
            {
                Orig = getter,
                Converter = converter
            };
        }

        class INotifyingListCastFunc<T, R> : INotifyingListGetter<R>
        {
            public INotifyingListGetter<T> Orig;
            public Func<T, R> Converter;

            public INotifyingItemGetter<int> CountProperty => Orig.CountProperty;
            public int Count => Orig.Count;

            public bool HasBeenSet => Orig.HasBeenSet;
            
            IEnumerable<R> IHasBeenSetItemGetter<IEnumerable<R>>.Item => Orig.Item.Select((t) => Converter(t));

            public R this[int index] => Converter(Orig[index]);

            public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, R> callback, bool fireInitial = true)
            {
                Orig.Subscribe_Enumerable(
                    owner,
                    (o2, changes) => callback(o2, changes.Select((c) => new ChangeAddRem<R>(this.Converter(c.Item), c.AddRem))),
                    fireInitial);
            }

            public void Unsubscribe(object owner)
            {
                Orig.Unsubscribe(owner);
            }

            public IEnumerator<R> GetEnumerator()
            {
                foreach (var t in Orig)
                {
                    yield return this.Converter(t);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public int IndexOf(R item)
            {
                throw new NotImplementedException();
            }

            public void Subscribe<O>(O owner, NotifyingCollection<R, ChangeIndex<R>>.NotifyingCollectionCallback<O> callback, bool fireInitial)
            {
                Orig.Subscribe<O>(
                    owner,
                    (o2, changes) => callback(o2, changes.Select((c) => new ChangeIndex<R>(Converter(c.Old), Converter(c.New), c.AddRem, c.Index))),
                    fireInitial);
            }

            public void Subscribe(NotifyingCollection<R, ChangeIndex<R>>.NotifyingCollectionSimpleCallback callback, bool fireInitial)
            {
                Orig.Subscribe(
                    (changes) => callback(changes.Select((c) => new ChangeIndex<R>(Converter(c.Old), Converter(c.New), c.AddRem, c.Index))),
                    fireInitial);
            }
        }

        public static INotifyingListGetter<R> Cast_As_List<T, R>(this INotifyingEnumerable<T> getter)
            where T : R
        {
            return Cast_As_List(getter, (t) => (R)t);
        }

        public static INotifyingListGetter<R> Cast_As_List<T, R>(this INotifyingEnumerable<T> getter, Func<T, R> converter)
        {
            if (getter is INotifyingListGetter<R> rhs)
            {
                return rhs;
            }
            return new INotifyingListCastAsFunc<T, R>(getter, converter);
        }

        class INotifyingListCastAsFunc<T, R> : INotifyingListGetter<R>
        {
            public INotifyingEnumerable<T> Orig { get; private set; }
            private Func<T, R> _converter;
            private NotifyingList<R> _internalList = new NotifyingList<R>();
            private int counter;

            public INotifyingItemGetter<int> CountProperty => Orig.CountProperty;
            public int Count => Orig.Count;

            public bool HasBeenSet => Orig.HasBeenSet;

            IEnumerable<R> IHasBeenSetItemGetter<IEnumerable<R>>.Item => Orig.Select((t) => _converter(t));

            public R this[int index] => _internalList[index];

            public INotifyingListCastAsFunc(INotifyingEnumerable<T> enumer, Func<T, R> converter)
            {
                this._converter = converter;
                this.Orig = enumer;
                enumer.Subscribe_Enumerable(
                    this,
                    (changes) =>
                    {
                        foreach (var change in changes)
                        {
                            switch (change.AddRem)
                            {
                                case AddRemove.Add:
                                    this._internalList[counter++] = _converter(change.Item);
                                    break;
                                case AddRemove.Remove:
                                    if (this._internalList.Remove(_converter(change.Item)))
                                    {
                                        counter--;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    });
            }

            public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, R> callback, bool fireInitial)
            {
                _internalList.Subscribe_Enumerable<O>(owner, callback, fireInitial);
            }

            public void Subscribe<O>(O owner, NotifyingCollection<R, ChangeIndex<R>>.NotifyingCollectionCallback<O> callback, bool fireInitial)
            {
                _internalList.Subscribe(owner, callback, fireInitial);
            }

            public void Subscribe(NotifyingCollection<R, ChangeIndex<R>>.NotifyingCollectionSimpleCallback callback, bool fireInitial)
            {
                _internalList.Subscribe(callback, fireInitial);
            }

            public void Unsubscribe(object owner)
            {
                Orig.Unsubscribe(owner);
            }

            public IEnumerator<R> GetEnumerator()
            {
                foreach (var t in Orig)
                {
                    yield return this._converter(t);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public int IndexOf(R item)
            {
                return _internalList.IndexOf(item);
            }
        }
        #endregion

        public static void Subscribe<O, T>(this INotifyingListGetter<T> getter, O owner, NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionCallback<O> callback)
        {
            getter.Subscribe(owner, callback, true);
        }

        public static void Subscribe<O, T>(this INotifyingListGetter<T> getter, O owner, NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionSimpleCallback callback, bool fireInitial = true)
        {
            getter.Subscribe(owner, (o2, ch) => callback(ch), fireInitial);
        }

        public static void SetToArray<T>(this INotifyingList<T> list, params T[] items)
        {
            INotifyingCollectionExt.SetTo(list, ((IEnumerable<T>)items));
        }

        public static bool InRange<T>(this INotifyingListGetter<T> list, int index)
        {
            return index >= 0 && index < list.CountProperty.Item;
        }

        public static bool TryGet<T>(this INotifyingListGetter<T> list, int index, out T item)
        {
            if (!InRange(list, index))
            {
                item = default(T);
                return false;
            }
            item = list[index];
            return true;
        }

        public static T TryGet<T>(this INotifyingListGetter<T> list, int index)
        {
            if (!InRange(list, index))
            {
                return default(T);
            }
            return list[index];
        }
    }
}
