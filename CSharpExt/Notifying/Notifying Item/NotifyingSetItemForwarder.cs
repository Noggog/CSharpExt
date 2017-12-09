using System;
using System.Collections.Generic;

namespace Noggog.Notifying
{
    /*
    * Class useed to forward another notifying item's values. 
    * It contains logic to convert any subscribers to new objects, so unsubscriptions don't collide and accidentally unsub unwanted things.
    */
    public class NotifyingSetItemForwarder<T> : INotifyingSetItemGetter<T>
    {
        public bool HasBeenSet => toForward.HasBeenSet;

        public T Item => toForward.Item;

        INotifyingSetItemGetter<T> toForward;

        Lazy<Dictionary<WeakReferenceEquatable, object>> subscriberConverter = new Lazy<Dictionary<WeakReferenceEquatable, object>>();

        public NotifyingSetItemForwarder(INotifyingSetItemGetter<T> toForward)
        {
            this.toForward = toForward;
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback, bool fireInitial)
        {
            object transl = subscriberConverter.Value.TryCreateValue(new WeakReferenceEquatable(owner));
            toForward.Subscribe(transl, (owner2, changes) => callback(owner, changes), fireInitial);
        }

        public void Unsubscribe(object owner)
        {
            var weakRef = new WeakReferenceEquatable(owner);
            if (!subscriberConverter.IsValueCreated || !subscriberConverter.Value.TryGetValue(weakRef, out object transl)) return;
            toForward.Unsubscribe(transl);
            subscriberConverter.Value.Remove(weakRef);
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback, bool fireInitial)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(NotifyingItemSimpleCallback<T> callback)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<O>(O owner, NotifyingItemCallback<O, T> callback)
        {
            throw new NotImplementedException();
        }
    }
}
