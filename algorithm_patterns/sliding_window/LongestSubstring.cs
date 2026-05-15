using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace algorithm_patterns.sliding_window
{
    public class LongestSubstring
    {
        /*
        This implementation uses a Dictionary<char, int> to track the next index of each character, 
        enabling the left pointer to immediately jump past duplicates in \(O(1)\) time.
        */
        public int LengthOfLongestSubstring(string s)
        {
            // Maps a character to the index right after its latest occurrence
            Dictionary<char, int> charMap = new Dictionary<char, int>();
            int maxLen = 0;
            int left = 0;

            for (int right = 0; right < s.Length; right++)
            {
                char currentChar = s[right];

                // If character exists and its recorded index is within the current window
                if (charMap.ContainsKey(currentChar) && charMap[currentChar] > left)
                {
                    left = charMap[currentChar]; // Fast-forward left pointer
                }

                // Calculate current unique substring window length
                maxLen = Math.Max(maxLen, right - left + 1);

                // Store or update the next ideal starting position for this character
                charMap[currentChar] = right + 1;
            }

            return maxLen;
        }

        //faster solution
        /*
        If you are aiming for extreme performance in an interview (minimising memory allocations on the heap), 
        you can replace the Dictionary with a fixed-size int[] array. Since standard string characters fall under 
        the ASCII or extended character sets, a direct index lookup is faster than a hash map lookup.
        */
        public int LengthOfLongestSubstring(string s) {
        // Extended ASCII character array cache initialized to 0
        int[] charPositions = new int[256]; 
        int maxLen = 0;
        int left = 0;

        for (int right = 0; right < s.Length; right++) {
            char currentChar = s[right];

            // Direct index access replaces ContainsKey check
            if (charPositions[currentChar] > left) {
                left = charPositions[currentChar];
            }

            maxLen = Math.Max(maxLen, right - left + 1);
            charPositions[currentChar] = right + 1;
        }

        return maxLen;
    }
    }
}

/*
Example 1:

Input: s = "abcabcbb"
Output: 3
Explanation: The answer is "abc", with the length of 3. Note that "bca" and "cab" are also correct answers.
Example 2:

Input: s = "bbbbb"
Output: 1
Explanation: The answer is "b", with the length of 1.
Example 3:

Input: s = "pwwkew"
Output: 3
Explanation: The answer is "wke", with the length of 3.
Notice that the answer must be a substring, "pwke" is a subsequence and not a substring.
*/