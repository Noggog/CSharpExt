using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using Noggog;
using Shouldly;
using Xunit;

namespace CSharpExt.UnitTests;

public class ShareLatestTests
{
    [Fact]
    public void OverlappingSubscribers_ShareSingleSourceSubscription()
    {
        var subscribeCount = 0;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            return new Subject<int>();
        });

        var shared = source.ShareLatest();

        var sub1 = shared.Subscribe();
        var sub2 = shared.Subscribe();
        var sub3 = shared.Subscribe();

        subscribeCount.ShouldBe(1);

        sub1.Dispose();
        sub2.Dispose();
        sub3.Dispose();
    }

    [Fact]
    public void LateSubscriber_GetsReplayedValue_WhileFirstStillAlive()
    {
        var source = new Subject<int>();
        var shared = source.ShareLatest();

        var first = new List<int>();
        var sub1 = shared.Subscribe(first.Add);

        source.OnNext(42);

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        first.ShouldBe(new[] { 42 });
        second.ShouldBe(new[] { 42 });

        sub1.Dispose();
        sub2.Dispose();
    }

    [Fact]
    public void CachedValue_IsDiscarded_OnRefcountZero()
    {
        var source = new Subject<int>();
        var shared = source.ShareLatest();

        var first = new List<int>();
        var sub1 = shared.Subscribe(first.Add);
        source.OnNext(42);
        sub1.Dispose();

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        first.ShouldBe(new[] { 42 });
        second.ShouldBeEmpty();

        sub2.Dispose();
    }

    [Fact]
    public void CachedError_IsDiscarded_OnRefcountZero()
    {
        var sourceFactoryCount = 0;
        var source = Observable.Defer(() =>
        {
            sourceFactoryCount++;
            return sourceFactoryCount == 1
                ? Observable.Throw<int>(new Exception("boom"))
                : Observable.Return(42);
        });

        var shared = source.ShareLatest();

        Exception? firstError = null;
        var sub1 = shared.Subscribe(_ => { }, ex => firstError = ex);
        sub1.Dispose(); // OnError already auto-disposed; this is belt-and-braces

        var secondValues = new List<int>();
        Exception? secondError = null;
        var sub2 = shared.Subscribe(secondValues.Add, ex => secondError = ex);

        firstError!.Message.ShouldBe("boom");
        secondError.ShouldBeNull();
        secondValues.ShouldBe(new[] { 42 });
        sourceFactoryCount.ShouldBe(2);

        sub2.Dispose();
    }

    [Fact]
    public void Source_IsReSubscribed_OnZeroToOneTransition()
    {
        var subscribeCount = 0;
        Subject<int>? currentInner = null;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            currentInner = new Subject<int>();
            return currentInner;
        });

        var shared = source.ShareLatest();

        var firstValues = new List<int>();
        var sub1 = shared.Subscribe(firstValues.Add);
        currentInner!.OnNext(1);
        sub1.Dispose();

        subscribeCount.ShouldBe(1);

        var secondValues = new List<int>();
        var sub2 = shared.Subscribe(secondValues.Add);

        subscribeCount.ShouldBe(2);

        currentInner!.OnNext(99);
        secondValues.ShouldBe(new[] { 99 });
        firstValues.ShouldBe(new[] { 1 });

        sub2.Dispose();
    }

    [Fact]
    public void ValuesEmittedWhileDisconnected_AreLostButDoNotPoisonNextCycle()
    {
        var source = new Subject<int>();
        var shared = source.ShareLatest();

        var first = new List<int>();
        var sub1 = shared.Subscribe(first.Add);
        source.OnNext(1);
        sub1.Dispose();

        source.OnNext(2);
        source.OnNext(3);

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        second.ShouldBeEmpty();
        source.OnNext(4);
        second.ShouldBe(new[] { 4 });

        sub2.Dispose();
    }

    [Fact]
    public void Completion_IsAlsoResetOnRefcountZero()
    {
        var subscribeCount = 0;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            return Observable.Return(subscribeCount);
        });

        var shared = source.ShareLatest();

        var first = new List<int>();
        var firstCompleted = false;
        shared.Subscribe(first.Add, () => firstCompleted = true);

        var second = new List<int>();
        var secondCompleted = false;
        shared.Subscribe(second.Add, () => secondCompleted = true);

        first.ShouldBe(new[] { 1 });
        second.ShouldBe(new[] { 2 });
        firstCompleted.ShouldBeTrue();
        secondCompleted.ShouldBeTrue();
        subscribeCount.ShouldBe(2);
    }

    // ============== Grace-period overload (ShareLatest(TimeSpan, IScheduler?, int)) ==============

    [Fact]
    public void GracePeriod_RejoinInsideWindow_ReusesWarmSubject()
    {
        var scheduler = new TestScheduler();
        var subscribeCount = 0;
        Subject<int>? currentInner = null;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            currentInner = new Subject<int>();
            return currentInner;
        });

        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        var first = new List<int>();
        var sub1 = shared.Subscribe(first.Add);
        currentInner!.OnNext(42);
        sub1.Dispose();

        scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        second.ShouldBe(new[] { 42 });
        subscribeCount.ShouldBe(1);

        sub2.Dispose();
    }

    [Fact]
    public void GracePeriod_RejoinOutsideWindow_AllocatesFreshSubject()
    {
        var scheduler = new TestScheduler();
        var subscribeCount = 0;
        Subject<int>? currentInner = null;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            currentInner = new Subject<int>();
            return currentInner;
        });

        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        var first = new List<int>();
        var sub1 = shared.Subscribe(first.Add);
        currentInner!.OnNext(42);
        sub1.Dispose();

        scheduler.AdvanceBy(TimeSpan.FromSeconds(11).Ticks);

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        second.ShouldBeEmpty();
        subscribeCount.ShouldBe(2);
        currentInner!.OnNext(99);
        second.ShouldBe(new[] { 99 });

        sub2.Dispose();
    }

    [Fact]
    public void GracePeriod_CachedErrorDoesNotSurviveTeardown()
    {
        var scheduler = new TestScheduler();
        var sourceFactoryCount = 0;
        var source = Observable.Defer(() =>
        {
            sourceFactoryCount++;
            return sourceFactoryCount == 1
                ? Observable.Throw<int>(new Exception("boom"))
                : Observable.Return(42);
        });

        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        Exception? firstError = null;
        shared.Subscribe(_ => { }, ex => firstError = ex);

        scheduler.AdvanceBy(TimeSpan.FromSeconds(11).Ticks);

        var secondValues = new List<int>();
        Exception? secondError = null;
        var sub2 = shared.Subscribe(secondValues.Add, ex => secondError = ex);

        firstError!.Message.ShouldBe("boom");
        secondError.ShouldBeNull();
        secondValues.ShouldBe(new[] { 42 });
        sourceFactoryCount.ShouldBe(2);

        sub2.Dispose();
    }

    [Fact]
    public void GracePeriod_OverlappingSubscribers_ShareSingleSourceSubscription()
    {
        var scheduler = new TestScheduler();
        var subscribeCount = 0;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            return new Subject<int>();
        });

        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        var sub1 = shared.Subscribe();
        var sub2 = shared.Subscribe();
        var sub3 = shared.Subscribe();

        subscribeCount.ShouldBe(1);

        sub1.Dispose();
        sub2.Dispose();
        sub3.Dispose();
    }

    // ============== Multicast fan-out: every live subscriber sees every event ==============

    [Fact]
    public void OverlappingSubscribers_AllReceiveEmittedEvents()
    {
        var source = new Subject<int>();
        var shared = source.ShareLatest();

        var a = new List<int>();
        var b = new List<int>();
        var c = new List<int>();

        var subA = shared.Subscribe(a.Add);
        var subB = shared.Subscribe(b.Add);
        var subC = shared.Subscribe(c.Add);

        source.OnNext(1);
        source.OnNext(2);
        source.OnNext(3);

        a.ShouldBe(new[] { 1, 2, 3 });
        b.ShouldBe(new[] { 1, 2, 3 });
        c.ShouldBe(new[] { 1, 2, 3 });

        subA.Dispose();
        subB.Dispose();
        subC.Dispose();
    }

    [Fact]
    public void GracePeriod_OverlappingSubscribers_AllReceiveEmittedEvents()
    {
        var scheduler = new TestScheduler();
        var source = new Subject<int>();
        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        var a = new List<int>();
        var b = new List<int>();
        var c = new List<int>();

        var subA = shared.Subscribe(a.Add);
        var subB = shared.Subscribe(b.Add);
        var subC = shared.Subscribe(c.Add);

        source.OnNext(1);
        source.OnNext(2);
        source.OnNext(3);

        a.ShouldBe(new[] { 1, 2, 3 });
        b.ShouldBe(new[] { 1, 2, 3 });
        c.ShouldBe(new[] { 1, 2, 3 });

        subA.Dispose();
        subB.Dispose();
        subC.Dispose();
    }

    [Fact]
    public void StaggeredSubscribers_AllReceiveEventsFromTheirJoinPointForward()
    {
        var source = new Subject<int>();
        var shared = source.ShareLatest();

        var a = new List<int>();
        var subA = shared.Subscribe(a.Add);
        source.OnNext(1);

        var b = new List<int>();
        var subB = shared.Subscribe(b.Add); // joins after 1 was cached
        source.OnNext(2);

        var c = new List<int>();
        var subC = shared.Subscribe(c.Add); // joins after 2 was cached
        source.OnNext(3);

        a.ShouldBe(new[] { 1, 2, 3 });
        b.ShouldBe(new[] { 1, 2, 3 }); // 1 was replayed, then live 2, 3
        c.ShouldBe(new[] { 2, 3 });    // 2 was replayed, then live 3

        subA.Dispose();
        subB.Dispose();
        subC.Dispose();
    }

    // ============== Buffer size variants ==============

    [Fact]
    public void BufferSizeGreaterThanOne_LateSubscriberSeesAllBufferedValues()
    {
        var source = new Subject<int>();
        var shared = source.ShareLatest(bufferSize: 3);

        var first = new List<int>();
        var sub1 = shared.Subscribe(first.Add);

        source.OnNext(1);
        source.OnNext(2);
        source.OnNext(3);
        source.OnNext(4);

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        first.ShouldBe(new[] { 1, 2, 3, 4 });
        second.ShouldBe(new[] { 2, 3, 4 });

        sub1.Dispose();
        sub2.Dispose();
    }

    [Fact]
    public void BufferSizeZero_NoReplayToLateSubscribers()
    {
        var source = new Subject<int>();
        var shared = source.ShareLatest(bufferSize: 0);

        var first = new List<int>();
        var sub1 = shared.Subscribe(first.Add);

        source.OnNext(1);

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        first.ShouldBe(new[] { 1 });
        second.ShouldBeEmpty();

        source.OnNext(2);

        first.ShouldBe(new[] { 1, 2 });
        second.ShouldBe(new[] { 2 });

        sub1.Dispose();
        sub2.Dispose();
    }

    [Fact]
    public void GracePeriod_BufferSizeTwo_RejoinInsideWindowSeesBothCached()
    {
        var scheduler = new TestScheduler();
        var source = new Subject<int>();
        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler, bufferSize: 2);

        var sub1 = shared.Subscribe(_ => { });
        source.OnNext(1);
        source.OnNext(2);
        sub1.Dispose();

        scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        second.ShouldBe(new[] { 1, 2 });

        sub2.Dispose();
    }

    [Fact]
    public void GracePeriod_BufferSizeTwo_AfterTeardownNoCachedValues()
    {
        var scheduler = new TestScheduler();
        var source = new Subject<int>();
        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler, bufferSize: 2);

        var sub1 = shared.Subscribe(_ => { });
        source.OnNext(1);
        source.OnNext(2);
        sub1.Dispose();

        scheduler.AdvanceBy(TimeSpan.FromSeconds(11).Ticks);

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        second.ShouldBeEmpty();

        sub2.Dispose();
    }

    // ============== Multi-cycle / refcount behavior ==============

    [Fact]
    public void MultipleReplayCycles_EachAllocatesFreshSubject()
    {
        var subscribeCount = 0;
        Subject<int>? currentInner = null;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            currentInner = new Subject<int>();
            return currentInner;
        });

        var shared = source.ShareLatest();

        for (var cycle = 1; cycle <= 5; cycle++)
        {
            var values = new List<int>();
            var sub = shared.Subscribe(values.Add);
            currentInner!.OnNext(cycle);
            sub.Dispose();
            values.ShouldBe(new[] { cycle });
        }

        subscribeCount.ShouldBe(5);
    }

    [Fact]
    public void GracePeriod_RejoinInsideWindow_DoesNotResubscribeSource()
    {
        var scheduler = new TestScheduler();
        var subscribeCount = 0;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            return new Subject<int>();
        });

        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        var sub1 = shared.Subscribe();
        subscribeCount.ShouldBe(1);
        sub1.Dispose();

        for (var i = 0; i < 3; i++)
        {
            scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);
            var sub = shared.Subscribe();
            subscribeCount.ShouldBe(1);
            sub.Dispose();
        }
    }

    [Fact]
    public void GracePeriod_EventsEmittedDuringWindow_AreCachedForLateRejoin()
    {
        var scheduler = new TestScheduler();
        var source = new Subject<int>();
        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        var first = new List<int>();
        var sub1 = shared.Subscribe(first.Add);
        source.OnNext(1);
        sub1.Dispose();

        // No live subscribers, but during the grace window the subject is still alive and subscribed.
        source.OnNext(2);

        scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

        var second = new List<int>();
        var sub2 = shared.Subscribe(second.Add);

        second.ShouldBe(new[] { 2 });

        sub2.Dispose();
    }

    [Fact]
    public void GracePeriod_AlternatingSubscribersInsideWindow_RemainOnSameWarmSubject()
    {
        var scheduler = new TestScheduler();
        var subscribeCount = 0;
        Subject<int>? inner = null;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            inner = new Subject<int>();
            return inner;
        });

        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        var subA = shared.Subscribe();
        subA.Dispose();
        scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

        var subB = shared.Subscribe(); // cancels pending teardown
        inner!.OnNext(42);
        subB.Dispose();
        scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

        var late = new List<int>();
        var subC = shared.Subscribe(late.Add);

        subscribeCount.ShouldBe(1);
        late.ShouldBe(new[] { 42 });

        subC.Dispose();
    }

    [Fact]
    public void GracePeriod_RapidCycle_OnlyOneSourceSubscriptionUntilWindowExpires()
    {
        var scheduler = new TestScheduler();
        var subscribeCount = 0;
        var disposeCount = 0;
        var source = Observable.Create<int>(_ =>
        {
            subscribeCount++;
            return Disposable.Create(() => disposeCount++);
        });

        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        for (var i = 0; i < 20; i++)
        {
            var sub = shared.Subscribe();
            sub.Dispose();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
        }

        subscribeCount.ShouldBe(1);
        disposeCount.ShouldBe(0);

        scheduler.AdvanceBy(TimeSpan.FromSeconds(11).Ticks);
        disposeCount.ShouldBe(1);
    }

    [Fact]
    public void GracePeriod_ZeroDelay_DelegatesToNoTimespanOverload()
    {
        // TimeSpan.Zero short-circuits to synchronous teardown, so the scheduler must go unused.
        // We deliberately never advance it: if teardown routed through the scheduler, sub2 would
        // reuse the still-warm subject and subscribeCount would stay at 1.
        var scheduler = new TestScheduler();
        var subscribeCount = 0;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            return new Subject<int>();
        });

        var shared = source.ShareLatest(TimeSpan.Zero, scheduler);

        var sub1 = shared.Subscribe();
        sub1.Dispose();

        var sub2 = shared.Subscribe();
        subscribeCount.ShouldBe(2);

        sub2.Dispose();
    }

    // ============== Terminal-state behavior with grace period ==============

    [Fact]
    public void GracePeriod_ErrorThenRejoinOutsideWindow_GetsFreshSourceNoError()
    {
        var scheduler = new TestScheduler();
        var sourceCount = 0;
        var source = Observable.Defer(() =>
        {
            sourceCount++;
            return sourceCount == 1
                ? Observable.Throw<int>(new Exception("boom"))
                : Observable.Return(42);
        });

        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        Exception? firstError = null;
        shared.Subscribe(_ => { }, ex => firstError = ex);

        scheduler.AdvanceBy(TimeSpan.FromSeconds(11).Ticks);

        var values = new List<int>();
        Exception? secondError = null;
        shared.Subscribe(values.Add, ex => secondError = ex);

        firstError!.Message.ShouldBe("boom");
        secondError.ShouldBeNull();
        values.ShouldBe(new[] { 42 });
        sourceCount.ShouldBe(2);
    }

    [Fact]
    public void GracePeriod_CompletionThenRejoinOutsideWindow_GetsFreshSource()
    {
        var scheduler = new TestScheduler();
        var sourceCount = 0;
        var source = Observable.Defer(() =>
        {
            sourceCount++;
            return Observable.Return(sourceCount);
        });

        var shared = source.ShareLatest(TimeSpan.FromSeconds(10), scheduler);

        var first = new List<int>();
        var firstCompleted = false;
        shared.Subscribe(first.Add, () => firstCompleted = true);

        scheduler.AdvanceBy(TimeSpan.FromSeconds(11).Ticks);

        var second = new List<int>();
        var secondCompleted = false;
        shared.Subscribe(second.Add, () => secondCompleted = true);

        first.ShouldBe(new[] { 1 });
        second.ShouldBe(new[] { 2 });
        firstCompleted.ShouldBeTrue();
        secondCompleted.ShouldBeTrue();
        sourceCount.ShouldBe(2);
    }

    // ============== Argument validation ==============

    [Fact]
    public void Constructor_NullSource_Throws()
    {
        IObservable<int>? source = null;
        Should.Throw<ArgumentNullException>(() => source!.ShareLatest());
        Should.Throw<ArgumentNullException>(() => source!.ShareLatest(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public void Constructor_NegativeBufferSize_Throws()
    {
        var source = Observable.Empty<int>();
        Should.Throw<ArgumentOutOfRangeException>(() => source.ShareLatest(bufferSize: -1));
        Should.Throw<ArgumentOutOfRangeException>(() =>
            source.ShareLatest(TimeSpan.FromSeconds(1), bufferSize: -1));
    }

    [Fact]
    public void Constructor_NegativeDisconnectDelay_Throws()
    {
        var source = Observable.Empty<int>();
        Should.Throw<ArgumentOutOfRangeException>(() => source.ShareLatest(TimeSpan.FromMilliseconds(-1)));
    }

    // ============== Why we can't just use Publish().RefCount() at bufferSize: 0 ==============

    [Fact]
    public void PublishRefCount_HasTerminalCacheBug_AtBufferSizeZero()
    {
        // Why ShareLatest(bufferSize: 0) can't just be Publish().RefCount(): stock Publish reuses a
        // single Subject<T> for the connectable's lifetime, so once it sees OnError it stays terminal
        // and every future subscriber gets the cached error.
        var sourceFactoryCount = 0;
        var source = Observable.Defer(() =>
        {
            sourceFactoryCount++;
            return sourceFactoryCount == 1
                ? Observable.Throw<int>(new Exception("boom"))
                : Observable.Return(42);
        });

        var publishRefCount = source.Publish().RefCount();

        Exception? firstError = null;
        publishRefCount.Subscribe(_ => { }, ex => firstError = ex);

        var secondValues = new List<int>();
        Exception? secondError = null;
        publishRefCount.Subscribe(secondValues.Add, ex => secondError = ex);

        firstError!.Message.ShouldBe("boom");
        secondError!.Message.ShouldBe("boom"); // cached terminal replays — the bug this documents
        secondValues.ShouldBeEmpty();
    }

    [Fact]
    public void ShareLatest_AtBufferSizeZero_DoesNotHaveTerminalCacheBug()
    {
        // Success path uses a Subject so the emit happens *after* the late observer subscribes:
        // at bufferSize: 0 there's no cache, so a sync-emit-during-subscribe would just be lost
        // (that's the bufferSize: 0 contract, not the bug under test).
        var sourceFactoryCount = 0;
        Subject<int>? secondCycle = null;
        var source = Observable.Defer(() =>
        {
            sourceFactoryCount++;
            if (sourceFactoryCount == 1)
                return Observable.Throw<int>(new Exception("boom"));
            secondCycle = new Subject<int>();
            return secondCycle;
        });

        var shared = source.ShareLatest(bufferSize: 0);

        Exception? firstError = null;
        shared.Subscribe(_ => { }, ex => firstError = ex);

        var secondValues = new List<int>();
        Exception? secondError = null;
        shared.Subscribe(secondValues.Add, ex => secondError = ex);

        secondCycle!.OnNext(42);

        firstError!.Message.ShouldBe("boom");
        secondError.ShouldBeNull();
        secondValues.ShouldBe(new[] { 42 });
        sourceFactoryCount.ShouldBe(2);
    }

    // ============== Hot terminal signals (refcount > 0 when source terminates) ==============

    [Fact]
    public void HotCompletion_AllLiveSubscribersReceiveCompleted()
    {
        var source = new Subject<int>();
        var shared = source.ShareLatest();

        var a = new List<int>();
        var aCompleted = false;
        var b = new List<int>();
        var bCompleted = false;

        shared.Subscribe(a.Add, () => aCompleted = true);
        shared.Subscribe(b.Add, () => bCompleted = true);

        source.OnNext(1);
        source.OnCompleted();

        a.ShouldBe(new[] { 1 });
        b.ShouldBe(new[] { 1 });
        aCompleted.ShouldBeTrue();
        bCompleted.ShouldBeTrue();
    }

    [Fact]
    public void HotError_AllLiveSubscribersReceiveError()
    {
        var source = new Subject<int>();
        var shared = source.ShareLatest();

        Exception? aError = null;
        Exception? bError = null;

        shared.Subscribe(_ => { }, ex => aError = ex);
        shared.Subscribe(_ => { }, ex => bError = ex);

        source.OnError(new Exception("boom"));

        aError!.Message.ShouldBe("boom");
        bError!.Message.ShouldBe("boom");
    }

    // ============== Re-entrancy / self-disposal ==============

    [Fact]
    public void SelfDisposingSubscriber_InOnNextCallback_DoesNotCorruptState()
    {
        // Sole subscriber disposes itself synchronously inside its own OnNext — teardown runs
        // while still inside the source's emit. Must not corrupt state.
        var subscribeCount = 0;
        Subject<int>? current = null;
        var source = Observable.Defer(() =>
        {
            subscribeCount++;
            current = new Subject<int>();
            return current;
        });

        var shared = source.ShareLatest();

        IDisposable? selfSub = null;
        selfSub = shared.Subscribe(_ => selfSub!.Dispose());

        current!.OnNext(1);

        var values = new List<int>();
        var sub = shared.Subscribe(values.Add);
        current!.OnNext(99);
        values.ShouldBe(new[] { 99 });
        subscribeCount.ShouldBe(2);

        sub.Dispose();
    }

    // ============== Refcount integrity under rapid / concurrent churn ==============

    [Fact]
    public void ManyRapidCycles_NoSubscriptionLeak()
    {
        var subscribeCount = 0;
        var disposeCount = 0;
        var source = Observable.Create<int>(_ =>
        {
            subscribeCount++;
            return Disposable.Create(() => disposeCount++);
        });

        var shared = source.ShareLatest();

        for (var i = 0; i < 1000; i++)
        {
            var sub = shared.Subscribe();
            sub.Dispose();
        }

        subscribeCount.ShouldBe(1000);
        disposeCount.ShouldBe(1000);
    }

    [Fact]
    public async Task ConcurrentSubscribeAndDispose_NoLeakOrDeadlock()
    {
        var subscribeCount = 0;
        var disposeCount = 0;
        var source = Observable.Create<int>(_ =>
        {
            Interlocked.Increment(ref subscribeCount);
            return Disposable.Create(() => Interlocked.Increment(ref disposeCount));
        });

        var shared = source.ShareLatest();

        var tasks = Enumerable.Range(0, 50).Select(_ => Task.Run(() =>
        {
            for (var i = 0; i < 200; i++)
            {
                var sub = shared.Subscribe();
                sub.Dispose();
            }
        })).ToArray();

        await Task.WhenAll(tasks);

        subscribeCount.ShouldBeGreaterThan(0);
        subscribeCount.ShouldBe(disposeCount);
    }

    [Fact]
    public async Task GracePeriod_ConcurrentSubscribeAndDispose_NoLeakOrDeadlock()
    {
        var subscribeCount = 0;
        var disposeCount = 0;
        var source = Observable.Create<int>(_ =>
        {
            Interlocked.Increment(ref subscribeCount);
            return Disposable.Create(() => Interlocked.Increment(ref disposeCount));
        });

        // Real Scheduler.Default — TestScheduler is single-threaded and wouldn't actually
        // exercise the lock under concurrent pressure. Short grace so the test settles fast.
        var shared = source.ShareLatest(TimeSpan.FromMilliseconds(5));

        var tasks = Enumerable.Range(0, 50).Select(_ => Task.Run(() =>
        {
            for (var i = 0; i < 200; i++)
            {
                var sub = shared.Subscribe();
                sub.Dispose();
            }
        })).ToArray();

        await Task.WhenAll(tasks);

        // Wait out every pending grace-window teardown.
        await Task.Delay(200);

        subscribeCount.ShouldBeGreaterThan(0);
        subscribeCount.ShouldBe(disposeCount);
    }
}
