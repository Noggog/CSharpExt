using Noggog.Streams.Binary;

namespace Noggog;

public static class BinaryMemoryWriteStream
{
    public static IBinaryMemoryWriteStream Factory(byte[] buffer, bool isLittleEndian = true)
    {
        return isLittleEndian
            ? new LittleEndianBinaryMemoryWriteStream(buffer)
            : new BigEndianBinaryMemoryWriteStream(buffer);
    }
}