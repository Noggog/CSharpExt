#if NETSTANDARD2_0 
#else
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Noggog.Utility;

namespace Noggog.WorkEngine;

public interface IWorkConsumer
{
    int CpuId { get; }
    IObservable<(int CurrentCPUs, int DesiredCPUs)> CurrentCpuCount { get; }
    IObservable<Exception> Errors { get; }
    void Start();
}

public class WorkConsumer : IDisposable, IWorkConsumer
{
    private readonly CompositeDisposable _disposable = new();
    private const int UnassignedCpuId = 0;
    private readonly INumWorkThreadsController _numWorkThreadsController;
    private readonly IWorkQueue _queue;
    private readonly AsyncLock _lock = new();
    private int _desiredNumThreads;
    private readonly Dictionary<int, Task> _tasks = new();
    private int _nextCpuID = 1; // Start at 1, as 0 is "Unassigned"
    private static readonly AsyncLocal<int> _cpuId = new();
    internal static readonly AsyncLocal<IWorkQueue?> AsyncLocalCurrentQueue = new();
    public static bool WorkerThread => _cpuId.Value != 0;
    public int CpuId => _cpuId.Value;
    private readonly CancellationTokenSource _shutdown = new();

    private readonly BehaviorSubject<(int DesiredCPUs, int CurrentCPUs)> _cpuCountSubj = new((0, 0));
    public IObservable<(int CurrentCPUs, int DesiredCPUs)> CurrentCpuCount => _cpuCountSubj;

    private readonly Subject<Exception> _errors = new();
    public IObservable<Exception> Errors => _errors;

    public WorkConsumer(
        INumWorkThreadsController numWorkThreadsController,
        IWorkQueue queue)
    {
        _numWorkThreadsController = numWorkThreadsController;
        _queue = queue;
    }

    public WorkConsumer(
        int numThreads,
        IWorkQueue queue)
    {
        _numWorkThreadsController = new NumWorkThreadsConstant(numThreads);
        _queue = queue;
    }

    private async Task AddNewThreadsIfNeeded(int desired)
    {
        using (await _lock.WaitAsync())
        {
            _desiredNumThreads = desired;
            while (_desiredNumThreads > _tasks.Count)
            {
                var cpuID = _nextCpuID++;
                _tasks[cpuID] = Task.Run(() => ThreadBody(cpuID));
            }
            _cpuCountSubj.OnNext((_tasks.Count, _desiredNumThreads));
        }
    }

    private async Task ThreadBody(int cpuID)
    {
        _cpuId.Value = cpuID;
        AsyncLocalCurrentQueue.Value = _queue;

        try
        {
            while (true)
            {
                if (_shutdown.IsCancellationRequested) return;

                IToDo? toDo;
                try
                {
                    _queue.Reader.TryRead(out toDo);
                }
                catch (Exception)
                {
                    throw new OperationCanceledException();
                }

                if (toDo != null)
                {
                    if (toDo.IsAsync)
                    {
                        await toDo.DoAsync();
                    }
                    else
                    {
                        toDo.Do();
                    }
                }
                else
                {
                    if (!await _queue.Reader.WaitToReadAsync())
                    {
                        return;
                    }
                }

                // Check if we're currently trimming threads
                if (_desiredNumThreads >= _tasks.Count) continue;

                // Noticed that we may need to shut down, lock and check again
                using (await _lock.WaitAsync())
                {
                    // Check if another thread shut down before this one and got us back to the desired amount already
                    if (_desiredNumThreads >= _tasks.Count) continue;

                    // Shutdown
                    _tasks.Remove(cpuID);
                    _cpuCountSubj.OnNext((_tasks.Count, _desiredNumThreads));
                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _errors.OnNext(ex);
        }
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }

    public void Start()
    {
        _numWorkThreadsController.NumDesiredThreads
            .Select(x => x ?? 0)
            .Select(x => x == 0 ? Environment.ProcessorCount : x)
            .DistinctUntilChanged()
            .Subscribe(AddNewThreadsIfNeeded)
            .DisposeWithComposite(_disposable);
    }
}
#endif