using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.IO;

public class FilePathJsonConverterTests
{
    class Dto
    {
        public FilePath MyFile { get; set; }
    }
    
    [Fact]
    public void NakedSerialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "NakedFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyFile.RelativePath.Should().Be("C:\\SomeDir\\SomeFile.txt");
    }
}