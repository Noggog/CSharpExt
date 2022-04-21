using Noggog;

namespace CSharpExt.UnitTests;

public class BinaryMemoryStream_Tests : IBinaryStream_Tests
{
    // No actual edge, but let the tests do their thing
    public override int EdgeLocation => 100;

    public override IBinaryReadStream GetStream(int length)
    {
        return new BinaryMemoryReadStream(
            GetByteArray(length));
    }
}