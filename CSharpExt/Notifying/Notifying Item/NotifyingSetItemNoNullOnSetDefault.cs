using System;
using System.Collections.Generic;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemNoNullOnSetDefault<T> : NotifyingSetItem<T>
    {
        private readonly Func<T> noNullFallback;
        private readonly Action<T> onSet;
        private T _defaultValue;
        public override T DefaultValue => _defaultValue;

        public NotifyingSetItemNoNullOnSetDefault(
            Func<T> noNullFallback,
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet)
        {
            this.noNullFallback = noNullFallback;
            this.onSet = onSet;
            this._defaultValue = defaultVal;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmd = null)
        {
            if (value == null)
            {
                value = noNullFallback();
            }
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
