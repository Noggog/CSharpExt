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
            NotifyingSubscribeParameters cmds = null)
        {
            SubscribeWithBoolGate<O, T>(gate, owner, item, (o2, change) => callback(change), customDetachCallback, cmds);
        }

        public static void SubscribeWithBoolGate<O, T>(
            this INotifyingItemGetter<bool> gate,
            O owner,
            INotifyingItemGetter<T> item,
            NotifyingItemCallback<O, T> callback,
            Action<T> customDetachCallback = null,
            NotifyingSubscribeParameters cmds = null)
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
                cmds);
            gate.Subscribe<O>(
                owner,
                (o2, change) =>
                {
                    if (change.New)
                    {
                        if (!attached)
                        {
                            attached = true;
                            callback(o2, new Change<T>(item.Item));
                        }
                    }
                    else
                    {
                        if (attached)
                        {
                            attached = false;
                            if (customDetachCallback != null)
                            {
                                customDetachCallback(item.Item);
                            }
                            else
                            {
                                callback(o2, new Change<T>(item.Item, default(T)));
                            }
                        }
                    }
                },
                cmds);
        }

        public static void Subscribe<O, T>(this INotifyingItemGetter<T> not, O owner, NotifyingItemCallback<O, T> callback)
        {
            not.Subscribe(owner, callback, NotifyingSubscribeParameters.Typical);
        }

        public static void Subscribe<O, T>(this INotifyingItemGetter<T> not, O owner, NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            not.Subscribe(owner, new NotifyingItemCallback<O, T>((o2, change) => callback(change)), cmds);
        }
        
        public static void Forward<T, R>(this INotifyingItemGetter<T> not, IHasItem<R> to, NotifyingSubscribeParameters cmds = null)
            where T : R
        {
            not.Subscribe(to, (change) => to.Item = change.New, cmds: cmds);
        }

        public static void Set<T>(this INotifyingItem<T> not, T value)
        {
            not.Set(value, NotifyingFireParameters.Typical);
        }

        public static void Unset<T>(this INotifyingSetItem<T> not)
        {
            not.Unset(null);
        }

        public static void SetToWithDefault<T>(
            this INotifyingSetItem<T> not,
            INotifyingSetItemGetter<T> rhs,
            INotifyingSetItemGetter<T> def,
            NotifyingFireParameters cmds)
        {
            if (rhs.HasBeenSet)
            {
                not.Set(rhs.Item, cmds);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(def.Item, cmds);
            }
            else
            {
                not.Unset(cmds.ToUnsetParams());
            }
        }

        public static void SetToWithDefault<T>(
            this INotifyingSetItem<T> not,
            INotifyingSetItemGetter<T> rhs,
            INotifyingSetItemGetter<T> def,
            NotifyingFireParameters cmds,
            Func<T, T, T> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def != null)
                {
                    not.Set(converter(rhs.Item, def.Item), cmds);
                }
                else
                {
                    not.Set(converter(rhs.Item, default(T)), cmds);
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(converter(def.Item, default(T)), cmds);
            }
            else
            {
                not.Unset(cmds.ToUnsetParams());
            }
        }
    }
}
