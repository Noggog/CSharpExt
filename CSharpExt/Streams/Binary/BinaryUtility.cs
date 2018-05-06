using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class BinaryUtility
    {
        public static String BytesToString(byte[] bytes, int offset, int count)
        {
            char[] chars = new char[count];
            for (int i = 0; i < count; i++)
            {
                chars[i] = (char)bytes[i + offset];
            }
            return new string(chars);
        }

        public static String BytesToString(byte[] bytes)
        {
            return BytesToString(bytes, 0, bytes.Length);
        }
    }
}
