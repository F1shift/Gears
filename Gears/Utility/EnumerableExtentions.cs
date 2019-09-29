using System;
using System.Collections.Generic;
using System.Text;

namespace Gears.Utility
{
    internal static class EnumerableExtentions
    {
        public static double Sum(this IEnumerable<double> array)
        {
            double sum = 0;
            foreach (var item in array)
            {
                sum += item;
            }
            return sum;
        }

        public static int Sum(this IEnumerable<int> array)
        {
            int sum = 0;
            foreach (var item in array)
            {
                sum += item;
            }
            return sum;
        }

        public static void AddRanges<T>(this List<T> list, params IEnumerable<T>[] lists)
        {
            for (int i = 0; i < lists.Length; i++)
            {
                list.AddRange(lists[i]);
            }
        }

        //public static T Fisrt<T>(this IEnumerable<T> list)
        //{
        //    var e = list.GetEnumerator();
        //    return e.Current;
        //}

        //public static T Last<T>(this IEnumerable<T> list)
        //{
        //    var e = list.GetEnumerator();
        //    while (e.MoveNext())
        //    {

        //    }
        //    return e.;
        //}

        public static void RemoveLast<T>(this List<T> list)
            {
                if (list.Count > 1)
                    list.RemoveAt(list.Count - 1);
            }

        public static void RemoveFirst<T>(this List<T> list)
        {
            if (list.Count > 1)
                list.RemoveAt(0);
        }

        public static List<T> GetReversed<T>(this List<T> list)
        {
            var newList = new List<T>();
            newList.AddRange(list);
            newList.Reverse();
            return newList;
        }
    }
}
