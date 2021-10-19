using System;
using System.Collections.Generic;

namespace PDCUtility
{
    public class Sorting
    {
        public delegate int ComparerDelegate<T>(T A, T B);

        public static int CompareNullable(double? a, double? b)
        {
            if (a.HasValue)
                if (b.HasValue)
                    return a.Value.CompareTo(b.Value);
                else
                    return 1;
            else
                if (b.HasValue)
                return -1;
            else
                return 0;
        }

        public static int CompareNullable(int? a, int? b)
        {
            if (a.HasValue)
                if (b.HasValue)
                    return a.Value.CompareTo(b.Value);
                else
                    return 1;
            else
                if (b.HasValue)
                return -1;
            else
                return 0;
        }

        public static T[] SortAscending<T>(T[] array) where T : IComparable
        {
            try
            {
                List<T> oList = new List<T>();
                oList.AddRange(array);
                oList.Sort((a, b) => a.CompareTo(b));
                return oList.ToArray();
            }
            catch
            {
                return array;
            }
        }

        public static T[] SortAscending<T>(T[] array, ComparerDelegate<T> fnCompare)
        {
            try
            {
                List<T> oList = new List<T>();
                oList.AddRange(array);
                oList.Sort((a, b) => fnCompare(a, b));
                return oList.ToArray();
            }
            catch
            {
                return array;
            }
        }

        public static List<T> SortAscending<T>(List<T> list) where T : IComparable
        {
            try
            {
                list.Sort((a, b) => a.CompareTo(b));
                return list;
            }
            catch
            {
                return list;
            }
        }

        public static List<T> SortAscending<T>(List<T> list, ComparerDelegate<T> fnCompare)
        {
            try
            {
                list.Sort((a, b) => fnCompare(a, b));
                return list;
            }
            catch
            {
                return list;
            }
        }

        public static T[] SortDescending<T>(T[] array) where T : IComparable
        {
            try
            {
                List<T> oList = new List<T>();
                oList.AddRange(array);
                oList.Sort((a, b) => b.CompareTo(a));
                return oList.ToArray();
            }
            catch
            {
                return array;
            }
        }

        public static T[] SortDescending<T>(T[] array, ComparerDelegate<T> fnCompare)
        {
            try
            {
                List<T> oList = new List<T>();
                oList.AddRange(array);
                oList.Sort((a, b) => fnCompare(b, a));
                return oList.ToArray();
            }
            catch
            {
                return array;
            }
        }

        public static List<T> SortDescending<T>(List<T> list) where T : IComparable
        {
            try
            {
                list.Sort((a, b) => b.CompareTo(a));
                return list;
            }
            catch
            {
                return list;
            }
        }

        public static List<T> SortDescending<T>(List<T> list, ComparerDelegate<T> fnCompare)
        {
            try
            {
                list.Sort((a, b) => fnCompare(b, a));
                return list;
            }
            catch
            {
                return list;
            }
        }
    }
}