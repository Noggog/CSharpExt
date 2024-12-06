namespace Noggog.Nuget.Services.Singleton;

public interface INugetConfigPathProvider
{
    FilePath Path { get; }
}

public class NugetConfigPathProvider : INugetConfigPathProvider
{
    public FilePath Path { get; }

    public NugetConfigPathProvider()
    {
        Path = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NuGet",
            "Nuget.Config");
    }
}