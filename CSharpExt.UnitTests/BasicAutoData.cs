using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace CSharpExt.UnitTests;

public class BasicAutoData : AutoDataAttribute
{
    public BasicAutoData(
        bool ConfigureMembers = false,
        bool UseMockFileSystem = true,
        bool GenerateDelegates = false,
        bool OmitAutoProperties = false)
        : base(() =>
        {
            var ret = new Fixture();
            ret.Customize(new AutoNSubstituteCustomization()
            {
                ConfigureMembers = ConfigureMembers,
                GenerateDelegates = GenerateDelegates
            });
            ret.Customize(new DefaultCustomization(UseMockFileSystem));
            ret.OmitAutoProperties = OmitAutoProperties;
            return ret;
        })
    {
    }
}

public class NoggogInlineData : CompositeDataAttribute
{
    public NoggogInlineData(
        bool ConfigureMembers = false, 
        params object[] ExtraParameters)
        : base(
            new InlineDataAttribute(ExtraParameters), 
            new BasicAutoData(ConfigureMembers: ConfigureMembers))
    {
    }
}