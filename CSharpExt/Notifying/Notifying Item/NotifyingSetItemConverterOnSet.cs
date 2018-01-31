using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemConverterOnSet<T> : NotifyingSetItem<T>
    {
        Func<T, T> converter;
        Action<T> onSet;

        public NotifyingSetItemConverterOnSet(
            Func<T, T> converter,
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.converter = converter;
            this.onSet = onSet;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            value = converter(value);
            base.Set(value, hasBeenSet, cmd);
            onSet(value);
        }
    }
}
