using System.Runtime.InteropServices;

namespace Noggog;

public static class ByteExt
{
    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int memcmp(byte[] b1, byte[] b2, long count);

    public static bool IsInRange(this byte d, byte min, byte max)
    {
        if (d < min) return false;
        if (d > max) return false;
        return true;
    }

    public static byte InRange(this byte d, byte min, byte max)
    {
        if (d < min) throw new ArgumentException($"{d} was lower than the minimum {min}.");
        if (d > max) throw new ArgumentException($"{d} was greater than the maximum {max}.");
        return d;
    }

    public static byte PutInRange(this byte d, byte min, byte max)
    {
        if (d < min) return min;
        if (d > max) return max;
        return d;
    }

    private unsafe static readonly uint[] _lookup32Unsafe = CreateLookup32Unsafe();
    public unsafe static readonly uint* Lookup32UnsafeP = (uint*)GCHandle.Alloc(_lookup32Unsafe, GCHandleType.Pinned).AddrOfPinnedObject();

    private static uint[] CreateLookup32Unsafe()
    {
        var result = new uint[256];
        for (int i = 0; i < 256; i++)
        {
            string s = i.ToString("X2");
            if (BitConverter.IsLittleEndian)
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            else
                result[i] = ((uint)s[1]) + ((uint)s[0] << 16);
        }
        return result;
    }

    public unsafe static string ToHexString(this byte[] bytes)
    {
        var lookupP = Lookup32UnsafeP;
        var result = new char[bytes.Length * 2];
        fixed (byte* bytesP = bytes)
        fixed (char* resultP = result)
        {
            uint* resultP2 = (uint*)resultP;
            for (int i = 0; i < bytes.Length; i++)
            {
                resultP2[i] = lookupP[bytesP[i]];
            }
        }
        return new string(result);
    }
        
    public static unsafe bool EqualsFast(this byte[]? b1, byte[]? b2)
    {
        if (b1 == null && b2 == null) return true;
        if (b1 == null || b2 == null) return false;
        return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
    }

    public static unsafe bool CharBytesEqualsFast(byte[]? strA, byte[]? strB)
    {
        if (strA == null && strB == null) return true;
        if (strA == null || strB == null) return false;
        int length = strA.Length;
        if (length != strB.Length)
        {
            return false;
        }
        fixed (byte* str = strA)
        {
            byte* chPtr = str;
            fixed (byte* str2 = strB)
            {
                byte* chPtr2 = str2;
                byte* chPtr3 = chPtr;
                byte* chPtr4 = chPtr2;
                while (length >= 10)
                {
                    if ((((*(((int*)chPtr3)) != *(((int*)chPtr4))) || (*(((int*)(chPtr3 + 2))) != *(((int*)(chPtr4 + 2))))) || ((*(((int*)(chPtr3 + 4))) != *(((int*)(chPtr4 + 4)))) || (*(((int*)(chPtr3 + 6))) != *(((int*)(chPtr4 + 6)))))) || (*(((int*)(chPtr3 + 8))) != *(((int*)(chPtr4 + 8)))))
                    {
                        break;
                    }
                    chPtr3 += 10;
                    chPtr4 += 10;
                    length -= 10;
                }
                while (length > 0)
                {
                    if (*(((int*)chPtr3)) != *(((int*)chPtr4)))
                    {
                        break;
                    }
                    chPtr3 += 2;
                    chPtr4 += 2;
                    length -= 2;
                }
                return (length <= 0);
            }
        }
    }
}