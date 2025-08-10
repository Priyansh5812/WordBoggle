using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
        LetterTile.IsSelectionStarted = false;

        // If game is over then refrain from proceeding further
        if (EventManager.IsGameOver.Invoke()) 
            return;

        // If no tiles were selected, then refrain from proceeding further
        if (tiles == null || tiles.Count == 0)
            return;

        string str = string.Empty;

        foreach (var i in tiles)
        {
            i.IsSelected = false;
            str += i.GetText();

        }

        // Checking whether the collected sequence contains any blocked tile. If any then terminate the function instantly
        foreach (var i in tiles)
        {
            if (i.IsBlocked)
            {
                tiles.Clear();
                return;
            }
        }

        Debug.Log("Word Selected : " + str);

        switch (EventManager.OnValidateWord.Invoke(str))
        {
            case WordValidationType.VALID:
                OnValidWord(tiles);
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

        tiles.Clear();
    }

    private void FreeBlockedNeighbourTiles(LetterTile tile)
    {
        // Get the Blocked Neighbours
        List<LetterTile> blockedTiles = EventManager.GetBlockedNeighbours.Invoke(tile);

        // Unblocked them
        foreach (var b in blockedTiles)
        {
            b.IsBlocked = false;
        }

        // Release the resource for reuse
        CollectionPool<List<LetterTile>, LetterTile>.Release(blockedTiles);
    }

    private void OnValidWord(List<LetterTile> tiles)
    {
        foreach (var i in tiles)
        {
            // Free Blocked Neighbour tiles (if any)
            FreeBlockedNeighbourTiles(i);
        }

        EventManager.OnValidWordSelected.Invoke(tiles);
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


