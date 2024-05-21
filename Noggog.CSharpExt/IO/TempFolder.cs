using System.IO.Abstractions;

namespace Noggog.IO;

public interface ITempFolder : IDisposable
{
    DirectoryPath Dir { get; }
}

public class TempFolder : ITempFolder
{
    public DirectoryPath Dir { get; }
    public bool DeleteAfter { get; set; }
    public bool ThrowIfUnsuccessfulDisposal { get; set; }
    private readonly IFileSystem _fileSystem;

    protected TempFolder(
        DirectoryPath dir, 
        bool deleteAfter = true,
        bool deleteBefore = true,
        bool throwIfUnsuccessfulDisposal = true,
        IFileSystem? fileSystem = null)
    {
        Dir = dir;
        _fileSystem = fileSystem ?? IFileSystemExt.DefaultFilesystem;
        DeleteAfter = deleteAfter;
        ThrowIfUnsuccessfulDisposal = throwIfUnsuccessfulDisposal;

        if (deleteBefore && _fileSystem.Directory.Exists(Dir))
        {
            Dir.DeleteEntireFolder();
        }
        _fileSystem.Directory.CreateDirectory(Dir);
    }

    public void Dispose()
    {
        if (DeleteAfter)
        {
            try
            {
                _fileSystem.Directory.DeleteEntireFolder(Dir.Path);
            }
            catch when(!ThrowIfUnsuccessfulDisposal)
            {
            }
        }
    }

    public static TempFolder Factory(
        bool deleteAfter = true, 
        bool deleteBefore = true,
        bool throwIfUnsuccessfulDisposal = true,
        IFileSystem? fileSystem = null)
    {
        return new TempFolder(
            new DirectoryPath(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())),
            deleteAfter: deleteAfter, 
            deleteBefore: deleteBefore,
            throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal,
            fileSystem: fileSystem);
    }

    public static TempFolder FactoryByPath(
        string path,
        bool deleteAfter = true, 
        bool deleteBefore = true,
        bool throwIfUnsuccessfulDisposal = true,
        IFileSystem? fileSystem = null)
    {
        return new TempFolder(
            new DirectoryPath(path),
            deleteAfter: deleteAfter,
            deleteBefore: deleteBefore,
            throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal,
            fileSystem: fileSystem);
    }

    public static TempFolder FactoryByAddedPath(
        string addedFolderPath, 
        bool deleteAfter = true, 
        bool deleteBefore = true,
        bool throwIfUnsuccessfulDisposal = true,
        IFileSystem? fileSystem = null)
    {
        return new TempFolder(
            new DirectoryPath(Path.Combine(Path.GetTempPath(), addedFolderPath)),
            deleteAfter: deleteAfter,
            deleteBefore: deleteBefore,
            throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal,
            fileSystem: fileSystem);
    }

#if NETSTANDARD2_0
#else
        public static AsyncTempFolder Factory(
            int retryCount,
            TimeSpan delay,
            bool deleteAfter = true,
            bool deleteBefore = true,
            bool throwIfUnsuccessfulDisposal = true)
        {
            return new AsyncTempFolder(
                new DirectoryPath(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())),
                retryCount: retryCount,
                delay: delay,
                deleteAfter: deleteAfter,
                deleteBefore: deleteBefore,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal);
        }

        public static AsyncTempFolder FactoryByPath(
            string path,
            int retryCount,
            TimeSpan delay,
            bool deleteAfter = true,
            bool deleteBefore = true,
            bool throwIfUnsuccessfulDisposal = true)
        {
            return new AsyncTempFolder(
                new DirectoryPath(path),
                retryCount: retryCount,
                delay: delay,
                deleteAfter: deleteAfter,
                deleteBefore: deleteBefore,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal);
        }

        public static AsyncTempFolder FactoryByAddedPath(
            string addedFolderPath,
            int retryCount,
            TimeSpan delay,
            bool deleteAfter = true,
            bool deleteBefore = true,
            bool throwIfUnsuccessfulDisposal = true)
        {
            return new AsyncTempFolder(
                new DirectoryPath(Path.Combine(Path.GetTempPath(), addedFolderPath)),
                retryCount: retryCount, 
                delay: delay,
                deleteAfter: deleteAfter,
                deleteBefore: deleteBefore,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal);
        }
#endif
}

#if NETSTANDARD2_0
#else
    public class AsyncTempFolder : TempFolder, IAsyncDisposable
    {
        public int RetryCount;
        public TimeSpan Delay;

        public AsyncTempFolder(
            DirectoryPath dir, 
            int retryCount,
            TimeSpan delay,
            bool deleteAfter = true,
            bool deleteBefore = true,
            bool throwIfUnsuccessfulDisposal = true)
            : base(
                dir,
                deleteBefore: deleteBefore,
                deleteAfter: deleteAfter,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal)
        {
            RetryCount = retryCount;
            Delay = delay;
        }

        public async ValueTask DisposeAsync()
        {
            Exception? ex = null;
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    Dir.DeleteEntireFolder();
                }
                catch (Exception e)
                {
                    ex = e;
                }
                if (!Dir.Exists) return;
                await Task.Delay(Delay).ConfigureAwait(false);
            }
            if (ThrowIfUnsuccessfulDisposal && Dir.Exists)
            {
                if (ex != null)
                {
                    throw ex;
                }
                throw new Exception($"Could not clean up temp directory: {Dir.Path}");
            }
        }
    }
#endif