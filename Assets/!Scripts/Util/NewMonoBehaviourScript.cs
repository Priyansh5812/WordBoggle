using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
public class NewMonoBehaviourScript : MonoBehaviour
{

    string[] allwords;
    List<string> words;
    [ContextMenu("Read")]
    public void Read()
    {
        if (File.Exists(Application.persistentDataPath + "/NewWords.txt"))
        {
            string str = File.ReadAllText(Application.persistentDataPath + "/NewWords.txt");
            allwords = str.Split(" ");
        }
        else
        { 
            Debug.Log("File Not Exists");
        }
    }

    [ContextMenu("Write")]
    public void Write()
    {
        if (!File.Exists(Application.persistentDataPath + "/NewWords.txt"))
        {
            File.Create(Application.persistentDataPath + "/NewWords.txt");
            
        }

        if (File.Exists(Application.persistentDataPath + "/NewWords.txt"))
        {
            //File.WriteAllLines(Application.persistentDataPath + "/words.txt", allwords);
            words = File.ReadLines(Application.persistentDataPath + "/NewWords.txt").ToList();
            File.WriteAllText(Application.persistentDataPath + "/NewWords.txt", "");
            for (int i = 0; i < words.Count; i++)
            {
                if (words[i].Length < 3)
                    words.RemoveAt(i--);
            }

            File.WriteAllLines(Application.persistentDataPath + "/NewWords.txt", words);

            


        }
    }
    
}
