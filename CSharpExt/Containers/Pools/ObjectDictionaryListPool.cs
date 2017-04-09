using System;
using System.Collections.Generic;

namespace Noggog.Containers.Pools
{
    public class ObjectDictionaryListPool<K, T> : ObjectPool<Dictionary<K, List<T>>>
    {
        public ObjectDictionaryListPool(
            ObjectListPool<T> targetListPool,
            int maxPooledInstances = int.MaxValue)
            : base(
                  () => new Dictionary<K, List<T>>(),
                  new LifecycleActions<Dictionary<K, List<T>>>(
                      onReturn: (list) =>
                      {
                          foreach (var subList in list.Values)
                          {
                              targetListPool.Return(subList);
                          }
                          list.Clear();
                      }),
                  maxPooledInstances)
        {
        }
    }
}
