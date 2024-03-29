using System.Collections.Specialized;

namespace Noggog;

public static class BitVector32Ext
{
    private static readonly int[] _masks;

    static BitVector32Ext()
    {
        _masks = new int[32];
        for (int i = 1; i < 32; i++)
        {
            _masks[i] = BitVector32.CreateMask(_masks[i - 1]);
        }
    }

    public static int GetNthMask(int i)
    {
        return _masks[i];
    }
}