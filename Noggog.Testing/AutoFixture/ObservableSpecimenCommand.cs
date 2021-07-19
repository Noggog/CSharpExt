using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reflection;
using AutoFixture.Kernel;
using NSubstitute;
using NSubstitute.Core;
using System.Linq;
using System.Reactive.Disposables;
using NSubstitute.Exceptions;

namespace Noggog.Testing.AutoFixture
{
    public class ObservableSpecimenCommand : ISpecimenCommand
    {
        public void Execute(object specimen, ISpecimenContext context)
        {
            try
            {
                SubstitutionContext.Current.GetCallRouterFor(specimen);
            }
            catch (NotASubstituteException)
            {
                return;
            }
            
            var method = new Lazy<MethodInfo>(() =>
            {
                return this.GetType()
                    .GetMethod("CreateEmpty", BindingFlags.Instance | BindingFlags.NonPublic)!;
            });

            foreach (var prop in specimen.GetType()
                .GetProperties()
                .Where(x => x.CanRead && x.GetGetMethod()!.IsVirtual))
            {
                if (!IsObservable(prop.PropertyType)) continue;

                method.Value
                    .MakeGenericMethod(prop.PropertyType.GenericTypeArguments[0])
                    .Invoke(this, new[] { specimen, prop });
            }
        }

        public static bool IsObservable(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IObservable<>);
        }

        private void CreateEmpty<T>(object specimen, PropertyInfo prop)
        {
            var value = prop.GetValue(specimen, null);
            value.Returns(Observable.Empty<T>());
        }
    }
}