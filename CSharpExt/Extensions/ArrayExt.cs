using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ArrayExt
    {
        public static Dictionary<Type, Func<object, char>> Converters;
        static ArrayExt()
        {
            Converters = new Dictionary<Type, Func<object, char>>();
            Converters.Add(typeof(bool), (b) =>
            {
                if ((bool)b) return ' ';
                else return (char)219;
            });
            Converters.Add(typeof(char), (c) =>
            {
                char ch = (char)c;
                if (ch == ((char)0)) return ' ';
                return (char)c;
            });
        }

        static public bool Contains<T>(this T[] arr, T val)
        {
            foreach (T t in arr)
            {
                if (t == null) continue;
                if (t.Equals(val))
                {
                    return true;
                }
            }
            return false;
        }

        static public Func<T, char> GetConverter<T>()
        {
            Func<T, char> converter;
            Type type = typeof(T);
            if (!Converters.TryGetValue(type, out Func<object, char> conv))
            {
                converter = new Func<T, char>((t) =>
                {
                    if (t == null)
                        return ' ';
                    string str = t.ToString();
                    return str.Length > 0 ? str[0] : ' ';
                });
            }
            else
            {
                converter = new Func<T, char>((t) =>
                {
                    return conv(t);
                });
            }
            return converter;
        }

        public static P2Int Center<T>(this T[,] array)
        {
            return new P2Int(array.GetLength(0) / 2, array.GetLength(1) / 2);
        }

        public static bool InRange<T>(this T[,] array, int x, int y)
        {
            return x >= 0 && y >= 0 && y < array.GetLength(0) && x < array.GetLength(1);
        }

        public static void Fill<T>(this T[,] array, T to)
        {
            for (int y = 0; y < array.GetLength(0); y++)
                for (int x = 0; x < array.GetLength(1); x++)
                    array[y, x] = to;
        }

        public static void Fill<T>(this T[][] array, T to)
        {
            for (int y = 0; y < array.Length; y++)
            {
                T[] arrx = array[y];
                for (int x = 0; x < arrx.Length; x++)
                {
                    arrx[x] = to;
                }
            }
        }

        public static T[,] Copy<T>(this T[,] array)
        {
            T[,] ret = new T[array.GetLength(0), array.GetLength(1)];
            Array.Copy(array, ret, array.Length);
            return ret;
        }

        public static T[,] Expand<T>(this T[,] array, int buffer)
        {
            T[,] ret = new T[array.GetLength(0) + 2 * buffer, array.GetLength(1) + 2 * buffer];
            for (int y = 0; y < array.GetLength(0); y++)
            {
                for (int x = 0; x < array.GetLength(1); x++)
                {
                    ret[y + buffer, x + buffer] = array[y, x];
                }
            }
            return ret;
        }

        public static T[] ToArray<T>(this T[] arr, int validCount)
        {
            T[] ret = new T[validCount];
            for (int i = 0; i < validCount; i++)
            {
                ret[i] = arr[i];
            }
            return ret;
        }

        #region Jagged3DArray
        #region Instantiation
        public static T[][][] Instantiate3DArray<T>(int size)
        {
            var arr = new T[size][][];
            InstantiateSubArrays(arr, size, size, size);
            return arr;
        }

        public static T[][][] Instantiate3DArray<T>(int size1, int size2, int size3)
        {
            var arr = new T[size1][][];
            InstantiateSubArrays(arr, size1, size2, size3);
            return arr;
        }

        static void InstantiateSubArrays<T>(this T[][][] arr, int size1, int size2, int size3)
        {
            for (int i = 0; i < size1; i++)
            {
                var iArr = new T[size2][];
                arr[i] = iArr;
                for (int j = 0; j < size2; j++)
                {
                    iArr[j] = new T[size3];
                }
            }
        }

        public static T[][][] Instantiate3DArray<T>(T[][][] rhs)
        {
            var arr = new T[rhs.Length][][];
            InstantiateSubArrays(arr, rhs);
            return arr;
        }

        static void InstantiateSubArrays<T>(this T[][][] arr, T[][][] rhs)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                var rhsiArr = rhs[i];
                var iArr = new T[rhsiArr.Length][];
                arr[i] = iArr;
                for (int j = 0; j < rhsiArr.Length; j++)
                {
                    var rhsjkArr = rhsiArr[j];
                    var ijArr = new T[rhsjkArr.Length];
                    iArr[j] = ijArr;
                    for (int k = 0; k < rhsjkArr.Length; k++)
                    {
                        ijArr[k] = rhsjkArr[k];
                    }
                }
            }
        }

        public static T[][][] Instantiate3DArray<T>(int size, T defaultVal)
        {
            var arr = new T[size][][];
            InstantiateSubArrays(arr, defaultVal);
            return arr;
        }

        static void InstantiateSubArrays<T>(this T[][][] arr, T defaultVal)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                var iArr = new T[arr.Length][];
                arr[i] = iArr;
                for (int j = 0; j < arr.Length; j++)
                {
                    var ijArr = new T[arr.Length];
                    iArr[j] = ijArr;
                    for (int k = 0; k < arr.Length; k++)
                    {
                        ijArr[k] = defaultVal;
                    }
                }
            }
        }

        public static T[][][] Instantiate3DArray<T>(int size, Func<T> getFunc)
        {
            return Instantiate3DArray<T>(size, size, size, getFunc);
        }

        public static T[][][] Instantiate3DArray<T>(int size1, int size2, int size3, Func<T> getFunc)
        {
            var arr = new T[size1][][];
            InstantiateSubArrays(arr, size1, size2, size3, getFunc);
            return arr;
        }

        static void InstantiateSubArrays<T>(this T[][][] arr, int size1, int size2, int size3, Func<T> getFunc)
        {
            for (int i = 0; i < size1; i++)
            {
                var iArr = new T[size2][];
                arr[i] = iArr;
                for (int j = 0; j < size2; j++)
                {
                    var ijArr = new T[size3];
                    iArr[j] = ijArr;
                    for (int k = 0; k < size3; k++)
                    {
                        ijArr[k] = getFunc();
                    }
                }
            }
        }

        public static T[][][] Instantiate3DArray<T>(int size, Func<int, int, int, T> getFunc)
        {
            return Instantiate3DArray<T>(size, size, size, getFunc);
        }

        public static T[][][] Instantiate3DArray<T>(int size1, int size2, int size3, Func<int, int, int, T> getFunc)
        {
            var arr = new T[size1][][];
            InstantiateSubArrays(arr, size1, size2, size3, getFunc);
            return arr;
        }

        static void InstantiateSubArrays<T>(this T[][][] arr, int size1, int size2, int size3, Func<int, int, int, T> getFunc)
        {
            for (int i = 0; i < size1; i++)
            {
                var iArr = new T[size2][];
                arr[i] = iArr;
                for (int j = 0; j < size2; j++)
                {
                    var ijArr = new T[size3];
                    iArr[j] = ijArr;
                    for (int k = 0; k < size3; k++)
                    {
                        ijArr[k] = getFunc(i, j, k);
                    }
                }
            }
        }
        #endregion

        #region Fills
        public static void Fill3DArray<T>(this T[][][] arr, T item)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                T[][] arrj = arr[i];
                for (int j = 0; j < arrj.Length; j++)
                {
                    T[] arrk = arrj[j];
                    for (int k = 0; k < arrk.Length; k++)
                    {
                        arrk[k] = item;
                    }
                }
            }
        }

        public static void Fill3DArray<T>(this T[][][] arr, Func<T> getFunc)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                T[][] arrj = arr[i];
                for (int j = 0; j < arrj.Length; j++)
                {
                    T[] arrk = arrj[j];
                    for (int k = 0; k < arrk.Length; k++)
                    {
                        arr[i][j][k] = getFunc();
                    }
                }
            }
        }

        public static void Fill3DArray<T>(this T[][][] arr, Func<int, int, int, T> getFunc)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                T[][] arrj = arr[i];
                for (int j = 0; j < arrj.Length; j++)
                {
                    T[] arrk = arrj[j];
                    for (int k = 0; k < arrk.Length; k++)
                    {
                        arr[i][j][k] = getFunc(i, j, k);
                    }
                }
            }
        }
        #endregion

        #region Copy

        public static T[][][] Copy3DArray<T>(this T[][][] arr, T[][][] rhs)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                var rhsiArr = rhs[i];
                var iArr = arr[i];
                for (int j = 0; j < rhsiArr.Length; j++)
                {
                    var rhsjkArr = rhsiArr[j];
                    var ijArr = iArr[j];
                    for (int k = 0; k < rhsjkArr.Length; k++)
                    {
                        ijArr[k] = rhsjkArr[k];
                    }
                }
            }
            return arr;
        }
        #endregion
        #endregion
    }
}
