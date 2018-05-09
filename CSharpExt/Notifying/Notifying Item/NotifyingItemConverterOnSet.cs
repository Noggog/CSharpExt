using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingItemConverterOnSet<T> : NotifyingItem<T>
    {
        private readonly Func<T, T> converter;
        private readonly Action<T> onSet;

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
