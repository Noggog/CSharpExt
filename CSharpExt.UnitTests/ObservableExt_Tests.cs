using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Xunit;
using Noggog;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Noggog.Utility;
using System.IO;
using DynamicData;

namespace CSharpExt.UnitTests
{
    public class ObservableExt_Tests
    {
        [Fact]
        public async Task SelectReplaceAsync() => await new TestScheduler().WithAsync(async scheduler =>
        {
            const int longInitialWait = 10000;
            const int secondWait = 1000;
            List<int> results = new List<int>();
            Subject<int> waitSubject = new Subject<int>();
            int throwCount = 0;
#if NET_5
            TaskCompletionSource complete = new TaskCompletionSource();
#else
            Noggog.TaskCompletionSource complete = new Noggog.TaskCompletionSource();
#endif
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
                    onCompleted: () => complete.SetResult());
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

        [Fact]
        public async Task WatchFolder()
        {
            using var temp = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(WatchFolder)));
            var fileA = Path.Combine(temp.Dir.Path, "FileA");
            var fileB = Path.Combine(temp.Dir.Path, "FileB");
            File.WriteAllText(fileA, string.Empty);
            var live = ObservableExt.WatchFolderContents(temp.Dir.Path)
                .RemoveKey();
            await Task.Delay(2000);
            var list = live.AsObservableList();
            list.Count.Should().Be(1);
            list.Items.ToExtendedList()[0].Should().Be(fileA);
            File.WriteAllText(fileB, string.Empty);
            await Task.Delay(2000);
            list = live.AsObservableList();
            list.Count.Should().Be(2);
            list.Items.ToExtendedList()[0].Should().Be(fileA);
            list.Items.ToExtendedList()[1].Should().Be(fileB);
            File.Delete(fileA);
            await Task.Delay(2000);
            list = live.AsObservableList();
            list.Count.Should().Be(1);
            list.Items.ToExtendedList()[0].Should().Be(fileB);
        }
    }
}
