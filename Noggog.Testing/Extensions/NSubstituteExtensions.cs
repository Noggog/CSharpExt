using System;
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
    }
}