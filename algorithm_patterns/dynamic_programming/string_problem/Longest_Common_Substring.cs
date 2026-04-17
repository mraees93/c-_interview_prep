using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace algorithm_patterns.dynamic_programming.string_problem
{
    public class Longest_Common_Substring
    {
        public int LongestCommonSubsequence(string text1, string text2)
    {
        int m = text1.Length;
        int n = text2.Length;
        int[,] dp = new int[m + 1, n + 1];  // Create a DP table with extra row and column for the base case

        // Fill the DP table
        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (text1[i - 1] == text2[j - 1])  // If characters match, add 1 to the previous diagonal value
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else  // If not, take the maximum value from either the previous row or column
                {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        // The final answer is stored in dp[m, n], representing the longest common subsequence
        return dp[m, n];
    }
    }
}