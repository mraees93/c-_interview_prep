using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//Longest Continuous Increasing Subsequence 
/*
Given an unsorted array of integers nums, return the length of the longest continuous increasing subsequence (i.e., a subarray) 
where each adjacent element increases by exactly 1.
A subsequence is continuous if it consists of elements that are directly next 
to each other in the original array.
Example: nums = [1, 2, 3, 5, 6, 7, 8, 4] Output: 4
Explanation: The longest continuous sequence 
incrementing by 1 is [5, 6, 7, 8], which has a length of 4. While [1, 2, 3] also qualifies, it is shorter.
*/
namespace algorithm_patterns.sliding_window
{
    public class LCIS
    {
        public int LongestContinuousIncreasingSubsequence(int[] nums)
        {
            if(nums == null || nums.Length == 0) return 0;

            int maxLength = 1;
            int currentLength = 1;

            for(int i = 1; i < nums.Length; i++)
            {
                // If it increments by exactly 1, keep building the current streak
                if(nums[i] == nums[i - 1] + 1)
                {
                    currentLength++;
                } 
                // If the streak breaks, record the max and reset the streak counter to 1
                else
                {
                    maxLength = Math.Max(maxLength, currentlength);
                    currentLength = 1;
                }
            }
            return Math.Max(maxLength, currentLength);
        }

        //[1, 2, 3, 5, 6, 7, 8, 4]
        public int LongestContinuousIncreasingSubsequence2(int[] nums)
        {
            if(nums == null || nums.Length == 0) return 0;

            int groupId = 0;

            return nums
                .Select((num, index) => new { num, index })
                .GroupBy(x => (x.index > 0 && x.num == nums[x.index - 1] + 1) ? groupId : ++group)
                .Max(group => group.Count());
        }
    }
}