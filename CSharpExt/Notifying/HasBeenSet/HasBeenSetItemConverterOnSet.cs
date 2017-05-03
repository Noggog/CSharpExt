using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class HasBeenSetItemConverterOnSet<T> : IHasBeenSetItem<T>
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
        private Action<T> onSet;

        public HasBeenSetItemConverterOnSet(
            Func<T, T> converter,
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.converter = converter;
            this.onSet = onSet;
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            this.HasBeenSet = markAsSet;
        }

        public void Set(T item)
        {
            this._Item = converter(item);
            this.HasBeenSet = true;
            this.onSet(this._Item);
        }

        public void Unset()
        {
            this._Item = converter(DefaultValue);
            this.HasBeenSet = false;
            this.onSet(this._Item);
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }
}
