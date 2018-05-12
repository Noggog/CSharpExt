using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemNoNullConverterDefault<T> : NotifyingSetItem<T>
    {
        private readonly Func<T> noNullFallback;
        private readonly Func<T, T> converter;
        private T _defaultValue;
        public override T DefaultValue => _defaultValue;

        public NotifyingSetItemNoNullConverterDefault(
            Func<T> noNullFallback,
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.noNullFallback = noNullFallback;
            this.converter = converter;
            this._defaultValue = defaultVal;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            if (value == null)
            {
                base.Set(converter(noNullFallback()), hasBeenSet, cmd);
            }
            else
            {
                base.Set(converter(value), hasBeenSet, cmd);
            }
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
