using System;
using System.Collections.Generic;
using System.Text;

namespace Gears.Math
{
    internal static class ArrayExtentions
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
    }
}
