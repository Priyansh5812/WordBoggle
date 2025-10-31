using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using WordBoggle;

public class WordsManager : MonoBehaviour
{
    private static WordsManager m_instance;

    public static UnityEvent OnWordsLoaded
    {
        get;
    } = new();

    public static bool areWordsLoaded = false;
    public static bool areCheckerWordsLoaded = false;
    public static event System.Action onWordsLoaded;
    TextAsset wordsTxt;
    static Node checkerRootNode;
    static Node wordsRootNode;
    private void Start()
    {
        if (m_instance == null)
        { 
            m_instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitListeners();
        }
        else
            Destroy(this.gameObject);

        StartCoroutine(LoadWords());
        StartCoroutine(LoadCheckerWords());
        StartCoroutine(MonitorWordsLoadCallback());
    }

    IEnumerator MonitorWordsLoadCallback()
    {
        while (!areWordsLoaded || !areCheckerWordsLoaded)
        { 
            yield return null;
        }

        onWordsLoaded?.Invoke();

    }

    private void InitListeners()
    {
        EventManager.OnSearchWordInTree.AddListener(DoesWordExistsInTree);
        EventManager.OnGetRandomWord.AddListener(GetRandomWord);
    }


    IEnumerator LoadWords()
    {   
        ResourceRequest req = Resources.LoadAsync<TextAsset>("easyWords");

        while (!req.isDone)
        {
            yield return null;
        }
        
        wordsTxt = req.asset as TextAsset;

        string[] words = wordsTxt.text.Split('\n');
        GenerateWordTree(ref wordsRootNode , ref words);
        areWordsLoaded = true;
    }

    IEnumerator LoadCheckerWords()
    {   
        ResourceRequest req = Resources.LoadAsync<TextAsset>("wordlist");

        while (!req.isDone)
        {
            yield return null;
        }
        
        wordsTxt = req.asset as TextAsset;

        string[] words = wordsTxt.text.Split('\n');
        GenerateWordTree(ref checkerRootNode , ref words);
        areCheckerWordsLoaded = true;
    }


    private static string GetRandomWord()
    {   
        string str = GetRandomWordInternal(ref wordsRootNode); ;
        Debug.Log("Got Word " + str);
        return str;
    }

    private bool DoesWordExistsInTree(string str)
    {
        Node node = checkerRootNode;

        for(int i = 0; i < str.Length; i++)
        {
            if (node.children.ContainsKey(str[i]))
            {
                node = node.children[str[i]];

                if (i == str.Length - 1)
                {
                    return node.isValidWord;
                }
            }
            else 
            {
                return false;
            }

        }

        return false;
    }


    private void GenerateWordTree(ref Node RootNode , ref string[] words)
    {
        RootNode = new('\n' , false);

        int limit = Mathf.Min(Constants.MAX_WORDS_IN_TREE, words.Length);
        for(int i = 0; i < limit; i++)
        {
            string str = words[i];
            int index = 0;
            AddWord(ref RootNode, ref index, ref str);
        }
    }


    private void AddWord(ref Node rootNode , ref int i , ref string str)
    {
        if (i == str.Length - 1)
        {
            if (rootNode.children.ContainsKey(str[i]))
            {
                rootNode.children[str[i]].isValidWord = true; 
            }
            else
            {
                rootNode.children.Add(str[i], new Node(str[i], true));
            }
            return;
        }

        Node child = null; 
        if (rootNode.children.ContainsKey(str[i]))
        {
            child = rootNode.children[str[i]];
        }
        else 
        {
            child = new(str[i], false);
            rootNode.children.Add(str[i], child);
        }
        i++;
        AddWord(ref child, ref i, ref str);
    }

    private static string GetRandomWordInternal(ref Node rootNode)
    {
        if (rootNode.isValidWord)
            return string.Empty;

        // Getting the random Child from the current node
        Node randomChild = null;
        int i = Random.Range(0, rootNode.children.Count);
        foreach (var c in rootNode.children)
        {
            if (i == 0)
            {
                randomChild = c.Value;
                break;
            }
            i--;
        }

        return (randomChild != null ? (randomChild.le + GetRandomWordInternal(ref randomChild)) : (string.Empty));
    }

    private void DeinitListeners()
    {
        EventManager.OnSearchWordInTree.AddListener(DoesWordExistsInTree);
        EventManager.OnGetRandomWord.AddListener(GetRandomWord);
    }
    private void OnDisable()
    {
        DeinitListeners();
        if (m_instance == this)
        {
            m_instance = null;
        }
    }

}
