using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FluentAssertions;
using Newtonsoft.Json;
using Noggog;
using Noggog.Json.IO;
using Noggog.Testing.IO;
using Noggog.Utility;
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
        var str = GetTestFile(Path.Combine("IO", "Files", "OldFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyFile.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomeFile.txt"));
    }

    [Fact]
    public void NakedDeserialization()
    {
        var str = GetTestFile(Path.Combine("IO", "Files", "NakedFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str)!;
        converted.MyFile.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomeFile.txt"));
    }

    [Fact]
    public void NakedSerialization()
    {
        var dto = new Dto()
        {
            MyFile = new FilePath(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomeFile.txt"))
        };
        var str = JsonConvert.SerializeObject(dto, Formatting.Indented);
        str.Should().Be(
            GetTestFile(Path.Combine("IO", "Files", "NakedFilePathSerialization.json")));
    }

    [Fact]
    public void ConverterDeserialization()
    {
        var str = GetTestFile(Path.Combine("IO", "Files", "ConverterFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyFile.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomeFile.txt"));
    }

    [Fact]
    public void OldToConverterDeserialization()
    {
        var str = GetTestFile(Path.Combine("IO", "Files", "OldFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyFile.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomeFile.txt"));
    }

    [Fact]
    public void NakedToConverterDeserialization()
    {
        var str = GetTestFile(Path.Combine("IO", "Files", "NakedFilePathSerialization.json"));
        var converted = JsonConvert.DeserializeObject<Dto>(str, ConverterSettings)!;
        converted.MyFile.RelativePath.Should().Be(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomeFile.txt"));
    }

    [Fact]
    public void ConverterSerialization()
    {
        var dto = new Dto()
        {
            MyFile = new FilePath(Path.Combine($"{PathingUtil.DrivePrefix}SomeDir", "SomeFile.txt"))
        };
        var str = GetTestFile(Path.Combine("IO", "Files", "ConverterFilePathSerialization.json"));
        var converted = JsonConvert.SerializeObject(dto, ConverterSettings);
        str.Should().Be(converted);
    }

    private string GetTestFile(string path)
    {
        var ret = File.ReadAllText(path);
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ret = ret.Replace("C:", string.Empty);
            ret = ret.Replace("\\\\", $"{System.IO.Path.DirectorySeparatorChar}");
            ret = IFileSystemExt.CleanDirectorySeparators(ret);
        }
        return ret;
    }
}