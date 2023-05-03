using Noggog;

namespace CSharpExt.UnitTests;

public class BinaryMemoryStreamTests : IBinaryStreamTests
{
    // No actual edge, but let the tests do their thing
    public override int EdgeLocation => 100;

    public override IBinaryReadStream GetStream(int length)
    {
        return BinaryMemoryReadStream.LittleEndian(GetByteArray(length));
    }
}