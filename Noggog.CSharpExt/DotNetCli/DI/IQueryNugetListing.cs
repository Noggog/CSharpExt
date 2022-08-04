using Noggog.Processes;
using Noggog.Processes.DI;

namespace Noggog.DotNetCli.DI;

public interface IQueryNugetListing
{
    Task<GetResponse<IEnumerable<NugetListingQuery>>> Query(
        FilePath projectPath,
        bool runRestore = true,
        bool outdated = false,
        bool includePrerelease = false,
        CancellationToken cancel = default);
}

public class QueryNugetListing : IQueryNugetListing
{
    public IProcessNugetQueryResults ResultProcessor { get; }
    public IProcessFactory ProcessFactory { get; }
    public IProcessRunner ProcessRunner { get; }
    public IDotNetCommandStartConstructor NetCommandStartConstructor { get; }

    public QueryNugetListing(
        IProcessFactory processFactory,
        IProcessRunner processRunner,
        IProcessNugetQueryResults resultProcessor,
        IDotNetCommandStartConstructor dotNetCommandStartConstructor)
    {
        ResultProcessor = resultProcessor;
        ProcessFactory = processFactory;
        ProcessRunner = processRunner;
        NetCommandStartConstructor = dotNetCommandStartConstructor;
    }
        
    public async Task<GetResponse<IEnumerable<NugetListingQuery>>> Query(
        FilePath projectPath, 
        bool runRestore = true,
        bool outdated = false,
        bool includePrerelease = false,
        CancellationToken cancel = default)
    {
        ProcessResult result;
        if (runRestore)
        {
            result = await ProcessRunner.RunAndCapture(
                NetCommandStartConstructor.Construct("restore", projectPath),
                cancel: cancel).ConfigureAwait(false);
            if (result.FailedReturnOrErrorMessages)
            {
                return result.AsErrorResponse().BubbleFailure<IEnumerable<NugetListingQuery>>();
            }
        }
        
        result = await ProcessRunner.RunAndCapture(
            NetCommandStartConstructor.Construct("list",
                projectPath, 
                "package",
                outdated ? "--outdated" : null,
                includePrerelease ? "--include-prerelease" : null),
            cancel: cancel).ConfigureAwait(false);
        
        if (result.FailedReturnOrErrorMessages)
        {
            return result.AsErrorResponse().BubbleFailure<IEnumerable<NugetListingQuery>>();
        }

        return GetResponse<IEnumerable<NugetListingQuery>>.Succeed(ResultProcessor.Process(result.Out));
    }
}