using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemNoNullConverter<T> : NotifyingItem<T>
    {
        Func<T> noNullFallback;
        Func<T, T> converter;

        public NotifyingItemNoNullConverter(
            Func<T> noNullFallback,
            Func<T, T> converter,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.noNullFallback = noNullFallback;
            this.converter = converter;
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                base.Set(converter(noNullFallback()), cmd);
            }
            else
            {
                base.Set(converter(value), cmd);
            }
        }
    }

    public class NotifyingItemNoNullDirectConverter<T> : NotifyingItem<T>
        where T : new()
    {
        Func<T, T> converter;

        public NotifyingItemNoNullDirectConverter(
            Func<T, T> converter,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.converter = converter;
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                base.Set(converter(new T()), cmd);
            }
            else
            {
                base.Set(converter(value), cmd);
            }
        }
    }
}
