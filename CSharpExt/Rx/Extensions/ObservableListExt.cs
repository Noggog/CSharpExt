using DynamicData;
using DynamicData.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class ObservableListExt
    {
        public static IObservable<IChangeSet<TObject>> DistinctValues<TObject>(this IObservable<IChangeSet<TObject>> source)
        {
            return DynamicData.ObservableListEx.DistinctValues(source, i => i);
        }

        public static IObservable<IChangeSet<TDestination>> TransformMany<TDestination, TSource>([NotNull] this IObservable<IChangeSet<TSource>> source,
            [NotNull] Func<TSource, IObservable<TDestination>> manyselector,
            IEqualityComparer<TDestination> equalityComparer = null)
        {
            return source
                .TransformMany(i =>
                {
                    return manyselector(i)
                        .ToObservableChangeSet()
                        .AsObservableList();
                },
                equalityComparer: equalityComparer);
        }
    }
}
