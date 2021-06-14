using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace Noggog
{
    public static class SpanExt
    {
        public static ReadOnlySpan<sbyte> AsSByteSpan(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, sbyte>(span);
        }

        public static ReadOnlySpan<short> AsInt16Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, short>(span);
        }

        public static ReadOnlySpan<int> AsInt32Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, int>(span);
        }

        public static ReadOnlySpan<long> AsInt64Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, long>(span);
        }

        public static ReadOnlySpan<ushort> AsUInt16Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, ushort>(span);
        }

        public static ReadOnlySpan<uint> AsUInt32Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, uint>(span);
        }

        public static ReadOnlySpan<ulong> AsUInt64Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, ulong>(span);
        }

        public static ReadOnlySpan<float> AsFloatSpan(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, float>(span);
        }

        public static ReadOnlySpan<double> AsDoubleSpan(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, double>(span);
        }

#if NETSTANDARD2_0
#else
        public static float Float(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadSingleLittleEndian(span);
        }

        public static unsafe double Double(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadDoubleLittleEndian(span);
        }
#endif

        public static unsafe string StringUTF8(this ReadOnlySpan<byte> span)
        {
            fixed (byte* buffer = &MemoryMarshal.GetReference(span))
            {
                return Encoding.UTF8.GetString(buffer, span.Length);
            }
        }

        public static byte UInt8(this ReadOnlySpan<byte> span)
        {
            return span[0];
        }

        public static sbyte Int8(this ReadOnlySpan<byte> span)
        {
            return (sbyte)span[0];
        }

        public static ushort UInt16(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public static short Int16(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public static uint UInt32(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
        }

        public static int Int32(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public static ulong UInt64(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadUInt64LittleEndian(span);
        }

        public static long Int64(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadInt64LittleEndian(span);
        }

#if NETSTANDARD2_0 
#else 
        public static float Float(this ReadOnlyMemorySlice<byte> span) => BinaryPrimitives.ReadSingleLittleEndian(span);

        public static double Double(this ReadOnlyMemorySlice<byte> span) => BinaryPrimitives.ReadDoubleLittleEndian(span);
#endif

        public static string StringUTF8(this ReadOnlyMemorySlice<byte> span) => span.Span.StringUTF8();

        public static byte UInt8(this ReadOnlyMemorySlice<byte> span)
        {
            return span[0];
        }

        public static sbyte Int8(this ReadOnlyMemorySlice<byte> span)
        {
            return (sbyte)span[0];
        }

        public static ushort UInt16(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public static short Int16(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public static uint UInt32(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
        }

        public static int Int32(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public static ulong UInt64(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadUInt64LittleEndian(span);
        }

        public static long Int64(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        public static unsafe string ToHexString(this ReadOnlySpan<byte> span)
        {
            var lookupP = ByteExt.Lookup32UnsafeP;
            var result = new char[span.Length * 2];
            fixed (char* resultP = result)
            {
                uint* resultP2 = (uint*)resultP;
                for (int i = 0; i < span.Length; i++)
                {
                    resultP2[i] = lookupP[span[i]];
                }
            }
            return new string(result);
        }

        public static unsafe string ToHexString(this ReadOnlyMemorySlice<byte> span)
        {
            return ToHexString(span.Span);
        }

        public static unsafe string ToHexString(this MemorySlice<byte> span)
        {
            return ToHexString(span.Span);
        }

        public static unsafe string ToHexString(this Span<byte> span)
        {
            return ToHexString((ReadOnlySpan<byte>)span);
        }
        
        public static LineSplitEnumerator SplitLines(this ReadOnlySpan<char> str)
        {
            // LineSplitEnumerator is a struct so there is no allocation here
            return new LineSplitEnumerator(str);
        }

        // Must be a ref struct as it contains a ReadOnlySpan<char>
        public ref struct LineSplitEnumerator
        {
            private ReadOnlySpan<char> _str;

            public LineSplitEnumerator(ReadOnlySpan<char> str)
            {
                _str = str;
                Current = default;
            }

            // Needed to be compatible with the foreach operator
            public LineSplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                var span = _str;
                if (span.Length == 0) // Reach the end of the string
                    return false;

                var index = span.IndexOfAny('\r', '\n');
                if (index == -1) // The string is composed of only one line
                {
                    _str = ReadOnlySpan<char>.Empty; // The remaining string is an empty string
                    Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
                    return true;
                }

                if (index < span.Length - 1 && span[index] == '\r')
                {
                    // Try to consume the '\n' associated to the '\r'
                    var next = span[index + 1];
                    if (next == '\n')
                    {
                        Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 2));
                        _str = span.Slice(index + 2);
                        return true;
                    }
                }

                Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 1));
                _str = span.Slice(index + 1);
                return true;
            }

            public LineSplitEntry Current { get; private set; }
        }

        public readonly ref struct LineSplitEntry
        {
            public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
            {
                Line = line;
                Separator = separator;
            }

            public ReadOnlySpan<char> Line { get; }
            public ReadOnlySpan<char> Separator { get; }

            // This method allow to deconstruct the type, so you can write any of the following code
            // foreach (var entry in str.SplitLines()) { _ = entry.Line; }
            // foreach (var (line, endOfLine) in str.SplitLines()) { _ = line; }
            // https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct?WT.mc_id=DT-MVP-5003978#deconstructing-user-defined-types
            public void Deconstruct(out ReadOnlySpan<char> line, out ReadOnlySpan<char> separator)
            {
                line = Line;
                separator = Separator;
            }

            // This method allow to implicitly cast the type into a ReadOnlySpan<char>, so you can write the following code
            // foreach (ReadOnlySpan<char> entry in str.SplitLines())
            public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
        }
    }
}
