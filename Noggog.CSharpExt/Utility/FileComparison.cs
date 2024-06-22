using System.IO.Abstractions;

namespace Noggog.Utility;

public static class FileComparison
{
    // https://stackoverflow.com/questions/1358510/how-to-compare-2-files-fast-using-net
    const int BYTES_TO_READ = sizeof(Int64);
    public static bool FilesAreEqual(FilePath first, FilePath second, IFileSystem? fileSystem = null)
    {
        fileSystem ??= fileSystem.GetOrDefault();
        var firstInfo = fileSystem.FileInfo.New(first.Path);
        var secondInfo = fileSystem.FileInfo.New(second.Path);
        if (firstInfo.Length != secondInfo.Length)
            return false;

        if (string.Equals(first.Path, second.Path, StringComparison.OrdinalIgnoreCase))
            return true;

        int iterations = (int)Math.Ceiling((double)firstInfo.Length / BYTES_TO_READ);

        using (var fs1 = first.OpenRead(fileSystem))
        {
            using (var fs2 = second.OpenRead(fileSystem))
            {
                return fs1.ContentsEqual(fs2);
            }
        }
    }

    private static readonly FileCompare comp = new FileCompare();
    public static bool FoldersAreEqual(DirectoryPath first, DirectoryPath second)
    {
        return first.EnumerateFiles(recursive: true)
            .SequenceEqual(
                second.EnumerateFiles(recursive: true),
                comp);
    }

    class FileCompare : IEqualityComparer<FilePath>
    {
        public bool Equals(FilePath x, FilePath y)
        {
            if (!string.Equals(x.Name, y.Name)) return false;
            return FilesAreEqual(x, y);
        }

        public int GetHashCode(FilePath fi)
        {
            throw new NotImplementedException();
        }
    }
}