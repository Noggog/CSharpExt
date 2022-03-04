using System;
using AutoFixture.Kernel;
using Noggog.IO;

namespace Noggog.Testing.AutoFixture
{
    public class CurrentDirectoryPathProviderBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type t) return new NoSpecimen();
            if (t == typeof(ICurrentDirectoryProvider))
            {
                return new CurrentDirectoryInjection(PathBuilder.ExistingDirectory);
            }
            return new NoSpecimen();
        }
    }   
}
