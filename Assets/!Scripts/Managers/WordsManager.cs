using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
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

    TextAsset wordsTxt;
    static Node rootNode;
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
    }

    private void InitListeners()
    {
        EventManager.OnSearchWordInTree.AddListener(DoesWordExistsInTree);
        EventManager.OnGetRandomWord.AddListener(GetRandomWord);
    }


    IEnumerator LoadWords()
    {   
        ResourceRequest req = Resources.LoadAsync<TextAsset>("wordlist");

        while (!req.isDone)
        {
            yield return null;
        }
        
        wordsTxt = req.asset as TextAsset;

        string[] words = wordsTxt.text.Split('\n');
        GenerateWordTree(ref words);
        areWordsLoaded = true;
        OnWordsLoaded?.Invoke();
    }


    private static string GetRandomWord()
    {
        return GetRandomWordInternal(ref rootNode);
    }

    private bool DoesWordExistsInTree(string str)
    {
        Node node = rootNode;

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


    private void GenerateWordTree(ref string[] words)
    {
        rootNode = new('\n' , false);

        int limit = Mathf.Min(Constants.MAX_WORDS_IN_TREE, words.Length);
        for(int i = 0; i < limit; i++)
        {
            string str = words[i];
            int index = 0;
            AddWord(ref rootNode, ref index, ref str);
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

        return randomChild.le + GetRandomWordInternal(ref randomChild);
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
