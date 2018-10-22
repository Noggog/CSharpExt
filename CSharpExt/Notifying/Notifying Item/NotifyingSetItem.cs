using System;
using System.Collections.Generic;
using System.Diagnostics;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public static class NotifyingSetItem
    {
        public static INotifyingSetItem<T> Factory<T>(
            T defaultVal = default(T),
            bool revertToDefaultOnUnset = false,
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
                        if (revertToDefaultOnUnset)
                        {
                            return new NotifyingSetItemDefault<T>(
                                defaultVal: defaultVal,
                                markAsSet: markAsSet);
                        }
                        else
                        {
                            return new NotifyingSetItem<T>(
                                defaultVal: defaultVal,
                                markAsSet: markAsSet);
                        }
                    }
                    else
                    {
                        if (revertToDefaultOnUnset)
                        {
                            return new NotifyingSetItemConverterDefault<T>(
                                converter,
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
                }
                else
                {
                    if (converter == null)
                    {
                        if (revertToDefaultOnUnset)
                        {
                            return new NotifyingSetItemOnSetDefault<T>(
                                onSet: onSet,
                                defaultVal: defaultVal,
                                markAsSet: markAsSet);
                        }
                        else
                        {
                            return new NotifyingSetItemOnSet<T>(
                                onSet: onSet,
                                defaultVal: defaultVal,
                                markAsSet: markAsSet);
                        }
                    }
                    else
                    {
                        if (revertToDefaultOnUnset)
                        {
                            return new NotifyingSetItemConverterOnSetDefault<T>(
                                converter: converter,
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
            }
            else
            {
                if (onSet == null)
                {
                    if (converter == null)
                    {
                        if (revertToDefaultOnUnset)
                        {
                            return new NotifyingSetItemNoNullDefault<T>(
                                noNullFallback: noNullFallback,
                                defaultVal: defaultVal,
                                markAsSet: markAsSet);
                        }
                        else
                        {
                            return new NotifyingSetItemNoNull<T>(
                                noNullFallback: noNullFallback,
                                defaultVal: defaultVal,
                                markAsSet: markAsSet);
                        }
                    }
                    else
                    {
                        if (revertToDefaultOnUnset)
                        {
                            return new NotifyingSetItemNoNullConverterDefault<T>(
                                noNullFallback: noNullFallback,
                                converter: converter,
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
                }
                else
                {
                    if (converter == null)
                    {
                        if (revertToDefaultOnUnset)
                        {
                            return new NotifyingSetItemNoNullOnSetDefault<T>(
                                noNullFallback: noNullFallback,
                                onSet: onSet,
                                defaultVal: defaultVal,
                                markAsSet: markAsSet);
                        }
                        else
                        {
                            return new NotifyingSetItemNoNullOnSet<T>(
                                noNullFallback: noNullFallback,
                                onSet: onSet,
                                defaultVal: defaultVal,
                                markAsSet: markAsSet);
                        }
                    }
                    else
                    {
                        if (revertToDefaultOnUnset)
                        {
                            return new NotifyingSetItemNoNullOnSetConverterDefault<T>(
                                noNullFallback: noNullFallback,
                                onSet: onSet,
                                converter: converter,
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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected T _item;
        public T Item
        {
            get => _item;
            set => Set(value, null);
        }

        public virtual T DefaultValue => default(T);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
            this._item = defaultVal;
            this._HasBeenSet = markAsSet;
        }

        [DebuggerStepThrough]
        public void Subscribe(Action callback, NotifyingSubscribeParameters cmds)
        {
            this.SubscribeInternal(
                owner: null,
                callback: (own, change) => callback(),
                cmds: cmds);
        }

        [DebuggerStepThrough]
        public void Subscribe(object owner, Action callback, NotifyingSubscribeParameters cmds)
        {
            this.SubscribeInternal(
                owner: owner,
                callback: (own, change) => callback(),
                cmds: cmds);
        }

        [DebuggerStepThrough]
        void INotifyingItemGetter<T>.Subscribe(object owner, NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds)
        {
            this.SubscribeInternal(
                owner: owner,
                callback: (own, change) => callback(change),
                cmds: cmds);
        }

        [DebuggerStepThrough]
        void INotifyingItemGetter<T>.Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, NotifyingSubscribeParameters cmds)
        {
            this.Subscribe<O>(
                owner: owner,
                callback: (own, change) => callback(own, change),
                cmds: cmds);
        }

        [DebuggerStepThrough]
        public void Subscribe(NotifyingSetItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.SubscribeInternal(
                owner: null,
                callback: (own, change) => callback(change),
                cmds: cmds);
        }

        [DebuggerStepThrough]
        public void Subscribe(object owner, NotifyingSetItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.SubscribeInternal(
                owner: owner,
                callback: (own, change) => callback(change),
                cmds: cmds);
        }

        [DebuggerStepThrough]
        public void Subscribe<O>(O owner, NotifyingSetItemCallback<O, T> callback, NotifyingSubscribeParameters cmds = null)
        {
            this.SubscribeInternal(
                owner: owner,
                callback: (own, change) => callback((O)own, change),
                cmds: cmds);
        }

        internal void SubscribeInternal(object owner, NotifyingSetItemInternalCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (subscribers == null)
            {
                subscribers = new SubscriptionHandler<NotifyingSetItemInternalCallback<T>>();
            }
            subscribers.Add(owner, callback);
            if (cmds.FireInitial)
            {
                callback(owner, new ChangeSet<T>(this.Item, newSet: this.HasBeenSet));
            }
        }

        public void Unsubscribe(object owner)
        {
            if (subscribers == null) return;
            subscribers.Remove(owner);
        }

        public virtual void Unset(NotifyingUnsetParameters cmds = null)
        {
            Set(value: default(T), 
                hasBeenSet: false, 
                cmds: cmds.ToFireParams());
        }

        public virtual void SetCurrentAsDefault()
        {
            throw new ArgumentException("Cannot set currenta s default on a notifying propery");
        }

        private void SetHasBeenSet(bool hasBeenSet)
        {
            var cmds = NotifyingFireParameters.Typical;
            var oldSet = this.HasBeenSet;
            this._HasBeenSet = hasBeenSet;
            if (cmds.ForceFire
                || oldSet != this.HasBeenSet)
            {
                if (subscribers != null && subscribers.HasSubs)
                {
                    Fire(new ChangeSet<T>(
                        oldVal: _item,
                        oldSet: oldSet,
                        newVal: _item,
                        newSet: true), cmds);
                }
            }
        }

        public void Set(T value, NotifyingFireParameters cmds = null)
        {
            cmds = cmds ?? NotifyingFireParameters.Typical;
            Set(value, hasBeenSet: true, cmds: cmds);
        }

        public virtual void Set(T value, bool hasBeenSet, NotifyingFireParameters cmds = null)
        {
            cmds = cmds ?? NotifyingFireParameters.Typical;
            var oldSet = this.HasBeenSet;
            this._HasBeenSet = hasBeenSet;
            if (cmds.ForceFire 
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
                        newSet: true), cmds);
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
            foreach (var sub in subscribers.GetSubs())
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

        void IHasBeenSetItem<T>.Set(T value, bool hasBeenSet) => Set(value, hasBeenSet, cmds: null);

        void IHasItem<T>.Unset() => Unset(cmds: null);

        public static implicit operator T(NotifyingSetItem<T> item)
        {
            return item.Item;
        }

        public override string ToString()
        {
            return $"{(this.HasBeenSet ? "Set" : "Unset")}: {Item?.ToString()}";
        }
    }
}
