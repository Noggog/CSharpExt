using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Noggog;
using Noggog.Json.IO;
using Xunit;

namespace CSharpExt.UnitTests.IO;

public class DirectoryPathJsonConverterTests
{
    private JsonSerializerSettings ConverterSettings = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter>()
        {
            new DirectoryPathJsonConverter()
        }
    };
    
    class Dto
    {
        public DirectoryPath MyDirectory { get; set; }
    }

    [Fact]
    public void OldDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "OldDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyDirectory.RelativePath.Should().Be("C:\\SomeDir\\SomePath");
    }

    [Fact]
    public void NakedDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "NakedDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyDirectory.RelativePath.Should().Be("C:\\SomeDir\\SomePath");
    }

    [Fact]
    public void NakedSerialization()
    {
        var dto = new Dto()
        {
            MyDirectory = new DirectoryPath("C:\\SomeDir\\SomePath")
        };
        var str = JsonConvert.SerializeObject(dto, Formatting.Indented);
        str.Should().Be(
            File.ReadAllText(Path.Combine("IO", "Files", "NakedDirectoryPathSerialization.json")));
    }

    [Fact]
    public void ConverterDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "ConverterDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyDirectory.RelativePath.Should().Be("C:\\SomeDir\\SomePath");
    }

    [Fact]
    public void OldToConverterDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "OldDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyDirectory.RelativePath.Should().Be("C:\\SomeDir\\SomePath");
    }

    [Fact]
    public void NakedToConverterDeserialization()
    {
        var str = File.ReadAllText(Path.Combine("IO", "Files", "NakedDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyDirectory.RelativePath.Should().Be("C:\\SomeDir\\SomePath");
    }

    [Fact]
    public void ConverterSerialization()
    {
        var dto = new Dto()
        {
            MyDirectory = new DirectoryPath("C:\\SomeDir\\SomePath")
        };
        var str = File.ReadAllText(Path.Combine("IO", "Files", "ConverterDirectoryPathSerialization.json"));
        var converted = JsonConvert.SerializeObject(dto, ConverterSettings);
        str.Should().Be(converted);
    }
}