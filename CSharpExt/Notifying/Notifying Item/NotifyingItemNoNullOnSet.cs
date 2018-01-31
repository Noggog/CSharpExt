using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemNoNullOnSet<T> : NotifyingItem<T>
    {
        Func<T> noNullFallback;
        Action<T> onSet;

        public NotifyingItemNoNullOnSet(
            Func<T> noNullFallback,
            Action<T> onSet,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.noNullFallback = noNullFallback;
            this.onSet = onSet;
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                value = noNullFallback();
            }
            base.Set(value, cmd);
            onSet(value);
        }
    }

    public class NotifyingItemNoNullDirectOnSet<T> : NotifyingItem<T>
        where T : new()
    {
        Action<T> onSet;

        public NotifyingItemNoNullDirectOnSet(
            Action<T> onSet,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.onSet = onSet;
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                value = new T();
            }
            base.Set(value, cmd);
            onSet(value);
        }
    }
}
