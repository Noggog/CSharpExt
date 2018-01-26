using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemOnSet<T> : NotifyingItem<T>
    {
        Action<T> onSet;

        public NotifyingItemOnSet(
            Action<T> onSet,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.onSet = onSet;
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            base.Set(value, cmd);
            onSet(value);
        }
    }
}
