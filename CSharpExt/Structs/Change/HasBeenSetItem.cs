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

namespace System
{
    public static class HasBeenSetItemExt
    {
        public static void SetToWithDefault<T>(
            this IHasBeenSetItem<T> not,
            IHasBeenSetItemGetter<T> rhs,
            IHasBeenSetItemGetter<T> def)
        {
            if (rhs.HasBeenSet)
            {
                not.Set(rhs.Value);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(def.Value);
            }
            else
            {
                not.Unset();
            }
        }

        public static void SetToWithDefault<T>(
            this IHasBeenSetItem<T> not,
            IHasBeenSetItemGetter<T> rhs,
            IHasBeenSetItemGetter<T> def,
            Func<T, T, T> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.Set(converter(rhs.Value, def.Value));
                }
                else
                {
                    not.Set(converter(rhs.Value, default(T)));
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(converter(def.Value, default(T)));
            }
            else
            {
                not.Unset();
            }
        }
    }
}