using System;
using System.Collections.Generic;

namespace Noggog
{
    public static class QueueExt
    {
        public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> rhs)
        {
            foreach (T t in rhs)
            {
                queue.Enqueue(t);
            }
        }
    }
}
