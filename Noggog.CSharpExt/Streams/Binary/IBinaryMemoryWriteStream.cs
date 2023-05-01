namespace Noggog;

public interface IBinaryMemoryWriteStream : IBinaryWriteStream
{
    int Position { get; set; }
    int Length { get; }
    int Remaining { get; }
    void Write(ReadOnlySpan<byte> buffer, int offset, int amount);
}