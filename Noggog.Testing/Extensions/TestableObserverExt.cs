using System.Reactive;
using Microsoft.Reactive.Testing;
using Shouldly;

namespace Noggog;

public static class TestableObserverExt
{
    public static void ShouldHaveNoErrors<T>(this ITestableObserver<T> obs)
    {
        obs.Messages.Where(x => x.Value.Kind == NotificationKind.OnError)
            .ShouldBeEmpty();
    }
        
    public static void ShouldNotBeCompleted<T>(this ITestableObserver<T> obs)
    {
        obs.Messages.Where(x => x.Value.Kind == NotificationKind.OnCompleted)
            .ShouldBeEmpty();
    }
}