using System;
using System.Collections.Generic;
// This lets Program.cs find your HashMapBucket class
using algorithm_patterns.Hash_Map_Frequency; 

class Program
{
    static void Main(string[] args)
    {
        // 1. Create an instance of your class
        HashMapBucket solver = new HashMapBucket();

        // 2. Set up the example input array
        string[] input = { "eat", "tea", "tan", "ate", "nat", "bat" };

        // 3. Call your method and store the result
        IList<IList<string>> result = solver.GroupAnagrams(input);

        // 4. Print the result to the screen so you can see it
        Console.WriteLine("--- Anagram Groups ---");
        foreach (var group in result)
        {
            Console.WriteLine("[" + string.Join(", ", group) + "]");
        }
    }
}
