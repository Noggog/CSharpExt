using System;
using System.Collections.Generic;
using System.Diagnostics;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public static class NotifyingItem
    {
        public static INotifyingItem<T> Factory<T>(
            T defaultVal = default(T),
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
                        return new NotifyingItem<T>(defaultVal: defaultVal);
                    }
                    else
                    {
                        return new NotifyingItemConverter<T>(
                            converter,
                            defaultVal: defaultVal);
                    }
                }
                else
                {
                    if (converter == null)
                    {
                        return new NotifyingItemOnSet<T>(
                            onSet: onSet,
                            defaultVal: defaultVal);
                    }
                    else
                    {
                        return new NotifyingItemConverterOnSet<T>(
                            converter: converter,
                            onSet: onSet,
                            defaultVal: defaultVal);
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
                            defaultVal: defaultVal);
                    }
                    else
                    {
                        return new NotifyingItemNoNullConverter<T>(
                            noNullFallback: noNullFallback,
                            converter: converter,
                            defaultVal: defaultVal);
                    }
                }
                else
                {
                    if (converter == null)
                    {
                        return new NotifyingItemNoNullOnSet<T>(
                            noNullFallback: noNullFallback,
                            onSet: onSet,
                            defaultVal: defaultVal);
                    }
                    else
                    {
                        return new NotifyingItemNoNullOnSetConverter<T>(
                            noNullFallback: noNullFallback,
                            onSet: onSet,
                            converter: converter,
                            defaultVal: defaultVal);
                    }
                }
            }
        }

        public static INotifyingItem<T> FactoryNoNull<T>(
            T initialVal = default(T),
            Action<T> onSet = null,
            Func<T, T> converter = null)
            where T : new()
        {
            if (onSet == null)
            {
                if (converter == null)
                {
                    return new NotifyingItemNoNullDirect<T>(
                        defaultVal: initialVal);
                }
                else
                {
                    return new NotifyingItemNoNullDirectConverter<T>(
                        converter,
                        defaultVal: initialVal);
                }
            }
            else
            {
                if (converter == null)
                {
                    return new NotifyingItemNoNullDirectOnSet<T>(
                        onSet: onSet,
                        defaultVal: initialVal);
                }
                else
                {
                    return new NotifyingItemNoNullDirectOnSetConverter<T>(
                        converter: converter,
                        onSet: onSet,
                        defaultVal: initialVal);
                }
            }
        }
    }

    public class NotifyingItem<T> : INotifyingItem<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected T _item;
        public T Item
        {
            get => _item;
            set => Set(value, null);
        }

        public T DefaultValue => default(T);

        protected SubscriptionHandler<NotifyingItemInternalCallback<T>> subscribers;

        public NotifyingItem()
        {
        }

        public NotifyingItem(T defaultVal)
        {
            this._item = defaultVal;
        }

        [DebuggerStepThrough]
        public void Subscribe(object owner, Action callback, NotifyingSubscribeParameters cmds = null)
        {
            this.SubscribeInternal(owner: owner, callback: (o, c) => callback(), cmds: cmds);
        }

        [DebuggerStepThrough]
        public void Subscribe(Action callback, NotifyingSubscribeParameters cmds = null)
        {
            this.SubscribeInternal(owner: null, callback: (o, c) => callback(), cmds: cmds);
        }

        [DebuggerStepThrough]
        public void Subscribe(object owner, NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.SubscribeInternal(owner: owner, callback: (o, c) => callback(c), cmds: cmds);
        }

        [DebuggerStepThrough]
        public void Subscribe(NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.SubscribeInternal(owner: null, callback: (o, c) => callback(c), cmds: cmds);
        }

        [DebuggerStepThrough]
        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.SubscribeInternal(owner, (own, change) => callback((O)own, change), cmds);
        }

        internal void SubscribeInternal(object owner, NotifyingItemInternalCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (subscribers == null)
            {
                subscribers = new SubscriptionHandler<NotifyingItemInternalCallback<T>>();
            }
            subscribers.Add(owner, callback);
            if (cmds.FireInitial)
            {
                callback(owner, new Change<T>(this.Item));
            }
        }

        public void Unsubscribe(object owner)
        {
            if (subscribers == null) return;
            subscribers.Remove(owner);
        }

        public virtual void Set(T value, NotifyingFireParameters cmds = null)
        {
            cmds = cmds ?? NotifyingFireParameters.Typical;

            if (cmds.ForceFire || !object.Equals(_item, value))
            {
                if (subscribers != null && subscribers.HasSubs)
                {
                    var old = _item;
                    _item = value;
                    Fire(old, value, cmds);
                }
                else
                {
                    _item = value;
                }
            }
        }

        protected void Fire(T old, T item, NotifyingFireParameters cmds = null)
        {
            List<Exception> exceptions = null;
            foreach (var sub in subscribers.GetSubs())
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

        public static implicit operator T(NotifyingItem<T> item)
        {
            return item.Item;
        }

        public override string ToString()
        {
            return Item?.ToString();
        }

        public void Unset()
        {
            this.Item = default(T);
        }
    }
}
