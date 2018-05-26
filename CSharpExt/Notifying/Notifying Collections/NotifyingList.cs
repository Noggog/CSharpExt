using Noggog;
using Noggog.Notifying;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Noggog.Notifying
{
    public interface INotifyingListGetter<T> : INotifyingEnumerable<T>
    {
        T this[int index] { get; }
        int IndexOf(T item);
        void Subscribe<O>(O owner, NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionCallback<O> callback, NotifyingSubscribeParameters cmds = null);
        void Subscribe(NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionSimpleCallback callback, NotifyingSubscribeParameters cmds = null);
    }

    public interface INotifyingList<T> : INotifyingListGetter<T>, INotifyingCollection<T>
    {
        new T this[int index] { get; set; }
        void Insert(int index, T item, NotifyingFireParameters cmds = null);
        void Set(int index, T item, NotifyingFireParameters cmds = null);
        void RemoveAt(int index, NotifyingFireParameters cmds = null);
    }

    public class NotifyingList<T> : NotifyingCollection<T, ChangeIndex<T>>, INotifyingList<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected NotifyingItem<int> _count = new NotifyingItem<int>();
        public INotifyingItemGetter<int> CountProperty => _count;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Count => _count.Item;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected List<T> list = new List<T>();
        public IEnumerable<T> List => list;

        public T this[int index]
        {
            get => list[index];
            set => Set(index, value);
        }

        public virtual void Set(int index, T item, NotifyingFireParameters cmds = null)
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

        public virtual void Insert(int index, T item, NotifyingFireParameters cmds = null)
        {
            cmds = ProcessCmds(cmds);
            list.Insert(index, item);
            _count.Set(_count.Item + 1, cmds);
            if (!HasSubscribers()) return;
            FireChange(
                new ChangeIndex<T>(default(T), item, AddRemoveModify.Add, index).Single(),
                cmds);
        }

        public virtual void Add(T item, NotifyingFireParameters cmds = null)
        {
            cmds = ProcessCmds(cmds);
            list.Add(item);
            _count.Set(list.Count, cmds);
            if (!HasSubscribers()) return;
            FireChange(
                new ChangeIndex<T>(default(T), item, AddRemoveModify.Add, list.Count - 1).Single(),
                cmds);
        }

        public virtual void Add(IEnumerable<T> items, NotifyingFireParameters cmds = null)
        {
            cmds = ProcessCmds(cmds);
            if (HasSubscribers())
            {
                // ToDo
                // Remove multiple enumerations of items
                list.AddRange(items);
                var changes = new List<ChangeIndex<T>>();
                int curCount = list.Count;
                foreach (var item in items)
                {
                    changes.Add(new ChangeIndex<T>(default(T), item, AddRemoveModify.Add, curCount++));
                }
                _count.Set(list.Count, cmds);
                FireChange(changes, cmds);
            }
            else
            {
                list.AddRange(items);
                _count.Set(list.Count, cmds);
            }
        }

        public void RemoveAt(int index, NotifyingFireParameters cmds = null)
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

        public bool Remove(T item, NotifyingFireParameters cmds = null)
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

        public virtual void SetTo(IEnumerable<T> enumer, NotifyingFireParameters cmds = null)
        {
            cmds = cmds ?? NotifyingFireParameters.Typical;

            HasBeenSet = true;
            List<Exception> exceptions = null;
            if (HasSubscribers())
            { // Will be firing
                var changes = new List<ChangeIndex<T>>();
                int i = 0;
                foreach (var item in enumer)
                {
                    if (i >= this.list.Count)
                    {
                        this.list.Add(item);

                        changes.Add(
                            new ChangeIndex<T>(default(T), item, AddRemoveModify.Add, i));
                    }
                    else if (!object.ReferenceEquals(this.list[i], item))
                    {
                        var cur = this.list[i];
                        this.list[i] = item;
                        changes.Add(
                            new ChangeIndex<T>(cur, item, AddRemoveModify.Modify, i));
                    }
                    i++;
                }

                for (int j = this.list.Count - 1; j >= 0 && j >= i; j--)
                { // Remove later indices going backwards
                    changes.Add(
                        new ChangeIndex<T>(this.list[j], default(T), AddRemoveModify.Remove, j));
                }
                this.list.RemoveToCount(i);

                _count.Set(list.Count, cmds);
                FireChange(changes, cmds);
            }
            else
            { // just internals
                this.list.SetTo(enumer);
                _count.Set(list.Count, cmds);
            }

            if (exceptions != null)
            {
                throw new AggregateException(exceptions.ToArray());
            }
        }

        public void Clear(NotifyingFireParameters cmds = null)
        {
            cmds = ProcessCmds(cmds);

            if (this.list.Count == 0 && !cmds.ForceFire) return;

            if (HasSubscribers())
            { // Will be firing
                var changes = new List<ChangeIndex<T>>(this.list.Count);
                for (int i = 0; i < this.list.Count; i++)
                { // Remove later indices going backwards
                    changes.Add(
                        new ChangeIndex<T>(this.list[i], default(T), AddRemoveModify.Remove, i));
                }

                list.Clear();
                _count.Set(0, cmds);
                FireChange(changes, cmds);
            }
            else
            { // just internals
                list.Clear();
                _count.Set(0, cmds);
            }
        }

        public void Unset(NotifyingUnsetParameters cmds = null)
        {
            HasBeenSet = false;
            Clear(cmds.ToFireParams());
        }

        protected override ICollection<ChangeIndex<T>> CompileCurrent()
        {
            var changes = new List<ChangeIndex<T>>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                changes.Add(new ChangeIndex<T>(default(T), list[i], AddRemoveModify.Add, i));
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

        [DebuggerStepThrough]
        public void Subscribe(NotifyingCollectionSimpleCallback callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Subscribe_Internal(
                owner: null, 
                callback: (o2, ch) => callback(ch), 
                cmds: cmds);
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

        protected override ICollection<ChangeAddRem<T>> CompileCurrentEnumer()
        {
            var changes = new List<ChangeAddRem<T>>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                changes.Add(new ChangeAddRem<T>(list[i], AddRemove.Add));
            }
            return changes;
        }

        protected void FireChange(IEnumerable<ChangeIndex<T>> changes, NotifyingFireParameters cmds)
        {
            List<Exception> exceptions = null;

            if (this.subscribers != null && this.subscribers.HasSubs)
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

            if (this.enumerSubscribers != null && this.enumerSubscribers.HasSubs)
            {
                var enumerChanges = new List<ChangeAddRem<T>>();
                foreach (var change in changes)
                {
                    switch (change.AddRem)
                    {
                        case AddRemoveModify.Add:
                            enumerChanges.Add(new ChangeAddRem<T>(change.New, AddRemove.Add));
                            break;
                        case AddRemoveModify.Remove:
                            enumerChanges.Add(new ChangeAddRem<T>(change.Old, AddRemove.Remove));
                            break;
                        case AddRemoveModify.Modify:
                            enumerChanges.Add(new ChangeAddRem<T>(change.Old, AddRemove.Remove));
                            enumerChanges.Add(new ChangeAddRem<T>(change.New, AddRemove.Add));
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

        public bool IsReadOnly => false;

        IEnumerable<T> IHasItemGetter<IEnumerable<T>>.Item => list;
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

            IEnumerable<R> IHasItemGetter<IEnumerable<R>>.Item => Orig.Item.Select((t) => Converter(t));

            public R this[int index] => Converter(Orig[index]);

            public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, R> callback, NotifyingSubscribeParameters cmds = null)
            {
                Orig.Subscribe_Enumerable(
                    owner,
                    (o2, changes) => callback(o2, changes.Select((c) => new ChangeAddRem<R>(this.Converter(c.Item), c.AddRem))),
                    cmds: cmds);
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

            public void Subscribe<O>(O owner, NotifyingCollection<R, ChangeIndex<R>>.NotifyingCollectionCallback<O> callback, NotifyingSubscribeParameters cmds = null)
            {
                Orig.Subscribe<O>(
                    owner,
                    (o2, changes) => callback(o2, changes.Select((c) => new ChangeIndex<R>(Converter(c.Old), Converter(c.New), c.AddRem, c.Index))),
                    cmds: cmds);
            }

            public void Subscribe(NotifyingCollection<R, ChangeIndex<R>>.NotifyingCollectionSimpleCallback callback, NotifyingSubscribeParameters cmds = null)
            {
                Orig.Subscribe(
                    (changes) => callback(changes.Select((c) => new ChangeIndex<R>(Converter(c.Old), Converter(c.New), c.AddRem, c.Index))),
                    cmds: cmds);
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

            IEnumerable<R> IHasItemGetter<IEnumerable<R>>.Item => Orig.Select((t) => _converter(t));

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

            [DebuggerStepThrough]
            public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, R> callback, NotifyingSubscribeParameters cmds = null)
            {
                _internalList.Subscribe_Enumerable(owner, callback, cmds: cmds);
            }

            [DebuggerStepThrough]
            public void Subscribe<O>(O owner, NotifyingCollection<R, ChangeIndex<R>>.NotifyingCollectionCallback<O> callback, NotifyingSubscribeParameters cmds = null)
            {
                _internalList.Subscribe(owner, callback, cmds: cmds);
            }

            [DebuggerStepThrough]
            public void Subscribe(NotifyingCollection<R, ChangeIndex<R>>.NotifyingCollectionSimpleCallback callback, NotifyingSubscribeParameters cmds = null)
            {
                _internalList.Subscribe(callback, cmds: cmds);
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

        [DebuggerStepThrough]
        public static void Subscribe<O, T>(this INotifyingListGetter<T> getter, O owner, NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionCallback<O> callback)
        {
            getter.Subscribe(owner, callback, cmds: NotifyingSubscribeParameters.Typical);
        }

        [DebuggerStepThrough]
        public static void Subscribe<T>(this INotifyingListGetter<T> getter, NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionSimpleCallback callback)
        {
            getter.Subscribe(callback, cmds: NotifyingSubscribeParameters.Typical);
        }

        [DebuggerStepThrough]
        public static void Subscribe<O, T>(this INotifyingListGetter<T> getter, O owner, NotifyingCollection<T, ChangeIndex<T>>.NotifyingCollectionSimpleCallback callback, bool fireInitial = true)
        {
            getter.Subscribe(owner, (o2, ch) => callback(ch), cmds: NotifyingSubscribeParameters.Typical);
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
