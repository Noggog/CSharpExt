using Newtonsoft.Json;
using Noggog.Json.IO;

namespace Noggog.Json
{
    public static class JsonConvertersMixIn
    {
        public static readonly FilePathJsonConverter FilePathConverter = new();
        public static readonly DirectoryPathJsonConverter DirectoryPathConverter = new();

        public static void AddNoggogConverters(this JsonSerializerSettings settings)
        { 
            settings.Converters.Add(FilePathConverter);
            settings.Converters.Add(DirectoryPathConverter);
        }
    }
}
