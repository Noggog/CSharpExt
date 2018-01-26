using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemConverter<T> : NotifyingItem<T>
    {
        Func<T, T> converter;

        public NotifyingItemConverter(
            Func<T, T> converter,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.converter = converter;
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            base.Set(converter(value), cmd);
        }
    }
}
