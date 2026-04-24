using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
Example Problem - Maximum Sum of a Subarray with K Elements
Given an array arr[] and an integer k, we need to calculate the maximum 
sum of a subarray having size exactly k.

Input  : arr[] = [5, 2, -1, 0, 3], k = 3
Output : 6
Explanation : We get maximum sum by considering the subaarray [5, 2 , -1]

Input  : arr[] = [1, 4, 2, 10, 23, 3, 1, 0, 20], k = 4 
Output : 39
Explanation : We get maximum sum by adding subarray [4, 2, 10, 23] of size 4.
*/

namespace alogorithm_patterns.sliding_window
{
    public class MaxSum
    {
        static int maxSum(int[] arr, int n, int k)
        {
            //check length of arr
            if(n <= k)
            {
                Console.WriteLine("Invalid");
                return -1;
            }

            int maxSum = 0;

            for(int i = 0; i < k; i++)
            {
                maxSum += arr[i];
            }

            int windowSum = maxSum;

            for(int i = k; i < n; i++)
            {
                windowSum += arr[i] - arr[i - k]; // adds next number to window then subtracts first number in previous window
                maxSum = Math.Max(maxSum, windowSum);
            }// ms=39, ws=39
            //17 + (23 - 1)
            return maxSum;
        }
    } 
}