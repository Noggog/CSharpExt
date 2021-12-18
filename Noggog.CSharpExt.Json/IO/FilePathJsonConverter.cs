using Newtonsoft.Json;

namespace Noggog.Json.IO;

public class FilePathJsonConverter : JsonConverter
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(FilePath);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not FilePath file) throw new ArgumentException("FilePath was not given");
        writer.WriteValue(file.RelativePath);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.ValueType == typeof(string))
        {
            return new FilePath((string)reader.Value!);
        }

        return JsonSerializer.Create().Deserialize<FilePath>(reader);
    }
}