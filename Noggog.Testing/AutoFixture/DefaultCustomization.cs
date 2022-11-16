using AutoFixture;
using Noggog.WorkEngine;

namespace Noggog.Testing.AutoFixture;

public class DefaultCustomization : ICustomization
{
    private readonly bool _useMockFileSystem;

    public DefaultCustomization(bool useMockFileSystem = false)
    {
        _useMockFileSystem = useMockFileSystem;
    }
        
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new FileSystemBuilder(_useMockFileSystem));
        fixture.Customizations.Add(new SchedulerBuilder());
        fixture.Customizations.Add(new PathBuilder());
        fixture.Customizations.Add(new CurrentDirectoryPathProviderBuilder());
        fixture.Customizations.Add(new CancellationBuilder());
        fixture.Customizations.Add(new ErrorResponseBuilder());
        fixture.Customizations.Add(new ErrorResponseParameterBuilder());
        fixture.Customizations.Add(new GetResponseBuilder());
        fixture.Customizations.Add(new GetResponseParameterBuilder());
        fixture.Customizations.Add(new ProcessBuilder());
        fixture.Customizations.Add(new LazyBuilder());
        fixture.Customizations.Add(new ExtendedListBuilder());
        fixture.Behaviors.Add(new ObservableEmptyBehavior());
        fixture.Register<IWorkDropoff>(() => new InlineWorkDropoff());
    }
}