using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemNoNull<T> : NotifyingItem<T>
    {
        private readonly Func<T> noNullFallback;

        public NotifyingItemNoNull(
            Func<T> noNullFallback,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.noNullFallback = noNullFallback;
            if (defaultVal == null)
            {
                this._item = noNullFallback();
            }
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
