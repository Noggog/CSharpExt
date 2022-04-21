using System.Reflection;
using AutoFixture.Kernel;
using Noggog.Testing.IO;

namespace Noggog.Testing.AutoFixture;

public class PathBuilder : ISpecimenBuilder
{
    public static string ExistingDirectory = $"{PathingUtil.DrivePrefix}ExistingDirectory";
    public static string ExistingFile = $"{PathingUtil.DrivePrefix}{Path.Combine("ExistingDirectory", "File")}";
        
    public object Create(object request, ISpecimenContext context)
    { 
        if (request is ParameterInfo p)
        {
            if (p.Name == null) return new NoSpecimen();
            if (p.Name.ContainsInsensitive("missing"))
            {
                if (p.ParameterType == typeof(FilePath))
                {
                    return new FilePath($"{PathingUtil.DrivePrefix}MissingFile");
                }
                else if (p.ParameterType == typeof(DirectoryPath))
                {
                    return new DirectoryPath($"{PathingUtil.DrivePrefix}MissingDirectory");
                }
            }
            else if (p.Name.ContainsInsensitive("existing"))
            {
                if (p.ParameterType == typeof(FilePath))
                {
                    return new FilePath(Path.Combine(ExistingDirectory, "File"));
                }
                else if (p.ParameterType == typeof(DirectoryPath))
                {
                    return new DirectoryPath(ExistingDirectory);
                }
            }
                
            if (p.ParameterType == typeof(FilePath))
            {
                return new FilePath(Path.Combine(ExistingDirectory, $"{p.Name}{Path.GetRandomFileName()}"));
            }
            else if (p.ParameterType == typeof(DirectoryPath))
            {
                return new DirectoryPath($"{PathingUtil.DrivePrefix}{p.Name}{Path.GetRandomFileName()}");
            }
        }
        else if (request is Type t)
        {
            if (t == typeof(FilePath))
            {
                return new FilePath(Path.Combine(ExistingDirectory, Path.GetRandomFileName()));
            }
            else if (t == typeof(DirectoryPath))
            {
                return new DirectoryPath($"{PathingUtil.DrivePrefix}{Path.GetRandomFileName()}");
            }
        }

        return new NoSpecimen();
    }
}