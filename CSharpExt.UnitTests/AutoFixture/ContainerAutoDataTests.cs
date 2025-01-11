using System.IO.Abstractions;
using Autofac;
using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests.AutoFixture;

public class ContainerAutoDataTests
{
    public class TypicalModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Something>()
                .AsImplementedInterfaces().SingleInstance();
        }
    }

    public interface ISomething
    {
    }

    public class Something : ISomething
    {
        
    }

    [Theory, ContainerAutoData(typeof(TypicalModule))]
    public void Typical(ISomething something, int i)
    {
        something.Should().BeOfType<Something>();
    }

    [Theory, ContainerAutoData(typeof(TypicalModule))]
    public void Missing(IFileSystem missing, int i)
    {
    }
}