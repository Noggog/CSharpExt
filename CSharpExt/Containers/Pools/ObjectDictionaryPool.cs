using System;
using System.Collections.Generic;

namespace Noggog.Containers.Pools
{
    public class ObjectDictionaryPool<K, V> : ObjectPool<Dictionary<K, V>>
    {
        public ObjectDictionaryPool(int maxPooledInstances = int.MaxValue)
            : base(
                  () => new Dictionary<K, V>(),
                  new LifecycleActions<Dictionary<K, V>>(
                      onReturn: (list) => list.Clear()),
                  maxPooledInstances)
        {
        }
    }
}
