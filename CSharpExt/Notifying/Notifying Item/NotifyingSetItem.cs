using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public static class NotifyingSetItem
    {
        public static INotifyingSetItem<T> Factory<T>(
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
                        return new NotifyingSetItem<T>(
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new NotifyingSetItemConverter<T>(
                            converter,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
                else
                {
                    if (converter == null)
                    {
                        return new NotifyingSetItemOnSet<T>(
                            onSet: onSet,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new NotifyingSetItemConverterOnSet<T>(
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
                        return new NotifyingSetItemNoNull<T>(
                            noNullFallback: noNullFallback,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new NotifyingSetItemNoNullConverter<T>(
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
                        return new NotifyingSetItemNoNullOnSet<T>(
                            noNullFallback: noNullFallback,
                            onSet: onSet,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                    else
                    {
                        return new NotifyingSetItemNoNullOnSetConverter<T>(
                            noNullFallback: noNullFallback,
                            onSet: onSet,
                            converter: converter,
                            defaultVal: defaultVal,
                            markAsSet: markAsSet);
                    }
                }
            }
        }

        public static INotifyingSetItem<T> FactoryNoNull<T>(
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
                    return new NotifyingSetItemNoNullDirect<T>(
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
                else
                {
                    return new NotifyingSetItemNoNullDirectConverter<T>(
                        converter,
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
            }
            else
            {
                if (converter == null)
                {
                    return new NotifyingSetItemNoNullDirectOnSet<T>(
                        onSet: onSet,
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
                else
                {
                    return new NotifyingSetItemNoNullDirectOnSetConverter<T>(
                        converter: converter,
                        onSet: onSet,
                        defaultVal: defaultVal,
                        markAsSet: markAsSet);
                }
            }
        }
    }

    public class NotifyingSetItem<T> : INotifyingSetItem<T>
    {
        protected T _item;
        public T Item
        {
            get => _item;
            set => Set(value, null);
        }

        public T DefaultValue { get; private set; }
        protected bool _HasBeenSet;
        public bool HasBeenSet
        {
            get => _HasBeenSet;
            set => SetHasBeenSet(value);
        }
        protected SubscriptionHandler<NotifyingSetItemInternalCallback<T>> subscribers;

        public NotifyingSetItem(
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.DefaultValue = defaultVal;
            this._item = defaultVal;
            this._HasBeenSet = markAsSet;
        }

        void INotifyingItemGetter<T>.Subscribe(Action callback, NotifyingSubscribeParameters cmds)
        {
            ((INotifyingItemGetter<T>)this).Subscribe<object>(
                owner: null,
                callback: (o, c) => callback(),
                cmds: cmds);
        }

        void INotifyingItemGetter<T>.Subscribe(object owner, Action callback, NotifyingSubscribeParameters cmds)
        {
            ((INotifyingItemGetter<T>)this).Subscribe<object>(
                owner: owner,
                callback: (o, c) => callback(),
                cmds: cmds);
        }

        void INotifyingItemGetter<T>.Subscribe(object owner, NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds)
        {
            ((INotifyingItemGetter<T>)this).Subscribe<object>(
                owner: owner,
                callback: (o, c) => callback(c),
                cmds: cmds);
        }

        void INotifyingItemGetter<T>.Subscribe(NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds)
        {
            ((INotifyingItemGetter<T>)this).Subscribe<object>(
                owner: null, 
                callback: (o, c) => callback(c), 
                cmds: cmds);
        }

        void INotifyingItemGetter<T>.Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, NotifyingSubscribeParameters cmds)
        {
            this.Subscribe<O>(
                owner: owner,
                callback: (o, c) => callback(o, c),
                cmds: cmds);
        }

        public void Subscribe(NotifyingSetItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Subscribe<object>(
                owner: null,
                callback: (o, c) => callback(c),
                cmds: cmds);
        }

        public void Subscribe(object owner, NotifyingSetItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.Subscribe<object>(
                owner: owner,
                callback: (o, c) => callback(c),
                cmds: cmds);
        }

        public void Subscribe<O>(O owner, NotifyingSetItemCallback<O, T> callback, NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (subscribers == null)
            {
                subscribers = new SubscriptionHandler<NotifyingSetItemInternalCallback<T>>();
            }
            subscribers.Add(owner, (own, change) => callback((O)own, change));
            if (cmds.FireInitial)
            {
                callback(owner, new ChangeSet<T>(this.Item, newSet: this.HasBeenSet));
            }
        }

        public void Bind(object owner, INotifyingSetItem<T> rhs, NotifyingBindParameters cmds = null)
        {
            this.Subscribe(
                owner: owner,
                callback: (c) =>
                {
                    rhs.Set(this.Item, this.HasBeenSet, cmds?.FireParameters);
                },
                cmds: cmds?.SubscribeParameters);
            rhs.Subscribe(
                owner: owner,
                callback: (c) =>
                {
                    this.Item = c.New;
                },
                cmds: NotifyingSubscribeParameters.NoFire);
        }

        public void Bind<R>(object owner, INotifyingSetItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null)
        {
            this.Subscribe(
                owner: owner,
                callback: (c) =>
                {
                    rhs.Set(toConv(this.Item), this.HasBeenSet, cmds?.FireParameters);
                },
                cmds: cmds?.SubscribeParameters);
            rhs.Subscribe(
                owner: owner,
                callback: (c) =>
                {
                    this.Item = fromConv(c.New);
                },
                cmds: NotifyingSubscribeParameters.NoFire);
        }

        public void Bind(object owner, INotifyingItem<T> rhs, NotifyingBindParameters cmds = null)
        {
            this.Subscribe(
                owner: owner,
                callback: (c) =>
                {
                    rhs.Set(this.Item, cmds?.FireParameters);
                },
                cmds: cmds?.SubscribeParameters);
            rhs.Subscribe(
                owner: owner,
                callback: (c) =>
                {
                    this.Item = c.New;
                },
                cmds: NotifyingSubscribeParameters.NoFire);
        }

        public void Bind<R>(object owner, INotifyingItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null)
        {
            this.Subscribe(
                owner: owner,
                callback: (c) =>
                {
                    rhs.Set(toConv(this.Item), cmds?.FireParameters);
                },
                cmds: cmds?.SubscribeParameters);
            rhs.Subscribe(
                owner: owner,
                callback: (c) =>
                {
                    this.Item = fromConv(c.New);
                },
                cmds: NotifyingSubscribeParameters.NoFire);
        }

        public void Bind(INotifyingSetItem<T> rhs, NotifyingBindParameters cmds = null)
        {
            this.Bind(
                owner: null,
                rhs: rhs,
                cmds: cmds);
        }

        public void Bind<R>(INotifyingSetItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null)
        {
            this.Bind(
                owner: null,
                rhs: rhs,
                toConv: toConv,
                fromConv: fromConv,
                cmds: cmds);
        }

        public void Bind(INotifyingItem<T> rhs, NotifyingBindParameters cmds = null)
        {
            this.Bind(
                owner: null,
                rhs: rhs,
                cmds: cmds);
        }

        public void Bind<R>(INotifyingItem<R> rhs, Func<T, R> toConv, Func<R, T> fromConv, NotifyingBindParameters cmds = null)
        {
            this.Bind(
                owner: null,
                rhs: rhs,
                toConv: toConv,
                fromConv: fromConv,
                cmds: cmds);
        }

        public void Unsubscribe(object owner)
        {
            if (subscribers == null) return;
            subscribers.Remove(owner);
        }

        public void Unset(NotifyingUnsetParameters cmds = null)
        {
            HasBeenSet = false;
            Set(DefaultValue, cmds.ToFireParams());
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._item;
        }

        private void SetHasBeenSet(bool value)
        {
            Set(this.Item, hasBeenSet: value, cmd: NotifyingFireParameters.Typical);
        }

        public void Set(T value, NotifyingFireParameters cmd = null)
        {
            cmd = cmd ?? NotifyingFireParameters.Typical;
            Set(value, cmd.MarkAsSet ? true : this.HasBeenSet, cmd);
        }

        public virtual void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = null)
        {
            var oldSet = this.HasBeenSet;
            this._HasBeenSet = hasBeenSet;
            if (cmd.ForceFire 
                || oldSet != this.HasBeenSet 
                || !object.Equals(_item, value))
            {
                if (subscribers != null && subscribers.HasSubs)
                {
                    var old = _item;
                    _item = value;
                    Fire(new ChangeSet<T>(
                        oldVal: old,
                        oldSet: oldSet,
                        newVal: value,
                        newSet: true), cmd);
                }
                else
                {
                    _item = value;
                }
            }
        }

        protected void Fire(ChangeSet<T> change, NotifyingFireParameters cmds = null)
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
                            action(sub.Key, change);
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

        void IHasBeenSetItem<T>.Set(T value, bool hasBeenSet) => Set(value, hasBeenSet, cmd: null);

        void IHasBeenSetItem<T>.Unset() => Unset(cmds: null);

        public static implicit operator T(NotifyingSetItem<T> item)
        {
            return item.Item;
        }
    }
}
