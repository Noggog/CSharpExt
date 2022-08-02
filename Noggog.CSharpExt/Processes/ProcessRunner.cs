using System.Diagnostics;

namespace Noggog.Processes;

public interface IProcessRunner
{
    Task<ProcessRunner.Return> RunAndCapture(
        ProcessStartInfo startInfo,
        CancellationToken? cancel = null);
        
    Task<int> Run(
        ProcessStartInfo startInfo,
        CancellationToken? cancel = null);

    Task<int> RunWithCallback(
        ProcessStartInfo startInfo,
        Action<string> outputCallback,
        Action<string> errorCallback,
        CancellationToken? cancel = null);

    Task<int> RunWithCallback(
        ProcessStartInfo startInfo,
        Action<string> callback,
        CancellationToken? cancel = null);
}

public class ProcessRunner : IProcessRunner
{
    public IProcessFactory Factory { get; }

    public ProcessRunner(IProcessFactory processFactory)
    {
        Factory = processFactory;
    }
    
    public record Return(int Result, List<string> Out, List<string> Errors)
    {
        public Return()
            : this(-1, new(), new())
        {
        }
    }
        
    public async Task<Return> RunAndCapture(
        ProcessStartInfo startInfo,
        CancellationToken? cancel = null)
    {
        var outs = new List<string>();
        var errs = new List<string>();
        var ret = await RunWithCallback(
            startInfo,
            i => outs.Add(i),
            i => errs.Add(i),
            cancel).ConfigureAwait(false);
        return new(ret, outs, errs);
    }

    public async Task<int> Run(
        ProcessStartInfo startInfo,
        CancellationToken? cancel = null)
    {
        using var proc = Factory.Create(
            startInfo,
            cancel: cancel);
        return await proc.Run().ConfigureAwait(false);
    }

    public async Task<int> RunWithCallback(
        ProcessStartInfo startInfo,
        Action<string> outputCallback, 
        Action<string> errorCallback,
        CancellationToken? cancel = null)
    {
        using var proc = Factory.Create(
            startInfo,
            cancel: cancel);
        using var outSub = proc.Output.Subscribe(outputCallback);
        List<string> errs = new();
        using var errSub = proc.Error.Subscribe(errs.Add);
        try
        {
            return await proc.Run().ConfigureAwait(false);
        }
        finally
        {
            foreach (var err in errs)
            {
                errorCallback(err);
            }
        }
    }

    public async Task<int> RunWithCallback(
        ProcessStartInfo startInfo,
        Action<string> callback,
        CancellationToken? cancel = null)
    {
        return await RunWithCallback(
            startInfo,
            callback,
            callback,
            cancel).ConfigureAwait(false);;
    }
}