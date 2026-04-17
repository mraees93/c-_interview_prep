using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
The top-down method, also known as memoization, involves solving the problem recursively while storing the results of subproblems. 
When a subproblem is encountered again, its result is retrieved from memory rather than being recomputed.
*/

namespace algorithm_patterns.dynamic_programming.top_down_approach
{
    public class Fibonacci
    {
        private readonly Dictionary<int, int> memo = new Dictionary<int, int>();

        public int FibonacciMemo(int n)
        {
            if(n <= 1) return n;

            if(!memo.ContainsKey(n))
            {
                memo[n] = FibonacciMemo(n - 1) + FibonacciMemo(n - 2);
                //If the result isn't in our notebook, we calculate it by breaking it down into two smaller sub-problems 
                // n - 1 and n - 2. Once we get that sum, we save it in the dictionary.
            }

            return memo[n];
        }
    }
}