using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemConverter<T> : NotifyingSetItem<T>
    {
        Func<T, T> converter;

        public NotifyingSetItemConverter(
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.converter = converter;
        }

        public override void Set(T value, NotifyingFireParameters? cmd = default(NotifyingFireParameters?))
        {
            base.Set(converter(value), cmd);
        }
    }
}
