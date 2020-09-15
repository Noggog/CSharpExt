using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using Xunit;
using Noggog;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using System.Threading;

namespace CSharpExt.UnitTests
{
    public class ObservableExt
    {
        [Fact]
        public async Task SelectReplaceAsync() => await new TestScheduler().WithAsync(async scheduler =>
        {
            const int longInitialWait = 10000;
            const int secondWait = 1000;
            List<int> results = new List<int>();
            Subject<int> waitSubject = new Subject<int>();
            int throwCount = 0;
            TaskCompletionSource complete = new TaskCompletionSource();
            waitSubject
                .SelectReplace(async (i, c) =>
                {
                    try
                    {
                        await Observable.Timer(TimeSpan.FromTicks(i), scheduler).ToTask(c).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        throwCount++;
                        return -1;
                    }
                    results.Add(i);
                    return i;
                })
                .Subscribe(
                    onNext: (i) => { },
                    onCompleted: () => complete.Complete());
            waitSubject.OnNext(longInitialWait);
            scheduler.AdvanceBy(longInitialWait / 2);
            waitSubject.OnNext(secondWait);
            waitSubject.OnCompleted();
            throwCount.Should().Equals(1);
            scheduler.AdvanceBy(secondWait - 10);
            results.Should().HaveCount(0);
            scheduler.AdvanceBy(20);
            await complete.Task;
            results.Should().HaveCount(1);
            results.Should().ContainInOrder(secondWait);
        });
    }
}
