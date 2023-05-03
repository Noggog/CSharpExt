using Noggog.Streams.Binary;

namespace Noggog;

public static class BinaryMemoryWriteStream
{
    public static IBinaryMemoryWriteStream Factory(byte[] buffer, bool isLittleEndian)
    {
        return isLittleEndian
            ? new LittleEndianBinaryMemoryWriteStream(buffer)
            : new BigEndianBinaryMemoryWriteStream(buffer);
    }
    
    public static IBinaryMemoryWriteStream LittleEndian(byte[] buffer)
    {
        return new LittleEndianBinaryMemoryWriteStream(buffer);
    }
    
    public static IBinaryMemoryWriteStream BigEndian(byte[] buffer)
    {
        return new BigEndianBinaryMemoryWriteStream(buffer);
    }
}