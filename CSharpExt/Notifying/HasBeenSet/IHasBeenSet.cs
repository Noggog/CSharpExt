using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Notifying
{
    public interface IHasBeenSet
    {
        bool HasBeenSet { get; }
    }

    public interface IHasBeenSetItemGetter<T> : IHasBeenSet
    {
        T Item { get; }
    }

    public interface IHasBeenSetItem<T> : IHasBeenSetItemGetter<T>
    {
        T DefaultValue { get; }
        new T Item { get; set; }
        new bool HasBeenSet { get; set; }
        void Set(T value);
        void Unset();
        void SetCurrentAsDefault();
    }
}
