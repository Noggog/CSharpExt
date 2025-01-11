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
            builder.RegisterType<TargetMissing>()
                .AsImplementedInterfaces().SingleInstance();
        }
    }

    public interface ISomething
    {
    }

    public class Something : ISomething
    {
    }

    public interface IMissing
    {
    }

    public interface ITargetMissing
    {
    }

    public class TargetMissing : ITargetMissing
    {
        public TargetMissing(IMissing missing)
        {
            
        }
    }

    [Theory, ContainerAutoData(typeof(TypicalModule))]
    public void Typical(ISomething something, int i)
    {
        something.Should().BeOfType<Something>();
    }

    [Theory, ContainerAutoData(typeof(TypicalModule))]
    public void Unrelated(IFileSystem missing, int i)
    {
    }
}