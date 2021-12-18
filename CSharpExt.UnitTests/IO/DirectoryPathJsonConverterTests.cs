using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.IO;

public class DirectoryPathJsonConverterTests
{
    class Dto
    {
        public DirectoryPath MyDirectory { get; set; }
    }
    
    [Fact]
    public void NakedSerialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "NakedDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyDirectory.RelativePath.Should().Be("C:\\SomeDir\\SomePath");
    }
}