using System;
using System.Collections.Generic;
using Noggog.Containers.Pools;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemNoNullConverter<T> : NotifyingSetItem<T>
    {
        Func<T> noNullFallback;
        Func<T, T> converter;

        public NotifyingSetItemNoNullConverter(
            Func<T> noNullFallback,
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.noNullFallback = noNullFallback;
            this.converter = converter;
        }

        public override void Set(T value, NotifyingFireParameters? cmd = default(NotifyingFireParameters?))
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

    public class NotifyingSetItemNoNullDirectConverter<T> : NotifyingSetItem<T>
        where T : new()
    {
        Func<T, T> converter;

        public NotifyingSetItemNoNullDirectConverter(
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.converter = converter;
        }

        public override void Set(T value, NotifyingFireParameters? cmd = default(NotifyingFireParameters?))
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
