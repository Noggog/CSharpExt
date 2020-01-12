using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class HasBeenSetItemConverter<T> : IHasBeenSetItem<T>
    {
        private T _Item;
        public T Item
        {
            get => _Item;
            set => Set(value);
        }
        public bool HasBeenSet { get; set; }
        public T DefaultValue { get; private set; }
        private Func<T, T> converter;

        public HasBeenSetItemConverter(
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.converter = converter;
            this.DefaultValue = defaultVal;
            this._Item = converter(defaultVal);
            this.HasBeenSet = markAsSet;
        }

        public void Set(T item, bool hasBeenSet = true)
        {
            this._Item = converter(item);
            this.HasBeenSet = hasBeenSet;
        }

        public void Unset()
        {
            this._Item = converter(DefaultValue);
            this.HasBeenSet = false;
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }
}
