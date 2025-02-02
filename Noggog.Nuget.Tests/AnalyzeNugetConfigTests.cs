using System.IO.Abstractions.TestingHelpers;
using AutoFixture.Xunit2;
using Noggog.Nuget.Errors;
using Noggog.Nuget.Services.Singleton;
using Noggog.Testing.AutoFixture;
using Shouldly;
using Xunit;

namespace Noggog.Nuget.Tests;

public class AnalyzeNugetConfigTests
{
    [Theory, DefaultAutoData]
    public void CorruptTrigger(
        FilePath path,
        [Frozen]MockFileSystem fs,
        AnalyzeNugetConfig sut)
    {
        fs.File.WriteAllText(path, "Whut");
        sut.Analyze(path)
            .ShouldBeOfType<CorruptError>();
    }
        
    [Theory, DefaultAutoData]
    public void MissingTrigger(
        FilePath path,
        AnalyzeNugetConfig sut)
    {
        sut.Analyze(path)
            .ShouldBeOfType<NotExistsError>();
    }
        
    [Theory, DefaultAutoData]
    public void EmptyFileTrigger(
        FilePath path,
        [Frozen]MockFileSystem fs,
        AnalyzeNugetConfig sut)
    {
        fs.File.WriteAllText(path, "");
        sut.Analyze(path)
            .ShouldBeOfType<NotExistsError>();
    }
        
    [Theory, DefaultAutoData]
    public void NoConfigurationTrigger(
        FilePath path,
        [Frozen]MockFileSystem fs,
        AnalyzeNugetConfig sut)
    {
        fs.File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<something />");
        sut.Analyze(path)
            .ShouldBeOfType<NotExistsError>();
    }
        
    [Theory, DefaultAutoData]
    public void EmptyPackageSourcesTrigger(
        FilePath path,
        [Frozen]MockFileSystem fs,
        AnalyzeNugetConfig sut)
    {
        fs.File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                   "<configuration>" +
                                   "<packageSources>" +
                                   "</packageSources>" +
                                   "</configuration>");
        sut.Analyze(path)
            .ShouldBeOfType<MissingNugetOrgError>();
    }
        
    [Theory, DefaultAutoData]
    public void OtherPackageSourcesTrigger(
        FilePath path,
        [Frozen]MockFileSystem fs,
        AnalyzeNugetConfig sut)
    {
        fs.File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                   "<configuration>" +
                                   "<packageSources>" +
                                   "<add key=\"CSharp Dev\" value=\"C:\\Repos\\CSharpExt\\Noggog.CSharpExt\\bin\\Debug\" />" +
                                   "</packageSources>" +
                                   "</configuration>");
        sut.Analyze(path)
            .ShouldBeOfType<MissingNugetOrgError>();
    }
        
    [Theory, DefaultAutoData]
    public void MissingPackageSourcesTrigger(
        FilePath path,
        [Frozen]MockFileSystem fs,
        AnalyzeNugetConfig sut)
    {
        fs.File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                   "<configuration>" +
                                   "</configuration>");
        sut.Analyze(path)
            .ShouldBeOfType<MissingNugetOrgError>();
    }

    [Theory, DefaultAutoData]
    public void LocatedNugetEntryReturnsNull(
        FilePath path,
        [Frozen]MockFileSystem fs,
        AnalyzeNugetConfig sut)
    {
        fs.File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                   "<configuration>" +
                                   "<packageSources>" +
                                   "<add key=\"nuget.org\" value=\"https://api.nuget.org/v3/index.json\" />" +
                                   "</packageSources>" +
                                   "</configuration>");
        sut.Analyze(path)
            .ShouldBeNull();
    }
}