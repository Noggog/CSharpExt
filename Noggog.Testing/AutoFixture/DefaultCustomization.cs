using AutoFixture;

namespace Noggog.Testing.AutoFixture
{
    public class DefaultCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new FileSystemBuilder());
            fixture.Customizations.Add(new SchedulerBuilder());
            fixture.Customizations.Add(new PathBuilder());
            fixture.Customizations.Add(new CancellationBuilder());
            fixture.Customizations.Add(new ProcessBuilder());
            fixture.Behaviors.Add(new ObservableEmptyBehavior());
        }
    }
}