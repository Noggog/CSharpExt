using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;

namespace Noggog.Testing.AutoFixture
{
    public class AutoFakeItEasyDataAttribute : AutoDataAttribute
    {
        public AutoFakeItEasyDataAttribute(bool Strict = true)
            : base(() =>
            {
                var customization = new AutoFakeItEasyCustomization();
                if (Strict)
                {
                    customization.Relay = new FakeItEasyStrictRelay();
                }
                return new Fixture().Customize(customization);
            })
        {
        }
    }
}