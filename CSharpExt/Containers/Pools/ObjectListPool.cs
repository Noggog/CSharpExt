using System;
using System.Collections.Generic;

namespace Noggog.Containers.Pools
{
    public class ObjectListPool<T> : ObjectPool<List<T>>
    {
        public ObjectListPool(int maxPooledInstances = int.MaxValue)
            : base(
                  () => new List<T>(),
                  new LifecycleActions<List<T>>(
                      onReturn: (list) => list.Clear(),
                      onGet: (list) =>
                      {
                          if (list.Count > 0)
                          {
                              throw new DataMisalignedException();
                          }
                      }),
                  maxPooledInstances)
        {
        }
    }
}
