using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public struct NotifyingSetItemWrapper<T> : INotifyingSetItemGetter<T>
    {
        public readonly T _item;
        public T Item => _item;

        public NotifyingSetItemWrapper(T item)
        {
            this._item = item;
        }

        public bool HasBeenSet => true;

        public void Subscribe(Action callback, NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (cmds.FireInitial)
            {
                callback();
            }
        }

        public void Subscribe(object owner, Action callback, NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (cmds.FireInitial)
            {
                callback();
            }
        }

        public void Subscribe(object owner, NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (cmds.FireInitial)
            {
                callback(new Change<T>(Item));
            }
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback, NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (cmds.FireInitial)
            {
                callback(new Change<T>(Item));
            }
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, NotifyingSubscribeParameters cmds = null)
        {
            cmds = cmds ?? NotifyingSubscribeParameters.Typical;
            if (cmds.FireInitial)
            {
                callback(owner, new Change<T>(Item));
            }
        }

        void INotifyingItemGetter<T>.Unsubscribe(object owner)
        {
        }
    }
}
