using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemNoNull<T> : NotifyingItem<T>
    {
        Func<T> noNullFallback;

        public NotifyingItemNoNull(
            Func<T> noNullFallback,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.noNullFallback = noNullFallback;
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                base.Set(noNullFallback(), cmd);
            }
            else
            {
                base.Set(value, cmd);
            }
        }
    }

    public class NotifyingItemNoNullDirect<T> : NotifyingItem<T>
        where T : new()
    {
        public NotifyingItemNoNullDirect(
            T defaultVal = default(T))
            : base(defaultVal)
        {
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                base.Set(new T(), cmd);
            }
            else
            {
                base.Set(value, cmd);
            }
        }
    }
}
