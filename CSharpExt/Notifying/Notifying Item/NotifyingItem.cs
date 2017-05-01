using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
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

        void IHasBeenSetItem<T>.Set(T value) => Set(value, cmd: null);

        void IHasBeenSetItem<T>.Unset() => Unset(cmds: null);
        
        public static implicit operator T(NotifyingItem<T> item)
        {
            return item.Value;
        }
    }
}
