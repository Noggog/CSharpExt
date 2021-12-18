using Newtonsoft.Json;

namespace Noggog.Json.IO;

public class DirectoryPathJsonConverter : JsonConverter
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(DirectoryPath);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not DirectoryPath dir) throw new ArgumentException("DirectoryPath was not given");
        writer.WriteValue(dir.RelativePath);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        if (existingValue == null) return null;
        return new DirectoryPath(existingValue.ToString());
    }
}