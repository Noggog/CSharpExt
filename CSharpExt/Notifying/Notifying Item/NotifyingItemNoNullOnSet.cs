using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemNoNullOnSet<T> : NotifyingItem<T>
    {
        private readonly Func<T> noNullFallback;
        private readonly Action<T> onSet;

        public NotifyingItemNoNullOnSet(
            Func<T> noNullFallback,
            Action<T> onSet,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.noNullFallback = noNullFallback;
            this.onSet = onSet;
            if (defaultVal == null)
            {
                this._item = noNullFallback();
            }
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
        private readonly Action<T> onSet;

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
