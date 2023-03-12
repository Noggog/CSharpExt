using FluentAssertions;
using Noggog;
using Noggog.DotNetCli.DI;
using Noggog.Processes;
using Noggog.Testing.AutoFixture;
using NSubstitute;

namespace CSharpExt.UnitTests.DotNetCli;

public class QueryNugetListingTests
{
    [Theory, DefaultAutoData]
    public async Task CallsConstructListWithProjPath(
        FilePath projPath,
        CancellationToken cancel,
        QueryNugetListing sut)
    {
        sut.ProcessRunner.RunAndCapture(default!, default).ReturnsForAnyArgs(new ProcessResult());
        await sut.Query(projPath, default, default, default, cancel);
        sut.NetCommandStartConstructor.Received(1).Construct("list", projPath, Arg.Any<string[]>());
    }
        
    [Theory, DefaultInlineData(true), DefaultInlineData(false)]
    public async Task ConstructListRespectsOutdated(
        bool outdated,
        FilePath projPath,
        CancellationToken cancel,
        QueryNugetListing sut)
    {
        sut.ProcessRunner.RunAndCapture(default!, default).ReturnsForAnyArgs(new ProcessResult());
        string[]? passedArgs = null;
        sut.NetCommandStartConstructor.Construct(Arg.Any<string>(), Arg.Any<FilePath>(),
            Arg.Do<string[]>(x => passedArgs = x));
        await sut.Query(projPath, default, outdated: outdated, default, cancel);
        if (outdated)
        {
            passedArgs.Should().Contain("--outdated");
        }
        else
        {
            passedArgs.Should().NotContain("--outdated");
        }
    }
        
    [Theory, DefaultInlineData(true), DefaultInlineData(false)]
    public async Task ConstructListRespectsIncludePrerelease(
        bool inclPrerelease,
        FilePath projPath,
        CancellationToken cancel,
        QueryNugetListing sut)
    {
        sut.ProcessRunner.RunAndCapture(default!, default).ReturnsForAnyArgs(new ProcessResult());
        string[]? passedArgs = null;
        sut.NetCommandStartConstructor.Construct(Arg.Any<string>(), Arg.Any<FilePath>(),
            Arg.Do<string[]>(x => passedArgs = x));
        await sut.Query(projPath, default, default, includePrerelease: inclPrerelease, cancel);
        if (inclPrerelease)
        {
            passedArgs.Should().Contain("--include-prerelease");
        }
        else
        {
            passedArgs.Should().NotContain("--include-prerelease");
        }
    }

    [Theory, DefaultAutoData]
    public async Task PassesResultsToProcessor(
        FilePath projPath,
        CancellationToken cancel,
        List<string> processOutput,
        IEnumerable<NugetListingQuery> functionReturn,
        QueryNugetListing sut)
    {
        var processReturn = new ProcessResult(0, processOutput, new List<string>());
        sut.ProcessRunner.RunAndCapture(default!, default).ReturnsForAnyArgs(processReturn);
        sut.ResultProcessor.Process(processReturn.Out).Returns(functionReturn);
        var result = await sut.Query(projPath, default, default, default, cancel);
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Equal(functionReturn);
    }
}