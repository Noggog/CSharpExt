using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            not.Set(value, NotifyingFireParameters.Typical);
        }

        public static void Unset<T>(this INotifyingItem<T> not)
        {
            not.Unset(null);
        }

        public static void SetToWithDefault<T>(
            this INotifyingItem<T> not,
            INotifyingItemGetter<T> rhs,
            INotifyingItemGetter<T> def,
            NotifyingFireParameters? cmds)
        {
            if (rhs.HasBeenSet)
            {
                not.Set(rhs.Value, cmds);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(def.Value, cmds);
            }
            else
            {
                not.Unset(cmds.ToUnsetParams());
            }
        }

        public static void SetToWithDefault<T>(
            this INotifyingItem<T> not,
            INotifyingItemGetter<T> rhs,
            INotifyingItemGetter<T> def,
            NotifyingFireParameters? cmds,
            Func<T, T, T> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.Set(converter(rhs.Value, def.Value), cmds);
                }
                else
                {
                    not.Set(converter(rhs.Value, default(T)), cmds);
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(converter(def.Value, default(T)), cmds);
            }
            else
            {
                not.Unset(cmds.ToUnsetParams());
            }
        }
    }
}
