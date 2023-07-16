using System.IO.Abstractions;

namespace Noggog.IO;

public interface ICreateStream
{
    Stream GetStreamFor(IFileSystem fileSystem, FilePath path, bool write);
}

public class NormalFileStreamCreator : ICreateStream
{
    public static readonly NormalFileStreamCreator Instance = new();
    
    public Stream GetStreamFor(IFileSystem fileSystem, FilePath path, bool write)
    {
        return fileSystem.File.Open(path, write ? FileMode.Create : FileMode.Open, write ? FileAccess.Write : FileAccess.Read, FileShare.Read);
    }
}

public class NoPreferenceStreamCreator : ICreateStream
{
    public Stream GetStreamFor(IFileSystem fileSystem, FilePath path, bool write)
    {
        throw new NotImplementedException();
    }
}

public static class ICreateStreamExt
{
    public static ICreateStream GetOrFallback(this ICreateStream? createStream, Func<ICreateStream> fallback)
    {
        if (createStream == null || createStream is NoPreferenceStreamCreator)
        {
            return fallback();
        }

        return createStream;
    }
}