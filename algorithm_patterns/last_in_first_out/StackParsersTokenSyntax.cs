using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
The Prompt: Given a string containing only structural bracket tokens—specifically (, ), {, }, [ and ]—write a validation check that returns a boolean indicating if 
the input string pattern closes in a valid sequence.
Example Input: "()[]{}" ➔ Expected Output: true
Example Input: "(]" ➔ Expected Output: false
Example Input: "([)]" ➔ Expected Output: false
*/

namespace algorithm_patterns.last_in_first_out
{
    public class StackParsersTokenSyntax
    {
        public bool IsValidSyntax(string s)
        {
            // 1. Edge Case: An odd number of characters can never form balanced pairs
            if (string.IsNullOrEmpty(s) || s.Length % 2 != 0) return false;

            var bracketStack = new Stack<char>();

            foreach (char ch in s)
            {
                // Whenever an opening token appears, push its closing match onto the stack
                if (ch == '(') bracketStack.Push(')');
                else if (ch == '{') bracketStack.Push('}');
                else if (ch == '[') bracketStack.Push(']');

                // If it's a closing token, check if the stack is empty (meaning no opening bracket exists)
                // or if the character fails to match the expected top item of the stack layout
                else if (bracketStack.Count == 0 || bracketStack.Pop() != ch)
                {
                    return false;
                }
            }

            // If the stack is completely empty, all opened brackets were successfully closed
            return bracketStack.Count == 0;
        }
    }
}
/*
🧠 The Algorithmic Pattern Used: This challenge uses the Last-In, First-Out (LIFO) Stack Pattern.
The Logic: A raw string parsing array or a hash map cannot easily solve this because bracket syntax relies on nested context. 
The last bracket you open must be the very first bracket you close.
The Mechanism: By pushing expected closing tokens onto a Stack<char> whenever you encounter an opening bracket, you create a 
chronological trail. When you hit a closing bracket, it must match the token popped from the top of the stack.
*/