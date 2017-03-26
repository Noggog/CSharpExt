using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public interface INotifyingEnumerable<T> : IEnumerable<T>
    {
        void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, T> callback, bool fireInitial);
        void Unsubscribe(object owner);
        INotifyingItemGetter<int> Count { get; }
        bool HasBeenSet { get; }
    }

    public interface INotifyingCollection<T> : INotifyingEnumerable<T>
    {
        void Unset(NotifyingUnsetParameters? cmds);
        void Clear(NotifyingFireParameters? cmds);
        bool Remove(T item, NotifyingFireParameters? cmds);
        void SetTo(IEnumerable<T> enumer, NotifyingFireParameters? cmds);
        void Add(T item, NotifyingFireParameters? cmds);
        void Add(IEnumerable<T> items, NotifyingFireParameters? cmds);
        new bool HasBeenSet { get; set; }
    }

    public delegate void NotifyingEnumerableCallback<O, T>(O owner, IEnumerable<ChangeAddRem<T>> changes);
    public delegate void NotifyingEnumerableSimpleCallback<T>(IEnumerable<ChangeAddRem<T>> changes);
    public delegate void NotifyingInternalEnumerableCallback<T>(object owner, IEnumerable<ChangeAddRem<T>> changes);

    public abstract class NotifyingCollection<T, F>
    {
        static ObjectPool<SubscriptionHandler<NotifyingCollectionInternalCallback>> pool = new ObjectPool<SubscriptionHandler<NotifyingCollectionInternalCallback>>(
            () => new SubscriptionHandler<NotifyingCollectionInternalCallback>(),
            new LifecycleActions<SubscriptionHandler<NotifyingCollectionInternalCallback>>()
            {
                OnReturn = (s) => s.Clear()
            },
            200);
        static ObjectPool<SubscriptionHandler<NotifyingInternalEnumerableCallback<T>>> enumerPool = new ObjectPool<SubscriptionHandler<NotifyingInternalEnumerableCallback<T>>>(
            () => new SubscriptionHandler<NotifyingInternalEnumerableCallback<T>>(),
            new LifecycleActions<SubscriptionHandler<NotifyingInternalEnumerableCallback<T>>>()
            {
                OnReturn = (s) => s.Clear()
            },
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

        protected void Subscribe_Internal<O>(
            O owner,
            NotifyingCollectionCallback<O> callback,
            bool fireInitial)
        {
            if (subscribers == null)
            {
                subscribers = pool.Get();
            }
            subscribers.Add(owner, (o, ch) => callback((O)o, ch));

            if (fireInitial)
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
            bool fireInitial)
        {
            if (enumerSubscribers == null)
            {
                enumerSubscribers = enumerPool.Get();
            }
            enumerSubscribers.Add(owner, (o, ch) => callback((O)o, ch));

            if (fireInitial)
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

        protected NotifyingFireParameters ProcessCmds(NotifyingFireParameters? cmds)
        {
            if (cmds == null)
            {
                cmds = NotifyingFireParameters.Typical;
            }
            if (cmds.Value.MarkAsSet)
            {
                HasBeenSet = true;
            }
            return cmds.Value;
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
            INotifyingListGetter<R> rhs = getter as INotifyingListGetter<R>;
            if (rhs != null)
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

            public bool HasBeenSet { get { return Orig.HasBeenSet; } }

            public INotifyingItemGetter<int> Count { get { return Orig.Count; } }

            public void Subscribe_Enumerable<O>(O owner, NotifyingEnumerableCallback<O, R> callback, bool fireInitial)
            {
                Orig.Subscribe_Enumerable(
                    owner,
                    (o2, changes) => callback(o2, changes.Select((c) => new ChangeAddRem<R>((R)c.Item, c.AddRem))),
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
                    yield return (R)t;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
        #endregion

        public static void Subscribe_Enumerable<O, T>(this INotifyingEnumerable<T> getter, O owner, NotifyingEnumerableCallback<O, T> callback)
        {
            getter.Subscribe_Enumerable<O>(owner, callback, true);
        }

        public static void Subscribe_Enumerable<T>(this INotifyingEnumerable<T> getter, object owner, NotifyingEnumerableSimpleCallback<T> callback, bool fireInitial = true)
        {
            getter.Subscribe_Enumerable(owner, (o2, changes) => callback(changes), fireInitial);
        }

        public static void SetTo<T>(this INotifyingCollection<T> list, IEnumerable<T> enumer)
        {
            list.SetTo(enumer, NotifyingFireParameters.Typical);
        }

        public static void Unset<T>(this INotifyingCollection<T> list)
        {
            list.Unset(cmds: null);
        }

        public static void Subscribe_Enumerable_Single<O, T>(this INotifyingEnumerable<T> get, O owner, Action<ChangeAddRem<T>> callback, bool fireInitial = true)
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
                fireInitial);
        }

        public static void Subscribe_Enumerable_Single<O, T>(this INotifyingEnumerable<T> get, O owner, Action<O, ChangeAddRem<T>> callback, bool fireInitial = true)
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
                fireInitial);
        }

        public static void Subscribe_Enumerable_Single<T>(this INotifyingEnumerable<T> get, object owner, Action<object, ChangeAddRem<T>> callback, bool fireInitial = true)
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
                fireInitial: fireInitial);
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
    }
}
