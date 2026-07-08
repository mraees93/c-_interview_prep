using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*
You are given an unsorted array (e.g., [4, 8, 1, 9, 3, 2]), and you need to find the length of the longest elements 
sequence that can be rearranged to increment by \(1\) (e.g., [1, 2, 3, 4], which has a length of \(4\)). The elements 
do not need to be next to each other or in order in the original array [1].Optimal Time Complexity: \(\mathcal{O}(n)\) 
using a HashSet<int>
*/
namespace algorithm_patterns.Hash_Map_Frequency
{
    public class Solution
    {
        public int LongestConsecutiveSequence(int[] nums)
        {
            // Store all numbers in a HashSet for O(1) lookups
            HashSet<int> numSet = new HashSet(nums);
            int longestStreak = 0;

            foreach(int num in numSet)
            {
                // Check if 'num' is the start of a sequence
                if(!numSet.Contains(num - 1))
                {
                    int currentNum = num;
                    int currentStreak = 1;

                    // Increment by 1 to find the rest of the sequence
                    while(numSet.Contains(currentNum + 1))
                    {
                        currentNum += 1;
                        currentStreak += 1;
                    }
                }

                longestStreak = Math.Max(longestStreak, currentStreak);
            }

            return longestStreak;
        }
    }
}