using System.Collections.Generic;
using UnityEngine;

namespace WordBoggle
{
    public static class Constants
    {
        public static readonly int MAX_WORDS_IN_TREE = 25000;
        public static readonly int MAX_WORDS_IN_GRID = 5;
        public static readonly int MAX_BONUS_IN_GAME = 3;
        public static readonly int MAX_BLOCKS_IN_GAME = 2;
        public static readonly string INVALID_COLOR = "#E35757";
        public static readonly string EXISTING_COLOR = "#F8E5BD";
        public static readonly string VALID_COLOR = "#B9F693";
        public static readonly string SELECTED_TILE_COLOR = "#B3FFAA";
        public static readonly string UNSELECTED_TILE_COLOR = "#FFFFFF";
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


    public enum WordValidationType
    {
        VALID,
        EXISTING,
        INVALID
    }
}


