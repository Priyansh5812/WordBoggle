using System.Collections.Generic;
using UnityEngine;

namespace WordBoggle
{
    public static class Constants
    {
        public static readonly int MAX_WORDS_IN_TREE = 25000;
        public static readonly int MAX_WORDS_IN_GRID = 5;
    }

    public class Node
    {
        public Node(char le, bool isValidWord)
        { 
            this.le = le;
            this.isValidWord = isValidWord;
        }

        public char le; // letter
        public int subTreeWordCount = 1; // Total Possible words inside the subtree from current node
        public bool isValidWord = false; // Is current Word valid
        public readonly Dictionary<char , Node> children = new(); // reference to children nodes
    }
}


