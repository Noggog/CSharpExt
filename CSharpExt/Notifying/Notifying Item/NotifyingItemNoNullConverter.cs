using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemNoNullConverter<T> : NotifyingItem<T>
    {
        private readonly Func<T> noNullFallback;
        private readonly Func<T, T> converter;

        public NotifyingItemNoNullConverter(
            Func<T> noNullFallback,
            Func<T, T> converter,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.noNullFallback = noNullFallback;
            this.converter = converter;
            if (defaultVal == null)
            {
                this._item = converter(noNullFallback());
            }
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
        private readonly Func<T, T> converter;

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
