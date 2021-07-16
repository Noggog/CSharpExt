using System.Linq;
using Autofac;
using Autofac.Core;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class RegistrationsTests
    {
        [Theory, TestData]
        public void NoRegistrationsThrows(
            IContainer container,
            Registrations getRegistrations)
        {
            Assert.Throws<AutofacValidationException>(() =>
            {
                container.ComponentRegistry.Registrations
                    .Returns(Enumerable.Empty<IComponentRegistration>());
                getRegistrations.Items.ToArray();
            });
        }
    }
}