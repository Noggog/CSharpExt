using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemNoNullOnSetConverter<T> : NotifyingItem<T>
    {
        private readonly Func<T> noNullFallback;
        private readonly Action<T> onSet;
        private readonly Func<T, T> converter;

        public NotifyingItemNoNullOnSetConverter(
            Func<T> noNullFallback,
            Action<T> onSet,
            Func<T, T> converter,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.noNullFallback = noNullFallback;
            this.onSet = onSet;
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
                value = noNullFallback();
            }
            value = converter(value);
            base.Set(value, cmd);
            onSet(this.Item);
        }
    }

    public class NotifyingItemNoNullDirectOnSetConverter<T> : NotifyingItem<T>
        where T : new()
    {
        private readonly Action<T> onSet;
        private readonly Func<T, T> converter;

        public NotifyingItemNoNullDirectOnSetConverter(
            Action<T> onSet,
            Func<T, T> converter,
            T defaultVal = default(T))
            : base(defaultVal)
        {
            this.onSet = onSet;
            this.converter = converter;
        }

        public override void Set(T value, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                value = new T();
            }
            value = converter(value);
            base.Set(value, cmd);
            onSet(this.Item);
        }
    }
}
