using System.IO.Abstractions.TestingHelpers;
using System.Xml.Linq;
using AutoFixture.Xunit2;
using Noggog.Nuget.Errors;
using Noggog.Testing.AutoFixture;
using Shouldly;
using Xunit;

namespace Noggog.Nuget.Tests;

public class NotExistsErrorTests
{
    [Theory, DefaultAutoData]
    public void MissingFix(
        FilePath path,
        [Frozen]MockFileSystem fs,
        NotExistsError sut)
    {
        sut.RunFix(path);
        var doc = XDocument.Load(fs.FileStream.New(path, FileMode.Open, FileAccess.Read));
        doc.ShouldBeEquivalentTo(NotExistsError.TypicalFile());
    }
        
    [Theory, DefaultAutoData]
    public void EmptyFileFix(
        FilePath path,
        [Frozen]MockFileSystem fs,
        CorruptError sut)
    {
        fs.File.WriteAllText(path, "");
        sut.RunFix(path);
        var doc = XDocument.Load(fs.FileStream.New(path, FileMode.Open, FileAccess.Read));
        doc.ShouldBeEquivalentTo(NotExistsError.TypicalFile());
    }

    [Theory, DefaultAutoData]
    public void NoConfigurationFix(
        FilePath path,
        [Frozen]MockFileSystem fs,
        CorruptError sut)
    {
        fs.File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<something />");
        sut.RunFix(path);
        var doc = XDocument.Load(fs.FileStream.New(path, FileMode.Open, FileAccess.Read));
        doc.ShouldBeEquivalentTo(NotExistsError.TypicalFile());
    }
}