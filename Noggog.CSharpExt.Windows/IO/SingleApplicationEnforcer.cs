using System.IO.MemoryMappedFiles;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Nito.AsyncEx.Interop;

namespace Noggog.IO;

public interface IWatchSingleAppArguments
{
    IObservable<IReadOnlyList<string>> WatchMemoryMappedFile();
}

/// <summary>
/// This class allows restricting the number of executables in execution to one.
/// </summary>
public class SingletonApplicationEnforcer : IDisposable
{
    private readonly string _applicationId;
    private readonly Lazy<WaitHandle> _handle;
    
    public bool IsFirstApplication => _handle.Value.IsFirstApplication;

    private string ArgsWaitHandleName => $"ArgsWaitHandle_{_applicationId}";
    private string MemoryFileName => $"ArgFile_{_applicationId}";

    /// <summary>
    /// Gets or sets the string that is used to join 
    /// the string array of arguments in memory.
    /// </summary>
    /// <value>The arg delimiter.</value>
    public string ArgDelimiter { get; set; } = "_;;_";

    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonApplicationEnforcer"/> class.
    /// </summary>
    /// <param name="applicationId">The application id used 
    /// for naming the <seealso cref="EventWaitHandle"/>.</param>
    public SingletonApplicationEnforcer(string applicationId)
    {
        _applicationId = applicationId;
        _handle = new Lazy<WaitHandle>(() => new WaitHandle(applicationId));
    }

    public void ForwardArgs(IReadOnlyList<string> args)
    {
        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(MemoryFileName))
        {
            using MemoryMappedViewStream stream = mmf.CreateViewStream();
            
            var writer = new BinaryWriter(stream);
            string joined = string.Join(ArgDelimiter, args);
            writer.Write(joined);
        }
        _handle.Value.Set();
    }

    public IObservable<IReadOnlyList<string>> WatchArgs()
    {
        return Observable.Create<IReadOnlyList<string>>(async obs =>
        {
            using MemoryMappedFile file = MemoryMappedFile.CreateOrOpen(MemoryFileName, 10000);
                        
            while (true)
            {
                await _handle.Value.WaitOneAsync();
                using (MemoryMappedViewStream stream = file.CreateViewStream())
                {
                    var reader = new BinaryReader(stream);
                    string args = reader.ReadString();
                    string[] argsSplit = args.Split(new string[] { ArgDelimiter }, 
                        StringSplitOptions.RemoveEmptyEntries);
                    obs.OnNext(argsSplit);
                }
            }
            
            return Disposable.Empty;
        });
    }

    public void Dispose()
    {
        if (!_handle.IsValueCreated) return;
        _handle.Value.Dispose();
    }
    
    private class WaitHandle : IDisposable
    {
        private readonly EventWaitHandle _argsWaitHandle;
        
        public bool IsFirstApplication { get; }

        public WaitHandle(string name)
        {
            _argsWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, name, out var isFirstApplication);
            IsFirstApplication = isFirstApplication;
        }

        public Task WaitOneAsync() => WaitHandleAsyncFactory.FromWaitHandle(_argsWaitHandle);

        public bool WaitOne() => _argsWaitHandle.WaitOne();

        public bool Set() => _argsWaitHandle.Set();

        public void Dispose()
        {
            _argsWaitHandle.Dispose();
        }
    }
}