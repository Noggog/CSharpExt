using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Noggog.Testing.AutoFixture
{
    public class TestDataAttribute : AutoDataAttribute
    {
        public TestDataAttribute(bool ConfigureMembers = false)
            : base(() =>
            {
                var customization = new AutoNSubstituteCustomization()
                {
                    ConfigureMembers = ConfigureMembers,
                    GenerateDelegates = true
                };

                return new Fixture()
                    .Customize(customization)
                    .Customize(new FileSystemBuilder().ToCustomization())
                    .Customize(new SchedulerBuilder().ToCustomization());
            })
        {
        }
    }
}