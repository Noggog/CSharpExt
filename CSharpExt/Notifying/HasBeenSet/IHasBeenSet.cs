using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public interface IHasItem<T> : IHasItemGetter<T>
    {
        T DefaultValue { get; }
        new T Item { get; set; }
        void Unset();
    }

    public interface IHasItemGetter<out T>
    {
        T Item { get; }
    }

    public interface IHasBeenSet : IHasBeenSetGetter
    {
        new bool HasBeenSet { get; set; }
        void Unset();
    }

    public interface IHasBeenSetGetter
    {
        bool HasBeenSet { get; }
    }

    public interface IHasBeenSetItemGetter<out T> : IHasBeenSetGetter, IHasItemGetter<T>
    {
    }

    public interface IHasBeenSetItem<T> : IHasItem<T>, IHasBeenSetItemGetter<T>, IHasBeenSet
    {
        new T Item { get; set; }
        void Set(T item, bool hasBeenSet = true);
        new void Unset();
    }
}

namespace Noggog
{
    public static class IHasItemExt
    {
        public static T GetOrDefault<T>(this IHasBeenSetItemGetter<T> getter, T def)
        {
            if (getter.HasBeenSet) return getter.Item;
            return def;
        }

        public static void SetIfNotSet<T>(this IHasBeenSetItem<T> prop, T item, bool markAsSet = true)
        {
            if (prop.HasBeenSet) return;
            prop.Set(item, markAsSet);
        }
    }
}
