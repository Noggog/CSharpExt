using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemConverterOnSet<T> : NotifyingItem<T>
    {
        Func<T, T> converter;
        Action<T> onSet;

        public NotifyingItemConverterOnSet(
            Func<T, T> converter,
            Action<T> onSet,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.converter = converter;
            this.onSet = onSet;
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            value = converter(value);
            base.Set(value, cmd);
            onSet(value);
        }
    }
}
