using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public class HasBeenSetItemNoNull<T> : IHasBeenSetItem<T>
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

        public T Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public HasBeenSetItemNoNull(
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
            this.DefaultValue = this.Item;
        }
    }

    public class HasBeenSetItemNoNull<T, Backup> : IHasBeenSetItem<T>
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

        public T Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public HasBeenSetItemNoNull(
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
            this.DefaultValue = this.Item;
        }
    }
}
