namespace Noggog;

public interface IPath
{
    bool Exists { get; }
    string Path { get; }
    FileName Name { get; }
}