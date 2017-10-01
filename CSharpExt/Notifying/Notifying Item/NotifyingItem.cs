using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public static class NotifyingItem
    {
        public static INotifyingItem<T> Factory<T>(
            T defaultVal = default(T),
            bool markAsSet = false,
            Func<T> noNullFallback = null,
            Action<T> onSet = null,
            Func<T, T> converter = null)
        {
            if (noNullFallback == null)
            {
                if (onSet == null)
                {
                    if (converter == null)
                    {
                        return new NotifyingItem<T>(
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new NotifyingItemConverter<T>(
                            converter,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
                else
                {
                    if (converter == null)
                    {
                        return new NotifyingItemOnSet<T>(
                            onSet: onSet,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new NotifyingItemConverterOnSet<T>(
                            converter: converter,
                            onSet: onSet,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
            }
            else
            {
                if (onSet == null)
                {
                    if (converter == null)
                    {
                        return new NotifyingItemNoNull<T>(
                            noNullFallback: noNullFallback,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new NotifyingItemNoNullConverter<T>(
                            noNullFallback: noNullFallback,
                            converter: converter,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
                else
                {
                    if (converter == null)
                    {
                        return new NotifyingItemNoNullOnSet<T>(
                            noNullFallback: noNullFallback,
                            onSet: onSet,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new NotifyingItemNoNullOnSetConverter<T>(
                            noNullFallback: noNullFallback,
                            onSet: onSet,
                            converter: converter,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
            }
        }

        public static INotifyingItem<T> FactoryNoNull<T>(
            T defaultVal = default(T),
            bool markAsSet = false,
            Action<T> onSet = null,
            Func<T, T> converter = null)
            where T : new()
        {
            if (onSet == null)
            {
                if (converter == null)
                {
                    return new NotifyingItemNoNullDirect<T>(
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
                else
                {
                    return new NotifyingItemNoNullDirectConverter<T>(
                        converter,
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
            }
            else
            {
                if (converter == null)
                {
                    return new NotifyingItemNoNullDirectOnSet<T>(
                        onSet: onSet,
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
                else
                {
                    return new NotifyingItemNoNullDirectOnSetConverter<T>(
                        converter: converter,
                        onSet: onSet,
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
            }
        }
    }

    public class NotifyingItem<T> : INotifyingItem<T>
    {
        static ObjectPool<SubscriptionHandler<NotifyingItemInternalCallback<T>>> pool = new ObjectPool<SubscriptionHandler<NotifyingItemInternalCallback<T>>>(
            () => new SubscriptionHandler<NotifyingItemInternalCallback<T>>(),
            new LifecycleActions<SubscriptionHandler<NotifyingItemInternalCallback<T>>>(
                onReturn: (s) => s.Clear()),
            200);

        protected T _item;
        public T Item
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
        protected SubscriptionHandler<NotifyingItemInternalCallback<T>> subscribers;

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
                callback(owner, new Change<T>(this.Item));
            }
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback, bool fireInitial)
        {
            this.Subscribe<object>(owner: null, callback: (o, c) => callback(c), fireInitial: fireInitial);
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback)
        {
            this.Subscribe<object>(owner: null, callback: (o, c) => callback(c), fireInitial: true);
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback)
        {
            this.Subscribe<O>(owner: owner, callback: callback, fireInitial: true);
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

        public virtual void Set(T value, NotifyingFireParameters? cmd = null)
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

        protected void Fire(T old, T item, NotifyingFireParameters? cmds = null)
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

        void IHasBeenSetItem<T>.Set(T value) => Set(value, cmd: null);

        void IHasBeenSetItem<T>.Unset() => Unset(cmds: null);

        public static implicit operator T(NotifyingItem<T> item)
        {
            return item.Item;
        }
    }
}
