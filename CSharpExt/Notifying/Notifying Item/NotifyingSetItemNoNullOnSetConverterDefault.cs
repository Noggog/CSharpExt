using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemNoNullOnSetConverterDefault<T> : NotifyingSetItem<T>
    {
        private readonly Func<T> noNullFallback;
        private readonly Action<T> onSet;
        private readonly Func<T, T> converter;
        private T _defaultValue;
        public override T DefaultValue => _defaultValue;

        public NotifyingSetItemNoNullOnSetConverterDefault(
            Func<T> noNullFallback,
            Action<T> onSet,
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.noNullFallback = noNullFallback;
            this.onSet = onSet;
            this.converter = converter;
            this._defaultValue = defaultVal;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                value = noNullFallback();
            }
            value = converter(value);
            base.Set(value, hasBeenSet, cmd);
            onSet(this.Item);
        }

        public override void Unset(NotifyingUnsetParameters cmds = null)
        {
            HasBeenSet = false;
            Set(DefaultValue, cmds.ToFireParams());
        }

        public override void SetCurrentAsDefault()
        {
            this._defaultValue = this._item;
        }
    }
}
