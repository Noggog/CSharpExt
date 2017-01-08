using System;
using System.Collections;
using System.Collections.Generic;
using Noggog.Containers.Pools;

namespace Noggog.Notifying
{
    public class SubscriptionHandler<T>
    {
        public readonly static ObjectListPool<T> pool = new ObjectListPool<T>(500);
        readonly static ObjectDictionaryListPool<WeakReferenceEquatable, T> dictPool = new ObjectDictionaryListPool<WeakReferenceEquatable, T>(pool, 250);

        Dictionary<WeakReferenceEquatable, List<T>> subscribers;
        Dictionary<WeakReferenceEquatable, List<T>> fireSubscribers;
        private bool reloadFireList = true;
        public bool HasSubs { get { return subscribers?.Count > 0; } }

        ~SubscriptionHandler()
        {
            if (subscribers != null)
            {
                dictPool.Return(subscribers);
                subscribers = null;
            }
            if (fireSubscribers != null)
            {
                dictPool.Return(fireSubscribers);
                fireSubscribers = null;
            }
        }

        public void Add(object owner, T item)
        {
            if (subscribers == null)
            {
                subscribers = dictPool.Get();
            }
            subscribers.TryCreateValue(
                new WeakReferenceEquatable(owner),
                () => pool.Get()).Add(item);
            reloadFireList = true;
        }

        public bool Remove(object owner)
        {
            if (subscribers == null) return false;
            List<T> list;
            var weakRef = new WeakReferenceEquatable(owner);
            if (subscribers.TryGetValue(weakRef, out list))
            {
                if (!subscribers.Remove(weakRef))
                {
                    throw new DataMisalignedException();
                }
                pool.Return(list);
                reloadFireList = true;
                return true;
            }
            return false;
        }

        public void Clear()
        {
            if (subscribers != null)
            {
                dictPool.Return(subscribers);
                subscribers = null;
            }
            if (fireSubscribers != null)
            {
                dictPool.Return(fireSubscribers);
                fireSubscribers = null;
            }
            reloadFireList = true;
        }

        public struct SubscriptionCheckout : IDisposable, IEnumerable<KeyValuePair<object, List<T>>>
        {
            private readonly Dictionary<WeakReferenceEquatable, List<T>> subscribers;
            private readonly Dictionary<WeakReferenceEquatable, List<T>> oldList;

            public SubscriptionCheckout(
                Dictionary<WeakReferenceEquatable, List<T>> old,
                Dictionary<WeakReferenceEquatable, List<T>> toFire)
            {
                this.subscribers = toFire;
                this.oldList = old;
            }

            public void Dispose()
            {
                if (oldList != null)
                {
                    pool.Return(oldList.Values);
                    dictPool.Return(oldList);
                }
            }

            public IEnumerator<KeyValuePair<object, List<T>>> GetEnumerator()
            {
                if (this.subscribers == null) yield break;
                foreach (var sub in this.subscribers)
                {
                    var item = sub.Key.Target;
                    if (!sub.Key.IsAlive)
                    {
                        this.subscribers.Remove(sub.Key);
                        continue;
                    }
                    yield return new KeyValuePair<object, List<T>>(item, sub.Value);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public SubscriptionCheckout GetSubs()
        {
            if (!this.HasSubs) return new SubscriptionCheckout();
            if (!reloadFireList) return new SubscriptionCheckout(null, this.fireSubscribers);

            reloadFireList = false;
            var oldList = fireSubscribers;

            // Get fresh list
            fireSubscribers = dictPool.Get();

            // Fill with callbacks
            foreach (var sub in subscribers)
            {
                if (!sub.Key.IsAlive) continue;
                var list = pool.Get();
                foreach (var item in sub.Value)
                {
                    list.Add(item);
                }
                fireSubscribers[sub.Key] = list;
            }

            return new SubscriptionCheckout(oldList, fireSubscribers);
        }
    }
}
