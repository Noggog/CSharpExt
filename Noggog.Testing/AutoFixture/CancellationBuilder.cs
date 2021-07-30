using System;
using System.Reflection;
using System.Threading;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture
{
    public class CancellationBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is ParameterInfo p)
            {
                if (p.Name == null) return new NoSpecimen();
                if (p.Name.ContainsInsensitive("cancelled"))
                {
                    return new CancellationToken(canceled: true);
                }
            }
            else if (request is Type t)
            {
                if (t == typeof(CancellationToken))
                {
                    return new CancellationToken(canceled: false);
                }
            }

            return new NoSpecimen();
        }
    }
}