using System.Linq;
using Autofac;
using Autofac.Core;
using FluentAssertions;
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

        [Fact]
        public void NormalNeedsValidation()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Class>().AsSelf();
            var cont = builder.Build();
            new Registrations(cont).Items
                .Where(x => x.Key == typeof(Class))
                .SelectMany(x => x.Value)
                .Should().Equal(
                    new Registration(typeof(Class), true));
        }

        [Fact]
        public void InstanceDoesNotNeedValidation()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new Class(new SubClass())).AsSelf();
            var cont = builder.Build();
            new Registrations(cont).Items
                .Where(x => x.Key == typeof(Class))
                .SelectMany(x => x.Value)
                .Should().Equal(
                    new Registration(typeof(Class), false));
        }

        [Fact]
        public void LambdaDoesNotNeedValidation()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => new Class(new SubClass())).AsSelf();
            var cont = builder.Build();
            var regis = new Registrations(cont).Items
                .Where(x => x.Key == typeof(Class))
                .SelectMany(x => x.Value)
                .Should().Equal(
                    new Registration(typeof(Class), false));
        }

        class SubClass
        {
        }

        class Class
        {
            public Class(SubClass cl)
            {
                
            }
        }
    }
}