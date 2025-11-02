using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using WordBoggle;
using static Unity.Burst.Intrinsics.X86;
public class BoggleHandler : MonoBehaviour
{
    [SerializeField] GridHandler gridHandler;
    private List<LetterTile> aux = null;
    private Queue<LetterTile> selectedTiles = null;
    LetterTile lastSelectedTile = null;
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
        if (selectedTiles == null)
            selectedTiles = new();

        selectedTiles.Enqueue(tile);
        lastSelectedTile = tile;
        Debug.Log("Added :" + tile.GetText());
    }

    private bool OnSelectLogic(LetterTile tile)
    {
        if (selectedTiles.Count == 0)
        { 
            Debug.LogError("Start Selection Logic did not encountered a tile as a Starting tile. Investigate !!!");
            return false;
        }

        List<LetterTile> neighbours = gridHandler.GetNeighbours(lastSelectedTile.GetTileIndex());
        bool res = neighbours.Contains(tile);
        if (res)
        { 
            selectedTiles.Enqueue(tile);
            lastSelectedTile = tile;
        }

        CollectionPool<List<LetterTile> , LetterTile>.Release(neighbours);   
        return res;
    }

    private void OnEndSelectionLogic()
    {   
        LetterTile.IsSelectionStarted = false;

        // If game is over then refrain from proceeding further
        if (EventManager.IsGameOver.Invoke()) 
            return;

        // If no tiles were selected, then refrain from proceeding further
        if (selectedTiles == null || selectedTiles.Count == 0)
            return;

        string str = string.Empty;
        aux ??= CollectionPool<List<LetterTile>,LetterTile>.Get();
        while (selectedTiles.Count > 0)
        {   
            LetterTile tile = selectedTiles.Dequeue();
            aux.Add(tile);
            tile.IsSelected = false;
            str += tile.GetText();
        }

        // Checking whether the collected sequence contains any blocked tile. If any then terminate the function instantly
        foreach (var i in aux)
        {
            if (i.IsBlocked)
            {
                foreach (var j in aux)
                {
                    j.VibrateOnWrong();
                }
                aux.Clear();
                return;
            }
        }

        switch (EventManager.OnValidateWord.Invoke(str))
        {
            case WordValidationType.VALID:
                OnValidWord(aux);
                break;
            case WordValidationType.INVALID:

                // Shake the wrong ones
                foreach (var j in aux)
                {
                    j.VibrateOnWrong();
                }

                OnInvalidWord();
                break;
            case WordValidationType.EXISTING:
                OnExistingWord(str);
                break;
            default:
                Debug.LogError("Unknown validation verdict");
                break;
        }

        selectedTiles.Clear();
        aux.Clear();
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

    private void OnExistingWord(string str)
    {
        EventManager.OnExistingWordSelected.Invoke(str);
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
        selectedTiles?.Clear();
        if (aux != null)
            CollectionPool<List<LetterTile>, LetterTile>.Release(aux);
    }
}


