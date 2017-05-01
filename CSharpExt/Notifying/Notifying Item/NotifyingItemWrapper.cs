using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public struct NotifyingItemWrapper<T> : INotifyingItemGetter<T>
    {
        public readonly T Item;

        public NotifyingItemWrapper(T item)
        {
            this.Item = item;
        }

        public bool HasBeenSet => true;

        T IHasBeenSetItemGetter<T>.Value => this.Item;

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
