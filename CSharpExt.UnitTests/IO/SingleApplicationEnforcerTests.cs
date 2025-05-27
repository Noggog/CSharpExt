using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Noggog.IO;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.Extensions;
using Shouldly;

namespace CSharpExt.UnitTests.IO;

public class SingleApplicationEnforcerTests
{
    [Theory, DefaultAutoData]
    public async Task SendingMessageIsReceivedOnExistingApp(string appName)
    {
        using var singleApp = new SingletonApplicationEnforcer(appName);

        var tcs = new TaskCompletionSource<IReadOnlyList<string>>();
        
        singleApp.WatchArgs()
            .SubscribeOn(TaskPoolScheduler.Default)
            .Take(1)
            .Subscribe(x => tcs.SetResult(x));

        await Task.Delay(100);
        
        singleApp.ForwardArgs(new []{ "Hello", "World" });

        (await tcs.Task).ShouldEqualEnumerable("Hello", "World");
    }
    
    [Theory, DefaultAutoData]
    public async Task SendingNonExistantAppThrows(string appName)
    {
        using var singleApp = new SingletonApplicationEnforcer(appName);

        Assert.Throws<FileNotFoundException>(() =>
        {
            singleApp.ForwardArgs(new[] { "Hello", "World" });
        });
    }
    
    [Theory, DefaultAutoData]
    public async Task SendingMultipleMessagesWithDelayGetsAll(string appName)
    {
        using var singleApp = new SingletonApplicationEnforcer(appName);

        var results = new List<IReadOnlyList<string>>();

        singleApp.WatchArgs()
            .SubscribeOn(TaskPoolScheduler.Default)
            .Subscribe(results.Add);

        await Task.Delay(100);
        
        singleApp.ForwardArgs(new []{ "Hello", "World" });

        await Task.Delay(100);
        singleApp.ForwardArgs(new []{ "What", "Is", "Up" });

        await Task.Delay(100);

        results.Count.ShouldBe(2);
        results[0].ShouldEqualEnumerable( "Hello", "World");
        results[1].ShouldEqualEnumerable("What", "Is", "Up");
    }
    
    [Theory, DefaultAutoData]
    public async Task SendingMultipleMessagesImmediatelyProblematic(string appName)
    {
        using var singleApp = new SingletonApplicationEnforcer(appName);

        var results = new List<IReadOnlyList<string>>();

        singleApp.WatchArgs()
            .SubscribeOn(TaskPoolScheduler.Default)
            .Subscribe(results.Add);

        await Task.Delay(1000);
        
        singleApp.ForwardArgs(new []{ "Hello", "World" });
        singleApp.ForwardArgs(new []{ "What", "Is", "Up" });

        await Task.Delay(5000);

        // Notified twice, but only see the last message twice
        results.Count.ShouldBe(2);
        results[0].ShouldEqualEnumerable("What", "Is", "Up");
        results[1].ShouldEqualEnumerable("What", "Is", "Up");
    }
}