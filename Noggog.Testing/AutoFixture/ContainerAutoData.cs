using AutoFixture;
using AutoFixture.Xunit2;

namespace Noggog.Testing.AutoFixture;

public class ContainerAutoData : AutoDataAttribute
{
    public ContainerAutoData(
        Type ContainerType,
        TargetFileSystem FileSystem = TargetFileSystem.Fake,
        bool OmitAutoProperties = false)
        : base(() =>
        {
            var ret = new Fixture();
            ret.Customize(new DefaultCustomization(FileSystem));
            ret.OmitAutoProperties = OmitAutoProperties;
            ret.Customize(new ContainerAutoDataCustomization(ContainerType));
            return ret;
        })
    {
    }
}