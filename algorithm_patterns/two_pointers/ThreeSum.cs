using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace algorithm_patterns.two_pointers
{
    public class ThreeSum
    {
        public IList<IList<int>> ThreeSum(int[] nums)
        {
            var res = new List<IList<int>>();

            // Step 1: Sort the array (O(n log n))
            Array.Sort(nums);

            for (int i = 0; i < nums.Length - 2; i++)
            {
                // Step 2: Skip duplicate values for the first element
                if (i > 0 && nums[i] == nums[i - 1])
                {
                    continue;
                }

                // Step 3: Initialize two pointers
                int left = i + 1;
                int right = nums.Length - 1;

                while (left < right)
                {
                    int threeSum = nums[i] + nums[left] + nums[right];

                    if (threeSum == 0)
                    {
                        res.Add(new List<int> { nums[i], nums[left], nums[right] });
                        left++;
                        right--;

                        // Step 4: Skip duplicate values for the left pointer
                        while (left < right && nums[left] == nums[left - 1])
                        {
                            left++;
                        }
                    }
                    else if (threeSum < 0)
                    {
                        left++; // Sum too small, move left pointer right
                    }
                    else
                    {
                        right--; // Sum too large, move right pointer left
                    }
                }
            }

            return res;
        }
    }
}