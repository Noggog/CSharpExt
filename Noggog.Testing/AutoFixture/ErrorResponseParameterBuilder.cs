using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture
{
    public class ErrorResponseParameterBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not ParameterInfo p) return new NoSpecimen();
            if (p.Name == null) return new NoSpecimen();
            
            if (p.ParameterType == typeof(ErrorResponse))
            {
                if (IsFail(p.Name))
                {
                    return ErrorResponse.Create(
                        successful: false, 
                        context.Create<string>(), 
                        ex: null);
                }

                return context.Create<ErrorResponse>();
            }
            
            return new NoSpecimen();
        }

        private bool IsPassing(string name)
        {
            return name.ContainsInsensitive("success")
                   || name.ContainsInsensitive("pass");
        }

        private bool IsFail(string name)
        {
            return name.ContainsInsensitive("fail");
        }
    }
}