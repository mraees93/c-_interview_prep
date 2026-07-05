using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace algorithm_patterns.two_pointers
{
    public class SortArrayByParity
    {
        //my slower solution
        public int[] SortArrayByParity1(int[] nums)
        {
            if (nums.Length == 1) return nums;

            List<int> even = new List<int>();
            List<int> odd = new List<int>();

            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] % 2 == 0) even.Add(nums[i]);
                else odd.Add(nums[i]);
            }

            int[] merged = even.Concat(odd).ToArray();
            return merged;
        }

        // optimal performance 
        public int[] SortArrayByParity2(int[] nums)
        {
            int left = 0;
            int right = nums.Length - 1;

            while (left < right)
            {
                // Swap if left is odd and right is even
                if (nums[left] % 2 != 0 && nums[right] % 2 == 0)
                {
                    int temp = nums[left];
                    nums[left] = nums[right];
                    nums[right] = temp;
                    left++;
                    right--;
                }

                // Advance left if it already points to an even number
                if (left < nums.Length && nums[left] % 2 == 0)
                {
                    left++;
                }

                // Decrement right if it already points to an odd number
                if (right >= 0 && nums[right] % 2 != 0)
                {
                    right--;
                }
            }

            return nums;
        }

        // readability
        public int[] SortArrayByParity3(int[] nums)
        {
            return nums.OrderBy(x => x % 2 != 0).ToArray();
            /*
            LINQ's OrderBy sorts elements based on a key.In C#, boolean values have an implicit order where false comes 
            before true.Because even numbers evaluate to false and odd numbers evaluate to true, OrderBy naturally pushes 
            all the false values (evens) to the front and all the true values (odds) to the back.
            */
        }
    }
}