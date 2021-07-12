using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;

namespace Noggog.Testing.AutoFixture
{
    public class AutoFakeItEasyDataAttribute : AutoDataAttribute
    {
        public AutoFakeItEasyDataAttribute(bool Strict = true, bool ConfigureMembers = false)
            : base(() =>
            {
                var customization = new AutoFakeItEasyCustomization()
                {
                    ConfigureMembers = ConfigureMembers,
                    GenerateDelegates = true
                };
                if (Strict)
                {
                    customization.Relay = new FakeItEasyStrictRelay();
                }

                return new Fixture()
                    .Customize(customization)
                    .Customize(new FileSystemBuilder().ToCustomization())
                    .Customize(new SchedulerBuilder().ToCustomization());
            })
        {
        }
    }
}