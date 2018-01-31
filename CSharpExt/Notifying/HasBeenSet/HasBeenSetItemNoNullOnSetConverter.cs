using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class HasBeenSetItemNoNullOnSetConverter<T> : IHasBeenSetItem<T>
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
        Action<T> onSet;
        Func<T, T> converter;

        public HasBeenSetItemNoNullOnSetConverter(
            Func<T> defaultFallback,
            Action<T> onSet,
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.defaultFallback = defaultFallback;
            this.onSet = onSet;
            this.converter = converter;
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            if (defaultVal == null)
            {
                this._Item = defaultFallback();
            }
            else
            {
                this._Item = defaultVal;
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
            this.onSet(this._Item);
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
            this.onSet(this._Item);
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }

    public class HasBeenSetItemNoNullDirectOnSetConverter<T> : IHasBeenSetItem<T>
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
        Action<T> onSet;
        Func<T, T> converter;

        public HasBeenSetItemNoNullDirectOnSetConverter(
            Action<T> onSet,
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.converter = converter;
            this.DefaultValue = defaultVal;
            this.onSet = onSet;
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
            this.onSet(this._Item);
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
            this.onSet(this._Item);
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }

    public class HasBeenSetItemNoNullDirectOnSetConverter<T, Backup> : IHasBeenSetItem<T>
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
        Action<T> onSet;
        Func<T, T> converter;

        public HasBeenSetItemNoNullDirectOnSetConverter(
            Action<T> onSet,
            Func<T, T> converter,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.converter = converter;
            this.DefaultValue = defaultVal;
            this.onSet = onSet;
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
            this.onSet(this._Item);
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
            this.onSet(this._Item);
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }
}
