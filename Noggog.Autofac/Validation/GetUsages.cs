using System;
using System.Collections.Generic;

namespace Noggog.Autofac.Validation
{
    public interface IGetUsages
    {
        HashSet<Type> Get(params Type[] concreteTypes);
    }

    public class GetUsages : IGetUsages
    {
        public HashSet<Type> Get(params Type[] concreteTypes)
        {
            var ret = new HashSet<Type>();
            foreach (var type in concreteTypes)
            {
                foreach (var ctor in type.GetConstructors())
                {
                    foreach (var param in ctor.GetParameters())
                    {
                        ret.Add(param.ParameterType);
                    }
                }
            }

            return ret;
        }
    }
}