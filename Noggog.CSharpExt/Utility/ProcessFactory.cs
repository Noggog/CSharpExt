using System.Diagnostics;

namespace Noggog.Utility;

public interface IProcessFactory
{
    IProcessWrapper Create(
        ProcessStartInfo startInfo,
        CancellationToken? cancel = null,
        bool hideWindow = true,
        bool hookOntoOutput = true,
        bool killWithParent = true);
}

public class ProcessFactory : IProcessFactory
{
    public IProcessWrapper Create(
        ProcessStartInfo startInfo,
        CancellationToken? cancel = null,
        bool hideWindow = true,
        bool hookOntoOutput = true,
        bool killWithParent = true)
    {
        return ProcessWrapper.Create(
            startInfo,
            cancel,
            hideWindow: hideWindow,
            hookOntoOutput: hookOntoOutput,
            killWithParent: killWithParent);
    }
}