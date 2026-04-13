using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alogorithm_patterns.binary_search
{
    public class BinarySearchRecursive
    {
        public static int BinarySearchRecursive(int[] arr, int left, int right, int target)
        {
            if (left > right) return -1;

            int mid = left + (right - left) / 2;
            if (arr[mid] == target) return mid;

            return arr[mid] > target ? 
                    BinarySearchRecursive(arr, left, mid - 1, target) : 
                    BinarySearchRecursive(arr, mid + 1, right, target);
        }
    }
}
/*
Input Array: [2, 3, 4, 10, 40]
Target: 10
Expected Result: 3 (The index of 10) 
*/