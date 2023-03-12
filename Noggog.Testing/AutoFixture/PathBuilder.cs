using System.IO.Abstractions;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Noggog.Testing.IO;

namespace Noggog.Testing.AutoFixture;

public class PathBuilder : ISpecimenBuilder
{
    public static string ExistingDirectory = $"{PathingUtil.DrivePrefix}ExistingDirectory";
    public static string ExistingFile = $"{PathingUtil.DrivePrefix}{Path.Combine("ExistingDirectory", "File.file")}";
        
    public object Create(object request, ISpecimenContext context)
    {
        Type type;
        string? name;
        if (request is ParameterInfo p)
        {
            if (p.Name == null) return new NoSpecimen();
            type = p.ParameterType;
            name = p.Name;
        }
        else if (request is Type t)
        {
            type = t;
            name = null;
        }
        else
        {
            return new NoSpecimen();
        }

        var existing = name != null && name.ContainsInsensitive("existing");
        
        if (type == typeof(FilePath))
        {
            var fp = new FilePath($"{PathingUtil.DrivePrefix}{name}{Path.GetRandomFileName()}");
            if (existing)
            {
                var fs = context.Create<IFileSystem>();
                fs.File.WriteAllText(fp, string.Empty);
            }
            return fp;
        }
        else if (type == typeof(DirectoryPath))
        {
            var dir = new DirectoryPath($"{PathingUtil.DrivePrefix}{name}{Path.GetRandomFileName()}");
            if (existing)
            {
                var fs = context.Create<IFileSystem>();
                fs.Directory.CreateDirectory(dir);
            }

            return dir;
        }

        return new NoSpecimen();
    }
}