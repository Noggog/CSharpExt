using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog.Notifying
{
    public class SubscriptionHandler
    {
        internal static readonly object NEVER_UNSUB = new object();
    }

    public class SubscriptionHandler<T>
    {
        private Dictionary<WeakReferenceEquatable, FireList<T>> subscribers;
        private List<KeyValuePair<WeakReferenceEquatable, List<T>>> fireSubscribers;
        private bool reloadFireList = true;
        public bool HasSubs => subscribers?.Count > 0;

        public void Add(object owner, T item)
        {
            if (subscribers == null)
            {
                subscribers = new Dictionary<WeakReferenceEquatable, FireList<T>>();
            }
            if (owner == null)
            {
                owner = SubscriptionHandler.NEVER_UNSUB;
            }
            subscribers.TryCreateValue(
                new WeakReferenceEquatable(owner),
                () => new FireList<T>()).Add(item);
            reloadFireList = true;
        }

        public bool Remove(object owner)
        {
            if (subscribers == null) return false;
            if (owner == null) return false;
            var weakRef = new WeakReferenceEquatable(owner);
            if (subscribers.Remove(weakRef))
            {
                reloadFireList = true;
                return true;
            }
            return false;
        }

        public void Clear()
        {
            if (subscribers != null)
            {
                subscribers.Clear();
                subscribers = null;
            }
            if (fireSubscribers != null)
            {
                fireSubscribers.Clear();
                fireSubscribers = null;
            }
            reloadFireList = true;
        }

        public IEnumerable<KeyValuePair<WeakReferenceEquatable, List<T>>> GetSubs()
        {
            if (!this.HasSubs) return EnumerableExt<KeyValuePair<WeakReferenceEquatable, List<T>>>.EMPTY;
            if (!reloadFireList)
            {
                return fireSubscribers;
            }

            reloadFireList = false;

            // Get fresh list
            fireSubscribers = new List<KeyValuePair<WeakReferenceEquatable, List<T>>>(subscribers.Count);

            // Fill with callbacks
            foreach (var sub in subscribers)
            {
                if (!sub.Key.IsAlive) continue;
                // ToDo
                // Remove dead owners
                fireSubscribers.Add(new KeyValuePair<WeakReferenceEquatable, List<T>>(
                    sub.Key,
                    sub.Value.GetFireList()));
            }

            return fireSubscribers;
        }
    }
}
