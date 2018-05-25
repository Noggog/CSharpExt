using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemConverterDefault<T> : NotifyingSetItem<T>
    {
        private readonly Func<T, T> converter;
        private T _defaultValue;
        public override T DefaultValue => _defaultValue;

        public NotifyingSetItemConverterDefault(
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.converter = converter;
            this._defaultValue = defaultVal;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            base.Set(converter(value), hasBeenSet, cmd);
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
