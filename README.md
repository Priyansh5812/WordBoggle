# Word Boggle Assignment Overview

## Word Hashing
Since we have 55681 words to work with, I decided to go with a Tree. This is because of the following reasons:
1. Searching a word in a Tree has less complexity ```O(word.length)``` rather than ```O(totalWords)``` in an Array/List.
2. Trees are highly versatile when it comes to their customization. (For Example : Adding a new word will reuse the existing nodes)

The structure of a Tree has a following format:

```c#
    public class Node
    {
        public char le; // letter
        public bool isValidWord = false; // Is current Word valid
        public readonly Dictionary<char , Node> children = new(); // reference to children nodes
    }
```

### Addons :
Since we have words divided among 6 types of length ```(3,4,5,6,7,8)```. In order to introduce difficulty and more control we can create trees based upon the length and traverse them based on the probability of occuring a certain length of word.
For example : 50% Prob of getting a 6 letter word, Therefore we can use algorithm of ```Weight Based Probability Selection``` which will yield number 6 which 50-50 chance, Proceeding with it, we can retrieve a word from a tree which only contains 6 letter words.


