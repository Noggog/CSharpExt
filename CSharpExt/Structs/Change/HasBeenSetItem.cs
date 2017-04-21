using Noggog.Notifying;
using System;

namespace Noggog.Notifying
{
    public class HasBeenSetGetter : IHasBeenSet
    {
        public static readonly HasBeenSetGetter NotBeenSet_Instance = new HasBeenSetGetter(false);
        public static readonly HasBeenSetGetter HasBeenSet_Instance = new HasBeenSetGetter(true);

        public readonly bool HasBeenSet;
        bool IHasBeenSet.HasBeenSet => this.HasBeenSet;

        public HasBeenSetGetter(bool on)
        {
            this.HasBeenSet = on;
        }
    }

    // Keep class, as it needs pass by reference
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

    public interface IHasBeenSet
    {
        bool HasBeenSet { get; }
    }

    public interface IHasBeenSetItemGetter<T> : IHasBeenSet
    {
        T Value { get; }
    }

    public interface IHasBeenSetItem<T> : IHasBeenSetItemGetter<T>
    {
        T DefaultValue { get; }
        new T Value { get; set; }
        new bool HasBeenSet { get; set; }
        void Set(T value);
        void Unset();
        void SetCurrentAsDefault();
    }
}
