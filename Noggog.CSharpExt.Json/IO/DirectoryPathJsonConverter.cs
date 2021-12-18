using Newtonsoft.Json;

namespace Noggog.Json.IO;

public class DirectoryPathJsonConverter : JsonConverter<DirectoryPath>
{
    public override void WriteJson(JsonWriter writer, DirectoryPath value, JsonSerializer serializer)
    {
        writer.WriteValue(value.RelativePath);
    }

    public override DirectoryPath ReadJson(JsonReader reader, Type objectType, DirectoryPath existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.ValueType == typeof(string))
        {
            return new DirectoryPath((string)reader.Value!);
        }

        return JsonSerializer.Create().Deserialize<DirectoryPath>(reader);
    }
}