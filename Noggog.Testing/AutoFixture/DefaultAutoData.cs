﻿using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Xunit;

namespace Noggog.Testing.AutoFixture;

/// <summary>
/// AutoFixture attribute with default customizations
/// </summary>
public class DefaultAutoData : AutoDataAttribute
{
    public DefaultAutoData(
        bool ConfigureMembers = false,
        TargetFileSystem FileSystem = TargetFileSystem.Fake,
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
            ret.Customize(new DefaultCustomization(FileSystem));
            ret.OmitAutoProperties = OmitAutoProperties;
            return ret;
        })
    {
    }
}

public class DefaultInlineData : CompositeDataAttribute
{
    public DefaultInlineData(
        bool ConfigureMembers = false, 
        params object[] ExtraParameters)
        : base(
            new InlineDataAttribute(ExtraParameters), 
            new DefaultAutoData(ConfigureMembers: ConfigureMembers))
    {
    }
}