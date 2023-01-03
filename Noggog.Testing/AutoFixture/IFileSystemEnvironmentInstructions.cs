using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public interface IFileSystemEnvironmentInstructions
{
    IEnumerable<FilePath> FilePaths(ISpecimenContext context);
    IEnumerable<DirectoryPath> DirectoryPaths(ISpecimenContext context);
}

public class DefaultFileSystemEnvironmentInstructions : IFileSystemEnvironmentInstructions
{
    public virtual IEnumerable<FilePath> FilePaths(ISpecimenContext context)
    {
        yield return PathBuilder.ExistingFile;
    }

        
    public virtual IEnumerable<DirectoryPath> DirectoryPaths(ISpecimenContext context)
    {
        yield return PathBuilder.ExistingDirectory;
    }
}