using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*
This approach iteratively builds the solution from smaller subproblems, 
avoiding the overhead of recursion and stack memory.
*/

namespace algorithm_patterns.dynamic_programming.bottom_up_approach
{
    public class Fibonacci
    {
        public int FibonacciTab(int n)
        {
            if(n <= 1) return n;

            int[] dp = new int[n + 1];
            dp[0] = 0;
            dp[1] = 1;

            for(int i = 2; i <= n; i++)
            {
                dp[i] = dp[i - 1] + dp[i - 2];
            }

            return dp[n];
        }
    }
}