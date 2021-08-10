using System;
using System.Linq;
using System.Reactive.Linq;
using NSubstitute;
using NSubstitute.Core;

namespace Noggog.NSubstitute
{
    public static class NSubstituteExtensions
    {
        public static ConfiguredCall ReturnEmpty<T>(this IObservable<T> obs)
        {
            return obs.Returns(Observable.Empty<T>());
        }

        public static T ArgIsSame<T>(this T item)
            where T : class
        {
            return Arg.Is<T>(x => object.ReferenceEquals(x, item));
        }

        public static ConfiguredCall ReturnsSerially<T>(
            this T value,
            params T[] returnThese)
        {
            return value.Returns(returnThese[0], returnThese.Skip(1).ToArray());
        }

        public static ConfiguredCall ReturnsSeriallyForAnyArgs<T>(
            this T value,
            params T[] returnThese)
        {
            return value.ReturnsForAnyArgs(returnThese[0], returnThese.Skip(1).ToArray());
        }
    }
}