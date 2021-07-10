using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using FakeItEasy;
using Noggog.Autofac.Validation;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace CSharpExt.UnitTests.Autofac
{
    public class RegistrationsTests
    {
        [Theory, AutoFakeItEasyData(false)]
        public void NoRegistrationsThrows(
            IContainer container,
            Registrations getRegistrations)
        {
            Assert.Throws<AutofacValidationException>(() =>
            {
                A.CallTo(() => container.ComponentRegistry.Registrations)
                    .Returns(Enumerable.Empty<IComponentRegistration>());
                getRegistrations.Items.ToArray();
            });
        }
    }
}