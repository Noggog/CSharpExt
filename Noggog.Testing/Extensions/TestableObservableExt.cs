using System.Reactive;
using Microsoft.Reactive.Testing;
using Shouldly;

namespace Noggog;

public static class TestableObservableExt
{
    public static void ShouldHaveNoErrors<T>(this ITestableObservable<T> obs)
    {
        obs.Messages.Where(x => x.Value.Kind == NotificationKind.OnError)
            .ShouldBeEmpty();
    }
        
    public static void ShouldNotBeCompleted<T>(this ITestableObservable<T> obs)
    {
        obs.Messages.Where(x => x.Value.Kind == NotificationKind.OnCompleted)
            .ShouldBeEmpty();
    }
}