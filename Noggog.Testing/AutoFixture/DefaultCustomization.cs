using AutoFixture;
using Noggog.IO;
using Noggog.WorkEngine;

namespace Noggog.Testing.AutoFixture;

/// <summary>
/// Default customization with all the typical builders
/// </summary>
public class DefaultCustomization : ICustomization
{
    private readonly TargetFileSystem _targetFileSystem;

    public DefaultCustomization(
        TargetFileSystem targetFileSystem = TargetFileSystem.Fake)
    {
        _targetFileSystem = targetFileSystem;
    }
        
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new FileSystemBuilder(_targetFileSystem));
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
        fixture.Customizations.Add(new Array2dBuilder());
        fixture.Behaviors.Add(new ObservableEmptyBehavior());
        fixture.Register<IWorkDropoff>(() => InlineWorkDropoff.Instance);
        fixture.Register<ICreateStream>(() => NormalFileStreamCreator.Instance);
    }
}