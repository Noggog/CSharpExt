using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Noggog.IO;
using Xunit;

namespace CSharpExt.UnitTests.IO;

public class SingleApplicationEnforcerTests
{
    [Fact]
    public async Task SendingMessageIsReceivedOnExistingApp()
    {
        using var singleApp = new SingletonApplicationEnforcer(nameof(SendingMessageIsReceivedOnExistingApp));

        var tcs = new TaskCompletionSource<IReadOnlyList<string>>();
        
        singleApp.WatchArgs()
            .SubscribeOn(TaskPoolScheduler.Default)
            .Take(1)
            .Subscribe(x => tcs.SetResult(x));

        await Task.Delay(100);
        
        singleApp.ForwardArgs(new []{ "Hello", "World" });

        (await tcs.Task).Should().Equal("Hello", "World");
    }
    
    [Fact]
    public async Task SendingNonExistantAppThrows()
    {
        using var singleApp = new SingletonApplicationEnforcer(nameof(SendingNonExistantAppThrows));

        Assert.Throws<FileNotFoundException>(() =>
        {
            singleApp.ForwardArgs(new[] { "Hello", "World" });
        });
    }
    
    [Fact]
    public async Task SendingMultipleMessagesWithDelayGetsAll()
    {
        using var singleApp = new SingletonApplicationEnforcer(nameof(SendingMultipleMessagesWithDelayGetsAll));

        var results = new List<IReadOnlyList<string>>();

        singleApp.WatchArgs()
            .SubscribeOn(TaskPoolScheduler.Default)
            .Subscribe(results.Add);

        await Task.Delay(100);
        
        singleApp.ForwardArgs(new []{ "Hello", "World" });

        await Task.Delay(100);
        singleApp.ForwardArgs(new []{ "What", "Is", "Up" });

        await Task.Delay(100);

        results.Should().HaveCount(2);
        results[0].Should().Equal( "Hello", "World");
        results[1].Should().Equal("What", "Is", "Up");
    }
    
    [Fact]
    public async Task SendingMultipleMessagesImmediatelyProblematic()
    {
        using var singleApp = new SingletonApplicationEnforcer(nameof(SendingMultipleMessagesImmediatelyProblematic));

        var results = new List<IReadOnlyList<string>>();

        singleApp.WatchArgs()
            .SubscribeOn(TaskPoolScheduler.Default)
            .Subscribe(results.Add);

        await Task.Delay(100);
        
        singleApp.ForwardArgs(new []{ "Hello", "World" });
        singleApp.ForwardArgs(new []{ "What", "Is", "Up" });

        await Task.Delay(500);

        // Notified twice, but only see the last message twice
        results.Should().HaveCount(2);
        results[0].Should().Equal("What", "Is", "Up");
        results[1].Should().Equal("What", "Is", "Up");
    }
}