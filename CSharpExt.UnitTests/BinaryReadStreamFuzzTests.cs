using System.Buffers.Binary;
using System.Text;
using Noggog;

namespace CSharpExt.UnitTests;

/// <summary>
/// Seeded randomized test: applies random in-range operations to a BinaryReadStream
/// and checks every result and Position against the source byte array.
/// Failures report the seed and operation index so they can be replayed.
/// </summary>
public class BinaryReadStreamFuzzTests
{
    private const int SeedsPerConfig = 300;
    private const int OpsPerSeed = 300;

    private enum Primitive
    {
        Boolean, UInt8, Int8, UInt16, Int16, UInt32, Int32, UInt64, Int64, Float, Double,
    }

    private enum PeekMode
    {
        Read,
        Get,
        GetAtOffset,
    }

    private static readonly Primitive[] Primitives = System.Enum.GetValues<Primitive>();

    private class ChunkedStream : MemoryStream
    {
        private readonly int _maxPerRead;
        public ChunkedStream(byte[] data, int maxPerRead) : base(data) => _maxPerRead = maxPerRead;
        public override int Read(byte[] buffer, int offset, int count) =>
            base.Read(buffer, offset, Math.Min(count, _maxPerRead));
        public override int Read(Span<byte> buffer) =>
            base.Read(buffer.Slice(0, Math.Min(buffer.Length, _maxPerRead)));
    }

    public static IEnumerable<object[]> Configs()
    {
        foreach (var buf in new[] { 8, 16, 50, 101 })
        foreach (var chunk in new[] { 0, 7 })
        foreach (var little in new[] { true, false })
        {
            yield return new object[] { buf, chunk, little };
        }
    }

    [Theory]
    [MemberData(nameof(Configs))]
    public void Fuzz(int bufferSize, int maxPerRead, bool littleEndian)
    {
        for (int seed = 0; seed < SeedsPerConfig; seed++)
        {
            RunSeed(seed, bufferSize, maxPerRead, littleEndian);
        }
    }

    private static void RunSeed(int seed, int bufferSize, int maxPerRead, bool littleEndian)
    {
        var rand = new Random(seed * 7919 + bufferSize * 31 + maxPerRead);
        // Lengths: smaller than buffer, exact multiples, multiples +/- a bit, larger
        var length = rand.Next(6) switch
        {
            0 => rand.Next(0, bufferSize),
            1 => bufferSize * rand.Next(1, 8),
            2 => bufferSize * rand.Next(1, 8) + rand.Next(-3, 4),
            _ => rand.Next(0, bufferSize * 12),
        };
        length = Math.Max(0, length);
        var data = new byte[length];
        rand.NextBytes(data);

        Stream baseStream = maxPerRead > 0 ? new ChunkedStream(data, maxPerRead) : new MemoryStream(data);
        var r = new BinaryReadStream(baseStream, bufferSize: bufferSize, isLittleEndian: littleEndian);
        long p = 0;
        string op = "";

        try
        {
            for (int i = 0; i < OpsPerSeed; i++)
            {
                var remaining = (int)(length - p);
                // Amounts biased toward buffer-boundary-sized values
                int Amount(int max)
                {
                    if (max <= 0) return 0;
                    var a = rand.Next(5) switch
                    {
                        0 => rand.Next(0, 9),
                        1 => bufferSize + rand.Next(-2, 3),
                        2 => rand.Next(0, bufferSize * 3),
                        _ => rand.Next(0, bufferSize),
                    };
                    return Math.Clamp(a, 0, max);
                }

                switch (rand.Next(17))
                {
                    case 0:
                    {
                        var target = rand.Next(0, length + 1);
                        op = $"Position = {target}";
                        r.Position = target;
                        p = target;
                        break;
                    }
                    case 1:
                    {
                        var delta = rand.Next(-25, 26);
                        if (p + delta < 0 || p + delta > length) delta = 0;
                        op = $"Position += {delta}";
                        r.Position += delta;
                        p += delta;
                        break;
                    }
                    case 2:
                    {
                        var origin = (SeekOrigin)rand.Next(3);
                        var target = rand.Next(0, length + 1);
                        long offset = origin switch
                        {
                            SeekOrigin.Begin => target,
                            SeekOrigin.Current => target - p,
                            _ => target - length,
                        };
                        op = $"Seek({offset}, {origin})";
                        Assert.Equal(target, r.Seek(offset, origin));
                        p = target;
                        break;
                    }
                    case 3:
                    {
                        var n = Amount(remaining);
                        op = $"Read(byte[], 0, {n})";
                        var buf = new byte[n];
                        Assert.Equal(n, r.Read(buf, 0, n));
                        Check(data, p, n, buf);
                        p += n;
                        break;
                    }
                    case 4:
                    {
                        var n = Amount(remaining);
                        op = $"ReadBytes({n})";
                        Check(data, p, n, r.ReadBytes(n));
                        p += n;
                        break;
                    }
                    case 5:
                    {
                        var n = Amount(remaining);
                        op = $"ReadSpan({n})";
                        Check(data, p, n, r.ReadSpan(n));
                        p += n;
                        break;
                    }
                    case 6:
                    {
                        var n = Amount(remaining);
                        op = $"GetSpan({n})";
                        Check(data, p, n, r.GetSpan(n));
                        break;
                    }
                    case 7:
                    {
                        var n = Amount(remaining);
                        var safe = rand.Next(2) == 0;
                        op = $"ReadMemory({n}, readSafe: {safe})";
                        Check(data, p, n, r.ReadMemory(n, readSafe: safe));
                        p += n;
                        break;
                    }
                    case 8:
                    {
                        var n = Amount(remaining);
                        var off = Amount(remaining - n);
                        var safe = rand.Next(2) == 0;
                        op = $"GetMemory({n}, {off}, readSafe: {safe})";
                        Check(data, p + off, n, r.GetMemory(n, off, readSafe: safe));
                        break;
                    }
                    case 9:
                    {
                        var n = Amount(remaining);
                        op = $"ReadStringUTF8({n})";
                        Assert.Equal(Encoding.UTF8.GetString(data, (int)p, n), r.ReadStringUTF8(n));
                        p += n;
                        break;
                    }
                    case 10:
                    {
                        var n = Amount(remaining);
                        var off = Amount(remaining - n);
                        op = $"GetStringUTF8({n}, {off})";
                        Assert.Equal(Encoding.UTF8.GetString(data, (int)(p + off), n), r.GetStringUTF8(n, off));
                        break;
                    }
                    case 11:
                    case 12:
                    case 13:
                    {
                        var kind = Primitives[rand.Next(Primitives.Length)];
                        var size = PrimitiveSize(kind);
                        if (remaining < size) break;
                        var mode = (PeekMode)rand.Next(3);
                        var off = mode == PeekMode.GetAtOffset ? Amount(remaining - size) : 0;
                        op = $"{(mode == PeekMode.Read ? "Read" : "Get")}{kind}({(mode == PeekMode.GetAtOffset ? off.ToString() : "")})";
                        var expected = Expected(data, (int)(p + off), kind, littleEndian);
                        object actual = mode switch
                        {
                            PeekMode.Read => ReadPrimitive(r, kind),
                            PeekMode.Get => GetPrimitive(r, kind),
                            _ => GetPrimitiveAt(r, kind, off),
                        };
                        Assert.Equal(expected, actual);
                        if (mode == PeekMode.Read) p += size;
                        break;
                    }
                    case 14:
                    {
                        if (remaining < 1) break;
                        op = "ReadByte()";
                        Assert.Equal(data[p], r.ReadByte());
                        p += 1;
                        break;
                    }
                    case 15:
                    {
                        var n = Amount(remaining);
                        op = $"WriteTo(ms, {n})";
                        var ms = new MemoryStream();
                        r.WriteTo(ms, n);
                        Check(data, p, n, ms.ToArray());
                        p += n;
                        break;
                    }
                    case 16:
                    {
                        op = "Remaining/Complete";
                        Assert.Equal(length - p, r.Remaining);
                        Assert.Equal(p >= length, r.Complete);
                        break;
                    }
                }
                Assert.True(p == r.Position, $"Position mismatch: expected {p}, got {r.Position}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Fuzz failure: seed={seed} bufferSize={bufferSize} maxPerRead={maxPerRead} littleEndian={littleEndian} length={length} pos={p} lastOp=[{op}]: {ex.Message}",
                ex);
        }
    }

    private static void Check(byte[] data, long pos, int n, ReadOnlySpan<byte> actual)
    {
        Assert.Equal(n, actual.Length);
        if (!data.AsSpan((int)pos, n).SequenceEqual(actual))
        {
            throw new Exception($"Byte mismatch reading {n} bytes at {pos}");
        }
    }

    private static int PrimitiveSize(Primitive kind) => kind switch
    {
        Primitive.Boolean or Primitive.UInt8 or Primitive.Int8 => 1,
        Primitive.UInt16 or Primitive.Int16 => 2,
        Primitive.UInt32 or Primitive.Int32 or Primitive.Float => 4,
        Primitive.UInt64 or Primitive.Int64 or Primitive.Double => 8,
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    private static object Expected(byte[] data, int at, Primitive kind, bool little)
    {
        var s = data.AsSpan(at);
        return kind switch
        {
            Primitive.Boolean => s[0] > 0,
            Primitive.UInt8 => s[0],
            Primitive.Int8 => (sbyte)s[0],
            Primitive.UInt16 => little ? BinaryPrimitives.ReadUInt16LittleEndian(s) : BinaryPrimitives.ReadUInt16BigEndian(s),
            Primitive.Int16 => little ? BinaryPrimitives.ReadInt16LittleEndian(s) : BinaryPrimitives.ReadInt16BigEndian(s),
            Primitive.UInt32 => little ? BinaryPrimitives.ReadUInt32LittleEndian(s) : BinaryPrimitives.ReadUInt32BigEndian(s),
            Primitive.Int32 => little ? BinaryPrimitives.ReadInt32LittleEndian(s) : BinaryPrimitives.ReadInt32BigEndian(s),
            Primitive.UInt64 => little ? BinaryPrimitives.ReadUInt64LittleEndian(s) : BinaryPrimitives.ReadUInt64BigEndian(s),
            Primitive.Int64 => little ? BinaryPrimitives.ReadInt64LittleEndian(s) : BinaryPrimitives.ReadInt64BigEndian(s),
            Primitive.Float => little ? BinaryPrimitives.ReadSingleLittleEndian(s) : BinaryPrimitives.ReadSingleBigEndian(s),
            Primitive.Double => little ? BinaryPrimitives.ReadDoubleLittleEndian(s) : BinaryPrimitives.ReadDoubleBigEndian(s),
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    private static object ReadPrimitive(BinaryReadStream r, Primitive kind) => kind switch
    {
        Primitive.Boolean => r.ReadBoolean(),
        Primitive.UInt8 => r.ReadUInt8(),
        Primitive.Int8 => r.ReadInt8(),
        Primitive.UInt16 => r.ReadUInt16(),
        Primitive.Int16 => r.ReadInt16(),
        Primitive.UInt32 => r.ReadUInt32(),
        Primitive.Int32 => r.ReadInt32(),
        Primitive.UInt64 => r.ReadUInt64(),
        Primitive.Int64 => r.ReadInt64(),
        Primitive.Float => r.ReadFloat(),
        Primitive.Double => r.ReadDouble(),
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    private static object GetPrimitive(BinaryReadStream r, Primitive kind) => kind switch
    {
        Primitive.Boolean => r.GetBoolean(),
        Primitive.UInt8 => r.GetUInt8(),
        Primitive.Int8 => r.GetInt8(),
        Primitive.UInt16 => r.GetUInt16(),
        Primitive.Int16 => r.GetInt16(),
        Primitive.UInt32 => r.GetUInt32(),
        Primitive.Int32 => r.GetInt32(),
        Primitive.UInt64 => r.GetUInt64(),
        Primitive.Int64 => r.GetInt64(),
        Primitive.Float => r.GetFloat(),
        Primitive.Double => r.GetDouble(),
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    private static object GetPrimitiveAt(BinaryReadStream r, Primitive kind, int off) => kind switch
    {
        Primitive.Boolean => r.GetBoolean(off),
        Primitive.UInt8 => r.GetUInt8(off),
        Primitive.Int8 => r.GetInt8(off),
        Primitive.UInt16 => r.GetUInt16(off),
        Primitive.Int16 => r.GetInt16(off),
        Primitive.UInt32 => r.GetUInt32(off),
        Primitive.Int32 => r.GetInt32(off),
        Primitive.UInt64 => r.GetUInt64(off),
        Primitive.Int64 => r.GetInt64(off),
        Primitive.Float => r.GetFloat(off),
        Primitive.Double => r.GetDouble(off),
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };
}
