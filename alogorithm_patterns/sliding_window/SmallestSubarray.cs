using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//variable sliding window
//Finding the minimum length required to reach a target sum.
namespace alogorithm_patterns.sliding_window
{
    public class SmallestSubarray
    {
        public int SmallestSubarray(int[] nums, int target)
        {
            int left = 0;
            int currentSum = 0;
            int minLength = int.MaxValue;

            for(int right = 0; right < nums.Length; right++)
            {
                currentSum += nums[right];

                while(currentSum >= target)
                {
                    minLength = Math.Min(minLength, right - left + 1);
                    currentSum -= nums[left];
                    left++;
                }
            }

            return minLength == int.MaxValue ? 0 : minLength;
        }
    } // currentSum = 6, minlength = 2, left=4, right=5
}
/*
Target: 7 
Array (nums): [2, 3, 1, 2, 4, 3]
*/

/*
Target unreachable: nums = [1, 1, 1], target = 10. The sum of all elements is less than the target. The expected output is 0.
Single element meets target: nums = [1, 10, 1], target = 10. The window should shrink until it only contains [10]. The expected output is 1.
Target is zero: Depending on requirements, if the target is 
 or 
 and the array contains positive integers, the output should be 1.
 */