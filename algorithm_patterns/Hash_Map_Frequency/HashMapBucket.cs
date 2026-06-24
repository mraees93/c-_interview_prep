using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
The Prompt: Given an array of strings, group all anagrams together into sub-lists. An anagram is a word formed by rearranging the 
letters of another word.Example Input: string[] strs = { "eat", "tea", "tan", "ate", "nat", "bat" }
Expected Output: [ ["bat"], ["nat", "tan"], ["ate", "eat", "tea"] ] (The ordering of the buckets does not matter)
*/

namespace algorithm_patterns.Hash_Map_Frequency
{
    public class HashMapBucket
    {
        public IList<IList<string>> GroupAnagrams(string[] strs)
        {
            // 1. Edge Case Check
            if (strs == null || strs.Length == 0) return new List<IList<string>>();

            // Key: The sorted alphabetical "signature", Value: The list of matching original words
            var anagramBuckets = new Dictionary<string, List<string>>();

            foreach (string word in strs)
            {
                // Convert the string to a character array, sort it, and turn it back to a string
                char[] charArray = word.ToCharArray();
                Array.Sort(charArray);
                string sortedKey = new string(charArray);

                // If this signature hasn't been seen yet, instantiate a new bucket list inside the map layout
                if (!anagramBuckets.ContainsKey(sortedKey))
                {
                    anagramBuckets[sortedKey] = new List<string>();
                }

                // Append the original unsorted word to its corresponding signature bucket
                anagramBuckets[sortedKey].Add(word); //************try
            }

            // Cast and return the inner values array collections cleanly as required by the interface
            return anagramBuckets.Values.Cast<IList<string>>().ToList();
        }

    }
}
/*
This challenge uses the Hash Map Frequency Bucket / Categorisation Pattern.
The Logic: To group words that have the exact same letters rearranged, you need a way to generate a uniform "signature key" for 
each word.
The Mechanism: An anagram's defining trait is that when its characters are sorted alphabetically, it produces the exact same 
string (e.g., "eat", "tea", and "ate" all sort perfectly into "aet"). 
We use that sorted string as a unique key in a Dictionary<string, List<string>> to bucket the original words together.
*/