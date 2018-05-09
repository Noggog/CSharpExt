using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemOnSet<T> : NotifyingSetItem<T>
    {
        private readonly Action<T> onSet;

        public NotifyingSetItemOnSet(
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.onSet = onSet;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            base.Set(value, hasBeenSet, cmd);
            onSet(value);
        }
    }
}
