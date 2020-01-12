using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class HasBeenSetItemNoNull<T> : IHasBeenSetItem<T>
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

        public HasBeenSetItemNoNull(
            Func<T> defaultFallback,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.defaultFallback = defaultFallback;
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
                this._Item = defaultFallback();
            }
            else
            {
                this._Item = item;
            }
            this.HasBeenSet = hasBeenSet;
        }

        public void Unset()
        {
            if (this.DefaultValue == null)
            {
                this._Item = defaultFallback();
            }
            else
            {
                this._Item = this.DefaultValue;
            }
            this.HasBeenSet = false;
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }

    public class HasBeenSetItemNoNullDirect<T> : IHasBeenSetItem<T>
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

        public HasBeenSetItemNoNullDirect(
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            if (defaultVal == null)
            {
                this._Item = new T();
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
                this._Item = new T();
            }
            else
            {
                this._Item = item;
            }
            this.HasBeenSet = hasBeenSet;
        }

        public void Unset()
        {
            if (this.DefaultValue == null)
            {
                this._Item = new T();
            }
            else
            {
                this._Item = this.DefaultValue;
            }
            this.HasBeenSet = false;
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }

    public class HasBeenSetItemNoNullDirect<T, Backup> : IHasBeenSetItem<T>
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

        public HasBeenSetItemNoNullDirect(
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.DefaultValue = defaultVal;
            this._Item = defaultVal;
            if (defaultVal == null)
            {
                this._Item = new Backup();
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
                this._Item = new Backup();
            }
            else
            {
                this._Item = item;
            }
            this.HasBeenSet = hasBeenSet;
        }

        public void Unset()
        {
            if (this.DefaultValue == null)
            {
                this._Item = new Backup();
            }
            else
            {
                this._Item = this.DefaultValue;
            }
            this.HasBeenSet = false;
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }
}
