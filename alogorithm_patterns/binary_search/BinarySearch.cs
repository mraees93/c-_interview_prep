using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace alogorithm_patterns.binary_search
{
    public class BinarySearch
    {
        public static int BinarySearch(int[] arr, int target)
        {
            int left = 0;
            int right = arr.Length - 1;

            while (left <= right) {
                int mid = left + (right - left) / 2; //left=3, right=4, mid=2 In integer math, 1 / 2 is exactly 0. (The .5 is simply discarded).
                
                if(arr[mid] == target) return mid;

                if(arr[mid] < target)
                {
                    left = mid + 1;
                } else
                {
                    right = mid - 1;
                }
            }

            return -1;
        }
    }//l=3, r=4, mid=3
}
/*
Input Array: [2, 3, 4, 10, 40]
Target: 10
Expected Result: 3 (The index of 10) 
*/