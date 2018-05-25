using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemConverterOnSetDefault<T> : NotifyingSetItem<T>
    {
        private readonly Func<T, T> converter;
        private readonly Action<T> onSet;
        private T _defaultValue;
        public override T DefaultValue => _defaultValue;

        public NotifyingSetItemConverterOnSetDefault(
            Func<T, T> converter,
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.converter = converter;
            this.onSet = onSet;
            this._defaultValue = defaultVal;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = default(NotifyingFireParameters))
        {
            value = converter(value);
            base.Set(value, hasBeenSet, cmd);
            onSet(value);
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
