using System;
using System.Reflection;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture
{
    public class PathBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        { 
            if (request is ParameterInfo p)
            {
                if (!p.Name?.ContainsInsensitive("missing") ?? true)
                {
                    return new NoSpecimen();
                }
                if (p.ParameterType == typeof(FilePath))
                {
                    return new FilePath("C:\\MissingFile");
                }
                else if (p.ParameterType == typeof(DirectoryPath))
                {
                    return new DirectoryPath("C:\\MissingDirectory");
                }
            }
            else if (request is Type t)
            {
                if (t == typeof(FilePath))
                {
                    return new FilePath("C:\\ExistingDirectory\\File");
                }
                else if (t == typeof(DirectoryPath))
                {
                    return new DirectoryPath("C:\\ExistingDirectory");
                }
            }

            return new NoSpecimen();
        }
    }
}