using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class HasBeenSetItemNoNullOnSet<T> : IHasBeenSetItem<T>
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

        public HasBeenSetItemNoNullOnSet(
            Func<T> defaultFallback,
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.defaultFallback = defaultFallback;
            this.onSet = onSet;
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

        public void Set(T item)
        {
            if (item == null)
            {
                this._Item = defaultFallback();
            }
            else
            {
                this._Item = item;
            }
            this.HasBeenSet = true;
            this.onSet(this._Item);
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
            this.onSet(this._Item);
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }

    public class HasBeenSetItemNoNullDirectOnSet<T> : IHasBeenSetItem<T>
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

        public HasBeenSetItemNoNullDirectOnSet(
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.DefaultValue = defaultVal;
            this.onSet = onSet;
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

        public void Set(T item)
        {
            if (item == null)
            {
                this._Item = new T();
            }
            else
            {
                this._Item = item;
            }
            this.HasBeenSet = true;
            this.onSet(this._Item);
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
            this.onSet(this._Item);
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }

    public class HasBeenSetItemNoNullDirectOnSet<T, Backup> : IHasBeenSetItem<T>
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

        public HasBeenSetItemNoNullDirectOnSet(
            Action<T> onSet,
            T defaultVal = default(T),
            bool markAsSet = false)
        {
            this.DefaultValue = defaultVal;
            this.onSet = onSet;
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

        public void Set(T item)
        {
            if (item == null)
            {
                this._Item = new Backup();
            }
            else
            {
                this._Item = item;
            }
            this.HasBeenSet = true;
            this.onSet(this._Item);
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
            this.onSet(this._Item);
        }

        public void SetCurrentAsDefault()
        {
            this.DefaultValue = this._Item;
        }
    }
}
