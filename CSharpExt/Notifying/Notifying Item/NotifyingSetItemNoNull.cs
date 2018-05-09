using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemNoNull<T> : NotifyingSetItem<T>
    {
        private readonly Func<T> noNullFallback;

        public NotifyingSetItemNoNull(
            Func<T> noNullFallback,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.noNullFallback = noNullFallback;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                base.Set(noNullFallback(), hasBeenSet, cmd);
            }
            else
            {
                base.Set(value, hasBeenSet, cmd);
            }
        }
    }

    public class NotifyingSetItemNoNullDirect<T> : NotifyingSetItem<T>
        where T : new()
    {
        public NotifyingSetItemNoNullDirect(
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                base.Set(new T(), hasBeenSet, cmd);
            }
            else
            {
                base.Set(value, hasBeenSet, cmd);
            }
        }
    }
}
