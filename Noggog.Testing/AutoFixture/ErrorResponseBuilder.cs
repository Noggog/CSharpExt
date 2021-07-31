using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture
{
    public class ErrorResponseBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is ParameterInfo p)
            {
                if (p.Name == null) return new NoSpecimen();
                if (p.ParameterType.GenericTypeArguments.Length == 1
                    && p.ParameterType.Name.StartsWith("GetResponse"))
                {
                    var createResponseMethod = new Lazy<MethodInfo>(() =>
                    {
                        return this.GetType()
                            .GetMethod("CreateGetResponse", BindingFlags.Instance | BindingFlags.NonPublic)!;
                    });
                    
                    if (IsFail(p.Name))
                    {
                        return createResponseMethod.Value
                            .MakeGenericMethod(p.ParameterType.GenericTypeArguments[0])
                            .Invoke(this, new object[] {context, false})!;
                    }
                    
                    return createResponseMethod.Value
                        .MakeGenericMethod(p.ParameterType.GenericTypeArguments[0])
                        .Invoke(this, new object[] {context, true})!;
                }

                if (p.ParameterType == typeof(ErrorResponse))
                {
                    if (IsFail(p.Name))
                    {
                        return ErrorResponse.Failure;
                    }
                    
                    return ErrorResponse.Success;
                }
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

        private GetResponse<T> CreateGetResponse<T>(
            ISpecimenContext context,
            bool successful)
        {
            return GetResponse<T>.Create(
                successful: successful,
                successful ? context.Create<T>() : default(T));
        }
    }
}