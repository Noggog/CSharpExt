using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Noggog.Containers.Pools;
using Noggog.Notifying;
using Noggog;

namespace Noggog.Notifying
{
    public interface INotifyingEnumerable<T> : IEnumerable<T>, IHasBeenSetItemGetter<IEnumerable<T>>
    {
        void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, T> callback, NotifyingSubscribeParameters cmds = null);
        void Unsubscribe(object owner);
        INotifyingItemGetter<int> CountProperty { get; }
        int Count { get; }
    }

    public interface INotifyingCollection<T> : INotifyingEnumerable<T>
    {
        void Unset(NotifyingUnsetParameters cmds);
        void Clear(NotifyingFireParameters cmds);
        bool Remove(T item, NotifyingFireParameters cmds);
        void SetTo(IEnumerable<T> enumer, NotifyingFireParameters cmds);
        void Add(T item, NotifyingFireParameters cmds);
        void Add(IEnumerable<T> items, NotifyingFireParameters cmds);
        new bool HasBeenSet { get; set; }
    }

    public delegate void NotifyingEnumerableCallback<O, T>(O owner, IEnumerable<ChangeAddRem<T>> changes);
    public delegate void NotifyingEnumerableSimpleCallback<T>(IEnumerable<ChangeAddRem<T>> changes);
    public delegate void NotifyingInternalEnumerableCallback<T>(object owner, IEnumerable<ChangeAddRem<T>> changes);

    public abstract class NotifyingCollection<T, F>
    {
        static ObjectPool<SubscriptionHandler<NotifyingCollectionInternalCallback>> pool = new ObjectPool<SubscriptionHandler<NotifyingCollectionInternalCallback>>(
            () => new SubscriptionHandler<NotifyingCollectionInternalCallback>(),
            new LifecycleActions<SubscriptionHandler<NotifyingCollectionInternalCallback>>(
                onReturn: (s) => s.Clear()),
            200);
        static ObjectPool<SubscriptionHandler<NotifyingInternalEnumerableCallback<T>>> enumerPool = new ObjectPool<SubscriptionHandler<NotifyingInternalEnumerableCallback<T>>>(
            () => new SubscriptionHandler<NotifyingInternalEnumerableCallback<T>>(),
            new LifecycleActions<SubscriptionHandler<NotifyingInternalEnumerableCallback<T>>>(
                onReturn: (s) => s.Clear()),
            200);

        protected static ObjectListPool<ChangeAddRem<T>> fireEnumerPool = new ObjectListPool<ChangeAddRem<T>>(100);

        public delegate void NotifyingCollectionSimpleCallback(IEnumerable<F> changes);
        public delegate void NotifyingCollectionInternalCallback(object owner, IEnumerable<F> changes);
        public delegate void NotifyingCollectionCallback<O>(O owner, IEnumerable<F> changes);

        protected SubscriptionHandler<NotifyingCollectionInternalCallback> subscribers;
        protected SubscriptionHandler<NotifyingInternalEnumerableCallback<T>> enumerSubscribers;
        public bool HasBeenSet { get; set; }

        ~NotifyingCollection()
        {
            if (subscribers != null)
            {
                pool.Return(subscribers);
                subscribers = null;
            }
            if (enumerSubscribers != null)
            {
                enumerPool.Return(enumerSubscribers);
                enumerSubscribers = null;
            }
        }

        public virtual bool HasSubscribers()
        {
            if (this.subscribers != null && this.subscribers.HasSubs) return true;
            if (this.enumerSubscribers != null && this.enumerSubscribers.HasSubs) return true;
            return false;
        }

        internal void Subscribe_Internal(
            object owner,
            NotifyingCollectionInternalCallback callback,
            NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (subscribers == null)
            {
                subscribers = pool.Get();
            }
            subscribers.Add(owner, callback);

            if (cmds.FireInitial)
            {
                using (var current = CompileCurrent())
                {
                    callback(owner, current.Item);
                }
            }
        }

        public void Subscribe_Enumerable<O>(
            O owner,
            NotifyingEnumerableCallback<O, T> callback,
            NotifyingSubscribeParameters cmds = null)
        {
            this.Subscribe_Enumerable_Internal(
                owner: owner,
                callback: (own, change) => callback((O)own, change),
                cmds: cmds);
        }

        internal void Subscribe_Enumerable_Internal(
            object owner,
            NotifyingInternalEnumerableCallback<T> callback,
            NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (enumerSubscribers == null)
            {
                enumerSubscribers = enumerPool.Get();
            }
            enumerSubscribers.Add(owner, callback);

            if (cmds.FireInitial)
            {
                using (var current = CompileCurrentEnumer())
                {
                    callback(owner, current.Item);
                }
            }
        }

        public virtual void Unsubscribe(object owner)
        {
            if (subscribers != null)
            {
                subscribers.Remove(owner);
            }
            if (enumerSubscribers != null)
            {
                enumerSubscribers.Remove(owner);
            }
        }

        protected abstract ObjectPoolCheckout<List<F>> CompileCurrent();

        protected abstract ObjectPoolCheckout<List<ChangeAddRem<T>>> CompileCurrentEnumer();

        protected NotifyingFireParameters ProcessCmds(NotifyingFireParameters cmds = null)
        {
            if (cmds == null)
            {
                cmds = NotifyingFireParameters.Typical;
            }
            if (cmds.MarkAsSet)
            {
                HasBeenSet = true;
            }
            return cmds;
        }
    }
}

namespace System
{
    public static class INotifyingCollectionExt
    {
        #region Cast
        public static INotifyingEnumerable<R> Cast<T, R>(this INotifyingEnumerable<T> getter)
            where T : R
        {
            if (getter is INotifyingListGetter<R> rhs)
            {
                return rhs;
            }
            return new INotifyingEnumerableCast<T, R>()
            {
                Orig = getter
            };
        }

        class INotifyingEnumerableCast<T, R> : INotifyingEnumerable<R>
            where T : R
        {
            public INotifyingEnumerable<T> Orig;

            public bool HasBeenSet => Orig.HasBeenSet;

            public INotifyingItemGetter<int> CountProperty => Orig.CountProperty;

            public int Count => Orig.Count;

            IEnumerable<R> IHasItemGetter<IEnumerable<R>>.Item => Orig.Select<T, R>((t) => t);

            public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, R> callback, NotifyingSubscribeParameters cmds = null)
            {
                Orig.Subscribe_Enumerable(
                    owner,
                    (o2, changes) => callback(o2, changes.Select((c) => new ChangeAddRem<R>((R)c.Item, c.AddRem))),
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
                    yield return (R)t;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
        #endregion

        public static void Subscribe_Enumerable<O, T>(
            this INotifyingEnumerable<T> getter,
            O owner,
            NotifyingEnumerableCallback<O, T> callback = null)
        {
            getter.Subscribe_Enumerable<O>(owner, callback, NotifyingSubscribeParameters.Typical);
        }

        public static void Subscribe_Enumerable<T>(
            this INotifyingEnumerable<T> getter,
            object owner,
            NotifyingEnumerableSimpleCallback<T> callback,
            NotifyingSubscribeParameters cmds = null)
        {
            getter.Subscribe_Enumerable(owner, (o2, changes) => callback(changes), cmds);
        }

        public static void Subscribe_Enumerable<T>(
            this INotifyingEnumerable<T> getter,
            NotifyingEnumerableSimpleCallback<T> callback,
            NotifyingSubscribeParameters cmds = null)
        {
            getter.Subscribe_Enumerable<object>(owner: null, callback: (o2, changes) => callback(changes), cmds: cmds);
        }

        public static void SetTo<T>(this INotifyingCollection<T> list, IEnumerable<T> enumer)
        {
            list.SetTo(enumer, NotifyingFireParameters.Typical);
        }

        public static void Unset<T>(this INotifyingCollection<T> list)
        {
            list.Unset(cmds: null);
        }

        public static void Subscribe_Enumerable_Single<O, T>(
            this INotifyingEnumerable<T> get,
            O owner,
            Action<ChangeAddRem<T>> callback,
            NotifyingSubscribeParameters cmds = null)
        {
            get.Subscribe_Enumerable<O>(
                owner,
                (o2, changes) =>
                {
                    foreach (var change in changes)
                    {
                        callback(change);
                    }
                },
                cmds: cmds);
        }

        public static void Subscribe_Enumerable_Single<O, T>(
            this INotifyingEnumerable<T> get,
            O owner, 
            Action<O, ChangeAddRem<T>> callback,
            NotifyingSubscribeParameters cmds = null)
        {
            get.Subscribe_Enumerable<O>(
                owner,
                (o2, changes) =>
                {
                    foreach (var change in changes)
                    {
                        callback(o2, change);
                    }
                },
                cmds: cmds);
        }

        public static void Subscribe_Enumerable_Single<T>(
            this INotifyingEnumerable<T> get, 
            object owner, 
            Action<object, ChangeAddRem<T>> callback,
            NotifyingSubscribeParameters cmds = null)
        {
            get.Subscribe_Enumerable(
                owner,
                (o2, changes) =>
                {
                    foreach (var change in changes)
                    {
                        callback(o2, change);
                    }
                },
                cmds: cmds);
        }

        public static void Subscribe_Enumerable_Single<T>(
            this INotifyingEnumerable<T> get, 
            Action<ChangeAddRem<T>> callback,
            NotifyingSubscribeParameters cmds = null)
        {
            get.Subscribe_Enumerable(
                (changes) =>
                {
                    foreach (var change in changes)
                    {
                        callback(change);
                    }
                },
                cmds: cmds);
        }

        public static void Clear<T>(this INotifyingCollection<T> not)
        {
            not.Clear(null);
        }

        public static void Add<T>(this INotifyingCollection<T> not, T item)
        {
            not.Add(item, null);
        }

        public static void Add<T>(this INotifyingCollection<T> not, IEnumerable<T> items)
        {
            not.Add(items, null);
        }

        public static bool Remove<T>(this INotifyingCollection<T> not, T item)
        {
            return not.Remove(item, null);
        }

        public static void SetToWithDefault<T>(
            this INotifyingCollection<T> not, 
            IHasBeenSetItemGetter<IEnumerable<T>> rhs, 
            IHasBeenSetItemGetter<IEnumerable<T>> def,
            NotifyingFireParameters cmds = null)
        {
            if (rhs.HasBeenSet)
            {
                not.SetTo(rhs.Item, cmds);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(def.Item, cmds);
            }
            else
            {
                not.Unset(cmds.ToUnsetParams());
            }
        }

        public static void SetToWithDefault<T>(
            this INotifyingCollection<T> not,
            IHasBeenSetItemGetter<IEnumerable<T>> rhs, 
            INotifyingListGetter<T> def, 
            Func<T, T, T> converter,
            NotifyingFireParameters cmds = null)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.SetTo(
                        rhs.Item.Select((t) => converter(t, default(T))),
                        cmds);
                }
                else
                {
                    int i = 0;
                    not.SetTo(
                        rhs.Item.Select((t) => converter(t, def.TryGet(i++))),
                        cmds);
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(
                    def.Item.Select((t) => converter(t, default(T))),
                    cmds);
            }
            else
            {
                not.Unset(cmds.ToUnsetParams());
            }
        }

        public static void SetIfSucceeded<T>(
            this INotifyingCollection<T> not,
            TryGet<IEnumerable<T>> tryGet,
            NotifyingFireParameters cmds = null)
        {
            if (tryGet.Failed) return;
            not.SetTo(tryGet.Value, cmds);
        }
    }
}
