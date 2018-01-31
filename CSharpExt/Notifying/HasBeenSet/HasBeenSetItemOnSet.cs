using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class HasBeenSetItemOnSet<T> : IHasBeenSetItem<T>
    {
        private T _Item;
        public T Item
        {
            get => _Item;
            set => Set(value);
        }
        public bool HasBeenSet { get; set; }
        public T DefaultValue { get; private set; }
        private Action<T> onSet;

        public HasBeenSetItemOnSet(
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.onSet = onSet;
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            this.HasBeenSet = markAsSet;
        }

        public void Set(T item, bool hasBeenSet = true)
        {
            this._Item = item;
            this.HasBeenSet = hasBeenSet;
            this.onSet(this._Item);
        }

        public void Unset()
        {
            this._Item = DefaultValue;
            this.HasBeenSet = false;
            this.onSet(this._Item);
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }
}
