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

        public void Subscribe(NotifyingItemSimpleCallback<T> callback, bool fireInitial)
        {
            if (fireInitial)
            {
                callback(new Change<T>(Item));
            }
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback)
        {
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback)
        {
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
