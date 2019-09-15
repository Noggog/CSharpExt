using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class ChangeSetExt
    {
        /*
         * Convenience function to turn a single item observable into a change set,
         * which removes itself when null, and add itself when non-null
         */
        public static IObservable<IChangeSet<T>> ToObservableChangeSet_SingleItemNotNull<T>(this IObservable<T> obs)
            where T : class
        {
            Optional<T> prev = default;
            return obs
                .DistinctUntilChanged(i => i == null)
                .Where(i => i != null || prev.HasValue)
                .Select(item =>
                {
                    if (item != null)
                    {
                        var ret = new ChangeSet<T>(
                            new Change<T>(
                                reason: ListChangeReason.Add,
                                current: item,
                                previous: Optional<T>.None)
                                .Single());
                        prev = item;
                        return ret;
                    }
                    else
                    {
                        var ret = new ChangeSet<T>(
                            new Change<T>(
                                reason: ListChangeReason.Remove,
                                current: null,
                                previous: prev)
                                .Single());
                        prev = Optional<T>.None;
                        return ret;
                    }
                });
        }
    }
}
