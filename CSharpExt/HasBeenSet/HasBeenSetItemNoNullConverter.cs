using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class HasBeenSetItemNoNullConverter<T> : IHasBeenSetItem<T>
    {
        private T _Item;
        public T Item
        {
            get => _Item;
            set => Set(value);
        }
        public bool HasBeenSet { get; set; }
        public T DefaultValue { get; private set; }
        Func<T> defaultFallback;
        Func<T, T> converter;

        public HasBeenSetItemNoNullConverter(
            Func<T> defaultFallback,
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.defaultFallback = defaultFallback;
            this.converter = converter;
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            if (defaultVal == null)
            {
                this._Item = converter(defaultFallback());
            }
            else
            {
                this._Item = converter(defaultVal);
            }
            this.HasBeenSet = markAsSet;
        }

        public void Set(T item, bool hasBeenSet = true)
        {
            if (item == null)
            {
                this._Item = converter(defaultFallback());
            }
            else
            {
                this._Item = converter(item);
            }
            this.HasBeenSet = hasBeenSet;
        }

        public void Unset()
        {
            if (this.DefaultValue == null)
            {
                this._Item = converter(defaultFallback());
            }
            else
            {
                this._Item = converter(this.DefaultValue);
            }
            this.HasBeenSet = false;
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this.Item;
        }
    }

    public class HasBeenSetItemNoNullDirectConverter<T> : IHasBeenSetItem<T>
        where T : new()
    {
        private T _Item;
        public T Item
        {
            get => _Item;
            set => Set(value);
        }
        public bool HasBeenSet { get; set; }
        public T DefaultValue { get; private set; }
        Func<T, T> converter;

        public HasBeenSetItemNoNullDirectConverter(
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.converter = converter;
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            if (defaultVal == null)
            {
                this._Item = converter(new T());
            }
            else
            {
                this._Item = converter(defaultVal);
            }
            this.HasBeenSet = markAsSet;
        }

        public void Set(T item, bool hasBeenSet = true)
        {
            if (item == null)
            {
                this._Item = converter(new T());
            }
            else
            {
                this._Item = converter(item);
            }
            this.HasBeenSet = hasBeenSet;
        }

        public void Unset()
        {
            if (this.DefaultValue == null)
            {
                this._Item = converter(new T());
            }
            else
            {
                this._Item = converter(this.DefaultValue);
            }
            this.HasBeenSet = false;
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }

    public class HasBeenSetItemNoNullDirectConverter<T, Backup> : IHasBeenSetItem<T>
        where Backup : T, new()
    {
        private T _Item;
        public T Item
        {
            get => _Item;
            set => Set(value);
        }
        public bool HasBeenSet { get; set; }
        public T DefaultValue { get; private set; }
        Func<T, T> converter;

        public HasBeenSetItemNoNullDirectConverter(
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.converter = converter;
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            if (defaultVal == null)
            {
                this._Item = converter(new Backup());
            }
            else
            {
                this._Item = converter(defaultVal);
            }
            this.HasBeenSet = markAsSet;
        }

        public void Set(T item, bool hasBeenSet = true)
        {
            if (item == null)
            {
                this._Item = converter(new Backup());
            }
            else
            {
                this._Item = converter(item);
            }
            this.HasBeenSet = hasBeenSet;
        }

        public void Unset()
        {
            if (this.DefaultValue == null)
            {
                this._Item = converter(new Backup());
            }
            else
            {
                this._Item = converter(this.DefaultValue);
            }
            this.HasBeenSet = false;
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }
}
