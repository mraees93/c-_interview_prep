using System;
using System.Collections.Generic;
// This lets Program.cs find your HashMapBucket class
using algorithm_patterns.Hash_Map_Frequency;
using algorithm_patterns.two_pointers;


class Program
{
    static void Main(string[] args)
    {
        // // 1. Create an instance of your class
        // HashMapBucket solver = new HashMapBucket();

        // // 2. Set up the example input array
        // string[] input = { "eat", "tea", "tan", "ate", "nat", "bat" };

        // // 3. Call your method and store the result
        // IList<IList<string>> result = solver.GroupAnagrams(input);

        // // 4. Print the result to the screen so you can see it
        // Console.WriteLine("--- Anagram Groups ---");
        // foreach (var group in result)
        // {
        //     Console.WriteLine("[" + string.Join(", ", group) + "]");
        // }

        // TwoSum twoSum = new FindPair();
        // int[] arr = {-8, 1, 4, 6, 10, 45};
        // int target = 16;

        // bool result = twoSum.FindPair(target, arr);
        // Console.WriteLine(result);

        // Create an instance of your algorithm class
        var solver = new ThreeSumClass();

        // Define a test input array
        int[] testInput = { -1, 0, 1, 2, -1, -4 };

        // Call the method and get the results
        IList<IList<int>> results = solver.ThreeSum(testInput);

        // Print the results to the console
        Console.WriteLine("ThreeSum Results:");
        foreach (var triplet in results)
        {
            Console.WriteLine($"[{string.Join(", ", triplet)}]");
        }
    }
}
