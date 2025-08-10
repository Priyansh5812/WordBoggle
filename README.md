# Word Boggle Assignment Overview

## Word Hashing
Word Hashing is done and managed by ``` WordsManager``` class and remain persistant and non-singleton across the game scenes.
Its accessibility is done by an ```EventManager``` class (More on it in future)

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

I have also added a callback for the "OnWordsLoaded" upon which we can make the interactions enabled or disable an ongoing loading screen. Though it is currently being used for enabling interactions only.


### Addons :
Since we have words divided among 6 types of length ```(3,4,5,6,7,8)```. In order to introduce difficulty and more control we can create trees based upon the length and traverse them based on the probability of occuring a certain length of word.
For example : 50% Prob of getting a 6 letter word, Therefore we can use algorithm of ```Weight Based Probability Selection``` which will yield number 6 with 50-50 chance, Proceeding with it, we can retrieve a word from a tree which only contains 6 letter words.

## Event-Management

I wanted to create this game such that there are no singletons and every class will be responsible for itself only. Therefore I have created a static class which will be containing all the statically defined public events and can be access throughout the game. Upon triggering a specific situation rather than storing references to all the classes require to act on, I am just invoking a specific Action / Func which will provide me the logic execution without any extra reference management. 

This type of architecture follows <b>Observer Design Pattern</b> which I use in almost every project.





