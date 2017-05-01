using Noggog.Notifying;
using System;

namespace Noggog.Notifying
{
    public class HasBeenSetItem<T> : IHasBeenSetItem<T>
    {
        private T _Item;
        public T Item
        {
            get => _Item;
            set => Set(value);
        }
        public bool HasBeenSet { get; set; }
        public T DefaultValue { get; private set; }

        public T Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public HasBeenSetItem(
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            this.HasBeenSet = markAsSet;
        }

        public void Set(T item)
        {
            this._Item = item;
            this.HasBeenSet = true;
        }

        public void Unset()
        {
            this._Item = DefaultValue;
            this.HasBeenSet = false;
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this.Item;
        }
    }
}
