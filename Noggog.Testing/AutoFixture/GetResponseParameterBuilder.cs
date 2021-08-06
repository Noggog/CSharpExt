using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture
{
    public class GetResponseParameterBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not ParameterInfo p) return new NoSpecimen();
            if (p.Name == null) return new NoSpecimen();

            if (GetResponseBuilder.IsGetResponseType(p.ParameterType))
            {
                if (IsFail(p.Name))
                {
                    return GetResponseBuilder.GetCreateMethod()
                        .MakeGenericMethod(p.ParameterType.GenericTypeArguments[0])
                        .Invoke(this, new object[] {context, false})!;
                }

                return context.Resolve(new SeededRequest(p.ParameterType, p.ParameterType));
            }
            
            return new NoSpecimen();
        }

        private bool IsFail(string name)
        {
            return name.ContainsInsensitive("fail");
        }
    }
}