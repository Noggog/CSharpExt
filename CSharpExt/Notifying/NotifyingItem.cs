using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public delegate void NotifyingItemCallback<O, T>(O owner, Change<T> change);
    public delegate void NotifyingItemSimpleCallback<T>(Change<T> change);
    public delegate void NotifyingItemInternalCallback<T>(object owner, Change<T> change);

    public interface INotifyingItemGetter<T> : IHasBeenSetGetter
    {
        T Value { get; }
        void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, bool fireInitial);
        void Unsubscribe(object owner);
    }

    public interface INotifyingItem<T> : INotifyingItemGetter<T>, IHasBeenSet<T>
    {
        new T Value { get; set; }
        T DefaultValue { get; }
        void SetCurrentAsDefault();
    }

    public class NotifyingItem<T> : INotifyingItem<T>
    {
        static ObjectPool<SubscriptionHandler<NotifyingItemInternalCallback<T>>> pool = new ObjectPool<SubscriptionHandler<NotifyingItemInternalCallback<T>>>(
            () => new SubscriptionHandler<NotifyingItemInternalCallback<T>>(),
            new LifecycleActions<SubscriptionHandler<NotifyingItemInternalCallback<T>>>(
                onReturn: (s) => s.Clear()),
            200);

        private T _item;
        public T Value
        {
            get
            {
                return _item;
            }
            set
            {
                Set(value, null);
            }
        }

        public T DefaultValue { get; private set; }
        public bool HasBeenSet { get; set; }
        SubscriptionHandler<NotifyingItemInternalCallback<T>> subscribers;

        public NotifyingItem(
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.DefaultValue = defaultVal;
            this._item = defaultVal;
            this.HasBeenSet = markAsSet;
        }

        ~NotifyingItem()
        {
            if (subscribers != null)
            {
                pool.Return(subscribers);
                subscribers = null;
            }
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, bool fireInitial = true)
        {
            if (subscribers == null)
            {
                subscribers = pool.Get();
            }
            subscribers.Add(owner, (own, change) => callback((O)own, change));
            if (fireInitial)
            {
                callback(owner, new Change<T>(this.Value));
            }
        }

        public void Unsubscribe(object owner)
        {
            if (subscribers == null) return;
            subscribers.Remove(owner);
        }

        public void Unset(NotifyingUnsetParameters? cmds = null)
        {
            HasBeenSet = false;
            Set(DefaultValue, cmds.ToFireParams());
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._item;
        }

        public void Set(T value, NotifyingFireParameters? cmd = null)
        {
            if (cmd == null)
            {
                cmd = NotifyingFireParameters.Typical;
            }

            if (cmd.Value.MarkAsSet)
            {
                HasBeenSet = true;
            }
            if (cmd.Value.ForceFire || !object.Equals(_item, value))
            {
                if (subscribers != null && subscribers.HasSubs)
                {
                    var old = _item;
                    _item = value;
                    Fire(old, value, cmd);
                }
                else
                {
                    _item = value;
                }
            }
        }

        private void Fire(T old, T item, NotifyingFireParameters? cmds = null)
        {
            List<Exception> exceptions = null;
            using (var fireSubscribers = subscribers.GetSubs())
            {
                foreach (var sub in fireSubscribers)
                {
                    foreach (var action in sub.Value)
                    {
                        try
                        {
                            action(sub.Key, new Change<T>(old, _item));
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
                    cmds.Value.ExceptionHandler(ex);
                }
            }
        }
        
        public static implicit operator T(NotifyingItem<T> item)
        {
            return item.Value;
        }
    }

    public struct NotifyingItemWrapper<T> : INotifyingItemGetter<T>
    {
        public readonly T Item;

        public NotifyingItemWrapper(T item)
        {
            this.Item = item;
        }

        public bool HasBeenSet { get { return true; } }

        T INotifyingItemGetter<T>.Value
        {
            get
            {
                return this.Item;
            }
        }

        void INotifyingItemGetter<T>.Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, bool fireInitial)
        {
            if (fireInitial)
            {
                callback(owner, new Change<T>(Item));
            }
        }

        void INotifyingItemGetter<T>.Unsubscribe(object owner)
        {
        }
    }
}

namespace System
{

    public static class NotifyingItemExt
    {
        /*
        * Item callback only happens when gate is on
        */
        public static void SubscribeWithBoolGate<O, T>(
            this INotifyingItemGetter<bool> gate,
            O owner,
            INotifyingItemGetter<T> item,
            NotifyingItemSimpleCallback<T> callback,
            Action<T> customDetachCallback = null,
            bool fireInitial = true)
        {
            SubscribeWithBoolGate<O, T>(gate, owner, item, (o2, change) => callback(change), customDetachCallback, fireInitial);
        }

        public static void SubscribeWithBoolGate<O, T>(
            this INotifyingItemGetter<bool> gate,
            O owner,
            INotifyingItemGetter<T> item,
            NotifyingItemCallback<O, T> callback,
            Action<T> customDetachCallback = null,
            bool fireInitial = true)
        {
            bool attached = false;
            item.Subscribe<O>(
                owner,
                (o2, change) =>
                {
                    if (attached)
                    {
                        callback(o2, change);
                    }
                },
                fireInitial);
            gate.Subscribe<O>(
                owner,
                (o2, change) =>
                {
                    if (change.New)
                    {
                        if (!attached)
                        {
                            attached = true;
                            callback(o2, new Change<T>(item.Value));
                        }
                    }
                    else
                    {
                        if (attached)
                        {
                            attached = false;
                            if (customDetachCallback != null)
                            {
                                customDetachCallback(item.Value);
                            }
                            else
                            {
                                callback(o2, new Change<T>(item.Value, default(T)));
                            }
                        }
                    }
                },
                fireInitial);
        }

        public static void Subscribe<O, T>(this INotifyingItemGetter<T> not, O owner, NotifyingItemCallback<O, T> callback)
        {
            not.Subscribe(owner, callback, true);
        }

        public static void Subscribe<O, T>(this INotifyingItemGetter<T> not, O owner, NotifyingItemSimpleCallback<T> callback, bool fireInitial = true)
        {
            not.Subscribe(owner, new NotifyingItemCallback<O, T>((o2, change) => callback(change)), fireInitial);
        }

        public static void Forward<T>(this INotifyingItemGetter<T> not, INotifyingItem<T> to, bool fireInitial = true)
        {
            not.Subscribe(to, (change) => to.Value = change.New, fireInitial: fireInitial);
        }

        public static void Forward<T, R>(this INotifyingItemGetter<T> not, INotifyingItem<R> to, bool fireInitial = true)
            where T : R
        {
            not.Subscribe(to, (change) => to.Value = change.New, fireInitial: fireInitial);
        }

        public static void Set<T>(this INotifyingItem<T> not, T value)
        {
            not.Set(value,
                new NotifyingFireParameters(
                    markAsSet: true,
                    forceFire: false));
        }

        public static void Unset<T>(this INotifyingItem<T> not)
        {
            not.Unset(null);
        }
    }
}
