using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemNoNullOnSet<T> : NotifyingSetItem<T>
    {
        private readonly Func<T> noNullFallback;
        private readonly Action<T> onSet;

        public NotifyingSetItemNoNullOnSet(
            Func<T> noNullFallback,
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.noNullFallback = noNullFallback;
            this.onSet = onSet;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = null)
        {
            if (value == null)
            {
                value = noNullFallback();
            }
            base.Set(value, hasBeenSet, cmd);
            onSet(value);
        }
    }

    public class NotifyingSetItemNoNullDirectOnSet<T> : NotifyingSetItem<T>
        where T : new()
    {
        private readonly Action<T> onSet;

        public NotifyingSetItemNoNullDirectOnSet(
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.onSet = onSet;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = null)
        {
            if (value == null)
            {
                value = new T();
            }
            base.Set(value, hasBeenSet, cmd);
            onSet(value);
        }
    }
}
