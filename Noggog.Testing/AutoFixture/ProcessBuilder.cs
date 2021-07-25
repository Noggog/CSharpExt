using System;
using System.Diagnostics;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture
{
    public class ProcessBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is ParameterInfo p)
            {
                if (p.Name == null) return new NoSpecimen();
                if (p.Name.ContainsInsensitive("existing"))
                {
                    return new ProcessStartInfo(
                        PathBuilder.ExistingFile,
                        context.Create<string>())
                    {
                        WorkingDirectory = PathBuilder.ExistingDirectory
                    };
                }
            }
            else if (request is Type t)
            {
                if (t == typeof(ProcessStartInfo))
                {
                    return new ProcessStartInfo(
                        context.Create<FilePath>().Path,
                        context.Create<string>())
                    {
                        WorkingDirectory = context.Create<DirectoryPath>().Path
                    };
                }
            }

            return new NoSpecimen();
        }
    }
}