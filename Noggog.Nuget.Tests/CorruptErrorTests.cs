using System.IO.Abstractions.TestingHelpers;
using System.Xml.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Noggog.Nuget.Errors;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace Noggog.Nuget.Tests;

public class CorruptErrorTests
{
    [Theory, DefaultAutoData]
    public void CorruptFix(
        FilePath path,
        [Frozen]MockFileSystem fs,
        CorruptError sut)
    {
        fs.File.WriteAllText(path, "Whut");
        sut.RunFix(path);
        var doc = XDocument.Load(fs.FileStream.New(path, FileMode.Open, FileAccess.Read));
        doc.Should().BeEquivalentTo(NotExistsError.TypicalFile());
    }
}