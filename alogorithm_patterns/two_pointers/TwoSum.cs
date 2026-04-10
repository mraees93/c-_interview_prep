using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*Example Problem - Sum of Pair Equal to Target
Given a sorted array arr (sorted in ascending order) and a target, find if there exists any pair of elements (arr[i], arr[j]) such that 
their sum is equal to the target.

Illustration : 

Input: arr[] = [10, 20, 35, 50], target =70
Output:  true
Explanation : There is a pair (20, 50) with given target.

Input: arr[] = [10, 20, 30], target =70
Output :  false
Explanation : There is no pair with sum 70

Input: arr[] = [-8, 1, 4, 6, 10, 45], target = 16
Output: true
Explanation : There is a pair (6, 10) with given target. */

namespace alogorithm_patterns.two_pointers
{
    class TwoSum
    {
        static bool TwoSum(int[] arr, int target)
        {
            // sort the array
            int left = 0, right = arr.Length - 1;

            while (left < right)
            {
                int sum = arr[left] + arr[right];

                if(sum == target) return true;
                else if (sum < target) left++;
                else right--;
            }

            return false;
        }
    }
}