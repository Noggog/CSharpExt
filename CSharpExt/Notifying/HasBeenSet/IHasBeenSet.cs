using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public interface IHasItem<T> : IHasItemGetter<T>
    {
        new T Item { get; set; }
    }

    public interface IHasItemGetter<T>
    {
        T Item { get; }
    }

    public interface IHasBeenSet
    {
        bool HasBeenSet { get; }
    }

    public interface IHasBeenSetItemGetter<T> : IHasBeenSet, IHasItemGetter<T>
    {
    }

    public interface IHasBeenSetItem<T> : IHasItem<T>, IHasBeenSetItemGetter<T>
    {
        T DefaultValue { get; }
        new T Item { get; set; }
        new bool HasBeenSet { get; set; }
        void Set(T value);
        void Unset();
        void SetCurrentAsDefault();
    }
}
