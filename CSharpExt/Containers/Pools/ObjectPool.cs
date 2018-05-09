using System;
using System.Collections.Generic;

namespace Noggog.Containers.Pools
{
    public class ObjectPool<T>
    {
        private static ObjectPool<T> _instance;

        public int MaxInstancesPooled;
        private readonly Queue<T> storage = new Queue<T>();
        private readonly LifecycleActions<T> actions;
        private readonly Func<T> creator;

        public ObjectPool(
            Func<T> creator,
            LifecycleActions<T> actions = new LifecycleActions<T>(),
            int maxInstances = int.MaxValue)
        {
            this.creator = creator;
            this.actions = actions;
            this.MaxInstancesPooled = maxInstances;
        }

        public T Get()
        {
            T t;
            if (storage.Count == 0)
            {
                t = creator();
                actions.OnCreate?.Invoke(t);
            }
            else
            {
                t = storage.Dequeue();
            }
            actions.OnGet?.Invoke(t);
            return t;
        }

        public ObjectPoolCheckout<T> Checkout()
        {
            return new ObjectPoolCheckout<T>(Get(), this);
        }

        public void Return(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Return(item);
            }
        }

        public virtual bool Return(T item)
        {
            if (item == null) return false;

            actions.OnReturn?.Invoke(item);

            if (storage.Count < MaxInstancesPooled)
            {
                storage.Enqueue(item);
                return true;
            }

            actions.OnDestroy?.Invoke(item);
            return false;
        }

        public static ObjectPool<T> Instance<R>()
            where R : T, new()
        {
            if (_instance == null)
            {
                var tmp = new ObjectPool<T>(() => new R());
                _instance = tmp;
            }
            return _instance;
        }
    }

    public struct ObjectPoolCheckout<T> : IDisposable
    {
        public T Item;
        private ObjectPool<T> pool;

        internal ObjectPoolCheckout(T item, ObjectPool<T> pool)
        {
            this.pool = pool;
            this.Item = item;
        }

        public void Dispose()
        {
            pool?.Return(Item);
        }
    }
}
