using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Noggog;
using Noggog.Json.IO;
using Xunit;

namespace CSharpExt.UnitTests.IO;

public class FilePathJsonConverterTests
{
    private JsonSerializerSettings ConverterSettings = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter>()
        {
            new FilePathJsonConverter()
        }
    };
    
    class Dto
    {
        public FilePath MyFile { get; set; }
    }
    
    [Fact]
    public void OldDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "OldFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyFile.RelativePath.Should().Be("C:\\SomeDir\\SomeFile.txt");
    }

    [Fact]
    public void NakedDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "NakedFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyFile.RelativePath.Should().Be("C:\\SomeDir\\SomeFile.txt");
    }

    [Fact]
    public void NakedSerialization()
    {
        var dto = new Dto()
        {
            MyFile = new FilePath("C:\\SomeDir\\SomeFile.txt")
        };
        var str = JsonConvert.SerializeObject(dto, Formatting.Indented);
        str.Should().Be(
            File.ReadAllText(Path.Combine("IO", "Files", "NakedFilePathSerialization.json")));
    }

    [Fact]
    public void ConverterDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "ConverterFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyFile.RelativePath.Should().Be("C:\\SomeDir\\SomeFile.txt");
    }

    [Fact]
    public void OldToConverterDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "OldFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyFile.RelativePath.Should().Be("C:\\SomeDir\\SomeFile.txt");
    }

    [Fact]
    public void NakedToConverterDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "NakedFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyFile.RelativePath.Should().Be("C:\\SomeDir\\SomeFile.txt");
    }

    [Fact]
    public void ConverterSerialization()
    {
        var dto = new Dto()
        {
            MyFile = new FilePath("C:\\SomeDir\\SomeFile.txt")
        };
        var str = File.ReadAllText(Path.Combine("IO", "Files", "ConverterFilePathSerialization.json"));
        var converted = JsonConvert.SerializeObject(dto, ConverterSettings);
        str.Should().Be(converted);
    }
}