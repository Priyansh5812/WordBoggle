using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using WordBoggle;
public class BoggleHandler : MonoBehaviour
{
    private List<LetterTile> tiles = null;

    private void OnEnable()
    {
        InitListeners();   
    }

    private void InitListeners()
    {
        EventManager.OnSelectionStarted.AddListener(StartSelectionLogic);
        EventManager.OnSelect.AddListener(OnSelectLogic);
        EventManager.OnSelectionEnded.AddListener(OnEndSelectionLogic);
    }

    private void StartSelectionLogic(LetterTile tile)
    {
        if(tiles == null)
            tiles = CollectionPool<List<LetterTile> , LetterTile>.Get();

        Debug.Log(tile.gameObject.name);
        tiles.Add(tile);
    }

    private void OnSelectLogic(LetterTile tile)
    {
        Debug.Log(tile.gameObject.name);
        tiles.Add(tile);
    }

    private void OnEndSelectionLogic()
    {
        Debug.Log("Ended");
        LetterTile.IsSelectionStarted = false;

        string str = string.Empty;

        foreach (var i in tiles)
        {
            i.IsSelected = false;
            str += i.GetText();
        }

        tiles.Clear();
        Debug.Log("Word Selected : " + str);

        switch (EventManager.OnValidateWord.Invoke(str))
        {
            case WordValidationType.VALID:
                OnValidWord();
                break;
            case WordValidationType.INVALID:
                OnInvalidWord();
                break;
            case WordValidationType.EXISTING:
                OnExistingWord();
                break;
            default:
                Debug.LogError("Unknown validation verdict");
                break;
        }
    }

    private void OnValidWord()
    {   
        EventManager.OnValidWordSelected.Invoke();
    }

    private void OnExistingWord()
    {
        EventManager.OnExistingWordSelected.Invoke();
    }

    private void OnInvalidWord()
    {
        EventManager.OnInvalidWordSelected.Invoke();
    }

    private void DeinitListeners()
    {
        EventManager.OnSelectionStarted.RemoveListener(StartSelectionLogic);
        EventManager.OnSelect.RemoveListener(OnSelectLogic);
        EventManager.OnSelectionEnded.RemoveListener(OnEndSelectionLogic);
    }


    private void OnDisable()
    {
        DeinitListeners();

        if (tiles != null)
        {
            CollectionPool<List<LetterTile>, LetterTile>.Release(tiles);
        }

    }
}


