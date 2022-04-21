using System.Runtime.InteropServices;
using FluentAssertions;
using Newtonsoft.Json;
using Noggog;
using Noggog.Json.IO;
using Noggog.Testing.IO;
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
        var str = GetTestFile(Path.Combine("IO", "Files", "OldDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyDirectory.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomePath"));
    }

    [Fact]
    public void NakedDeserialization()
    {
        var str = GetTestFile(Path.Combine("IO", "Files", "NakedDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyDirectory.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomePath"));
    }

    [Fact]
    public void NakedSerialization()
    {
        var dto = new Dto()
        {
            MyDirectory = new DirectoryPath(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomePath"))
        };
        var str = JsonConvert.SerializeObject(dto, Formatting.Indented);
        str.Should().Be(
            GetTestFile(Path.Combine("IO", "Files", "NakedDirectoryPathSerialization.json")));
    }

    [Fact]
    public void ConverterDeserialization()
    {
        var str = GetTestFile(Path.Combine("IO", "Files", "ConverterDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyDirectory.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomePath"));
    }

    [Fact]
    public void OldToConverterDeserialization()
    {
        var str = GetTestFile(Path.Combine("IO", "Files", "OldDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyDirectory.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomePath"));
    }

    [Fact]
    public void NakedToConverterDeserialization()
    {
        var str = GetTestFile(Path.Combine("IO", "Files", "NakedDirectoryPathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyDirectory.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomePath"));
    }

    [Fact]
    public void ConverterSerialization()
    {
        var dto = new Dto()
        {
            MyDirectory = new DirectoryPath(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomePath"))
        };
        var str = GetTestFile(Path.Combine("IO", "Files", "ConverterDirectoryPathSerialization.json"));
        var converted = JsonConvert.SerializeObject(dto, ConverterSettings);
        str.Should().Be(converted);
    }

    private string GetTestFile(string path)
    {
        var ret = File.ReadAllText(path);
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ret = ret.Replace("C:", string.Empty);
            ret = ret.Replace("\\\\", $"{Path.DirectorySeparatorChar}");
            ret = IFileSystemExt.CleanDirectorySeparators(ret);
        }
        return ret;
    }
}