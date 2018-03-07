using System;
using System.Collections;
using System.Collections.Generic;
using Noggog.Containers.Pools;

namespace Noggog.Notifying
{
    public class SubscriptionHandler<T>
    {
        private static readonly object NEVER_UNSUB = new object();
        
        Dictionary<WeakReferenceEquatable, FireList<T>> subscribers;
        List<KeyValuePair<WeakReferenceEquatable, List<T>>> fireSubscribers;
        private bool reloadFireList = true;
        public bool HasSubs => subscribers?.Count > 0;
        private object _lock = new object();

        public void Add(object owner, T item)
        {
            lock (_lock)
            {
                if (subscribers == null)
                {
                    subscribers = new Dictionary<WeakReferenceEquatable, FireList<T>>();
                }
                if (owner == null)
                {
                    owner = NEVER_UNSUB;
                }
                subscribers.TryCreateValue(
                    new WeakReferenceEquatable(owner),
                    () => new FireList<T>()).Add(item);
                reloadFireList = true;
            }
        }

        public bool Remove(object owner)
        {
            lock (_lock)
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
        }

        public void Clear()
        {
            lock (_lock)
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
        }

        public IEnumerable<KeyValuePair<WeakReferenceEquatable, List<T>>> GetSubs()
        {
            lock (_lock)
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
                    fireSubscribers.Add(new KeyValuePair<WeakReferenceEquatable, List<T>>(
                        sub.Key,
                        sub.Value.GetFireList()));
                }

                return fireSubscribers;
            }
        }
    }
}
