namespace Noggog;

public interface IBinaryMemoryWriteStream : IBinaryWriteStream
{
    new int Position { get; set; }
    new int Length { get; }
    int Remaining { get; }
    void Write(ReadOnlySpan<byte> buffer, int offset, int amount);
}