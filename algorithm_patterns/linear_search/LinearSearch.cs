using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace algorithm_patterns
{
    public class LinearSearch
    {
        public static int search(int[] arr, int x)
        {
            int n = arr.Length;

            // Iterate over the array in order to
            // find the key x
            for (int i = 0; i < n; i++)
            {
                if (arr[i] == x)
                    return i;
            }
            return -1;
        }
    }
}