using System;
using System.Reflection;
using AutoFixture.Kernel;
using FakeItEasy;

namespace Noggog.Testing.AutoFixture
{
    public class FakeItEasyStrictRelay : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (!this.IsSatisfiedBy(request))
                return new NoSpecimen();

            if (request is not Type type)
                return new NoSpecimen();

            var fakeFactoryMethod = this.GetType()
                .GetMethod("CreateStrictFake", BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod((Type) request);

            var fake = fakeFactoryMethod.Invoke(this, new object[0]);

            return fake!;
        }

        public bool IsSatisfiedBy(object request)
        {
            var t = request as Type;
            return (t != null) && ((t.IsAbstract) || (t.IsInterface));
        }

        private T CreateStrictFake<T>()
            where T : class
        {
            return A.Fake<T>(s => s.Strict());
        }
    }
}