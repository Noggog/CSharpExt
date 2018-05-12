using System;
using System.Collections.Generic;
using System.Diagnostics;
using Noggog.Notifying;

namespace Noggog.Notifying
{
    public class NotifyingSetItemDefault<T> : NotifyingSetItem<T>
    {
        private T _defaultValue;
        public override T DefaultValue => _defaultValue;

        public NotifyingSetItemDefault(
            T defaultVal = default(T),
            bool markAsSet = false)
            : base(defaultVal, markAsSet: markAsSet)
        {
            this._defaultValue = defaultVal;
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
