using System.Linq;
using System.Reactive;
using FluentAssertions;
using Microsoft.Reactive.Testing;

namespace Noggog
{
    public static class TestableObservableExt
    {
        public static void ShouldHaveNoErrors<T>(this ITestableObservable<T> obs)
        {
            obs.Messages.Where(x => x.Value.Kind == NotificationKind.OnError)
                .Should()
                .BeEmpty();
        }
        
        public static void ShouldNotBeCompleted<T>(this ITestableObservable<T> obs)
        {
            obs.Messages.Where(x => x.Value.Kind == NotificationKind.OnCompleted)
                .Should()
                .BeEmpty();
        }
    }
}