using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using WordBoggle;

public class GridHandler : MonoBehaviour
{
    readonly LetterTile[,] grid = new LetterTile[4,4];
    readonly List<string> wordsToInsert = new();
    readonly Dictionary<char, List<Vector2>> LetterReg = new();
    readonly List<string> selectedWords = new();

    private void OnEnable()
    {
        InitListeners();
    }

    private void Start()
    {
        InitializeGrid();
        GetRandomWords();
        StartCoroutine(InitializeWordBoggle());
    }

    private void InitListeners()
    {
        EventManager.OnValidateWord.AddListener(ValidateSelectedWord);
    }

    private void InitializeGrid()
    {
        LetterTile[] tiles = this.GetComponentsInChildren<LetterTile>();

        if (tiles.Length > 16)
        {
            Debug.LogError("Grid size exceeds the number of Letter Tiles\n Initialization Failed");
            return;
        }

        int k = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector2 index = Vector2.one;
                index.x = i;
                index.y = j;
                grid[i, j] = tiles[k++];
                grid[i, j]?.SetGridIndex(in index);
            }
        }

        Debug.Log("Grid Initialization Success");
    }

    private void GetRandomWords()
    {
        Debug.Log("Words to Insert : ");
        for (int i = 0; i < Constants.MAX_WORDS_IN_GRID; i++)
        {
            string str = EventManager.OnGetRandomWord.Invoke();
            if (wordsToInsert.Contains(str))
            {
                i--;
                continue;
            }
            wordsToInsert.Add(str);
        }
    }

    private WordValidationType ValidateSelectedWord(string str)
    {
        // Checking whether it exists in selected words list...
        if (selectedWords.Contains(str))
        {
            Debug.Log("Word already selected");
            return WordValidationType.EXISTING;
        }

        // Checking whether it exists in existing word List...
        if (wordsToInsert.Contains(str))
        {
            Debug.Log("New word selected");
            selectedWords.Add(str);
            return WordValidationType.VALID;
        }

        // Checking whether word exists in Tree
        if(EventManager.OnSearchWordInTree.Invoke(str))
        {
            Debug.Log("New word selected from Tree");
            selectedWords.Add(str);
            return WordValidationType.VALID;
        }

        return WordValidationType.INVALID;
    }


    IEnumerator InitializeWordBoggle()
    {

        string str;
        for (int i = 0; i < wordsToInsert.Count; i++)
        {
            str = wordsToInsert[i];
            if (!InitiateWordAddition(ref str, i==0))
            {
                Debug.Log($"Word addition broke on {str}");
                break;
            }
            Debug.Log($"Word Added : {str}");
        }

        //----- Finalize for the remaining Valid tiles (if any) with the Random Letters --------

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (!IsValidTile(grid[i, j]))
                {
                    string rand_le = (Random.Range((int)'a' , ((int)'z' + 1))).ToString();
                    grid[i, j].SetLetter(rand_le);
                    Debug.Log($"Added Random Letter {rand_le} in {grid[i,j]}");
                }
            }
        }

        yield return null;    
    }



    private bool InitiateWordAddition(ref string str, bool isFirst = false)
    {
       // bool res = false;
        List<LetterTile> tiles = CollectionPool<List<LetterTile>, LetterTile>.Get();
        LetterTile tile = null;
        int index = 1;

        //Try Getting the tiles from every similar existing letter
        if (LetterReg.ContainsKey(str[0]))
        {
            foreach (var i in LetterReg[str[0]])
            {
                Debug.Log(i);
                tile = grid[(int)i.x, (int)i.y];
                tiles.Add(tile);
                index = 1;
                if (TryAddWord(ref tiles, ref str, ref tile, ref index, true))
                {
                    UpdateLetterRegistry(ref tiles);
                    CollectionPool<List<LetterTile>, LetterTile>.Release(tiles);
                    Debug.Log($"Word {str} added with existing scan");
                    return true;
                }

                foreach (var t in tiles)
                {
                    if (i == t.GetTileIndex())
                    {
                        continue;
                    }
                    t.SetLetter(string.Empty);
                }

                tiles.Clear();
            }

        }

        

        // FALLBACK : Try additing the word from every valid tile
        return TryAddWordWithEveryValidTile(ref str, ref tiles, isFirst);
    }

    /// <summary>
    /// Function for Updating the Letter Registry (for faster lookups of existing Letters)
    /// </summary>
    /// <param name="tiles">Finalized Letter Tiles</param>
    private void UpdateLetterRegistry(ref List<LetterTile> tiles)
    {
        foreach (var i in tiles)
        {   
            // If slot for a particular Letter does not exist. then create it
            if (!LetterReg.ContainsKey(i.GetText()[0]))
            {
                LetterReg[i.GetText()[0]] = new();
            }

            // Add the Index to it
            LetterReg[i.GetText()[0]].Add(i.GetTileIndex());
        }
    }

    private bool TryAddWordWithEveryValidTile(ref string str, ref List<LetterTile> tiles, bool isFirst = false)
    {   
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {

                // For every valid tile...
                if (IsValidTile(grid[i, j]))
                {   
                    // Add starting letter to it...
                    LetterTile tile = grid[i, j];
                    tiles.Clear();
                    tiles.Add(tile);
                    tile.SetLetter(str.Substring(0, 1));
                    int index = 1;

                    // Continue the letter addition with rest of the words 
                    if (TryAddWord(ref tiles, ref str, ref tile, ref index, false, isFirst))
                    {
                        UpdateLetterRegistry(ref tiles);
                        CollectionPool<List<LetterTile>, LetterTile>.Release(tiles);
                        return true;
                    }

                    foreach (var t in tiles)
                    {
                        t.SetLetter(string.Empty);
                    }


                    tile = grid[i, j];
                    tiles.Clear();
                    tiles.Add(tile);
                    tile.SetLetter(str.Substring(0, 1));
                    index = 1;

                    if (i == 3 && j == 3)
                    { 
                        Debug.Log("Started for true");
                        
                    }

                    // Continue the letter addition with rest of the words 
                    if (TryAddWord(ref tiles, ref str, ref tile, ref index, true, isFirst))
                    {
                        UpdateLetterRegistry(ref tiles);
                        CollectionPool<List<LetterTile>, LetterTile>.Release(tiles);
                        return true;
                    }

                    if (i == 3 && j == 3)
                    {
                        Debug.Log("Output:");
                        foreach (var t in tiles)
                        {
                            Debug.Log(t.GetTileIndex()+ " " + t.GetText());
                        }
                    }

                    // Remove only those letters which were newly added
                    foreach (var t in tiles)
                    {
                        if (LetterReg.ContainsKey(t.GetText()[0]))
                        {
                            if (LetterReg[t.GetText()[0]].Contains(t.GetTileIndex()))
                            {
                                continue;
                            }
                        }
                        t.SetLetter(string.Empty);
                    }

                    tiles.Clear();
                }



                // If call was not successful, then reset the word
                foreach (var t in tiles)
                {
                    t.SetLetter(string.Empty); 
                }
            }
        }
        // If addition of every tile was unsuccessful, then release that memory
        CollectionPool<List<LetterTile>, LetterTile>.Release(tiles);

        return false;
    }

    private bool TryAddWord(ref List<LetterTile> tiles, ref string str, ref LetterTile startTile, ref int i, bool considerExisting = false, bool isFirst = false)
    {

        List<LetterTile> neighbours = null;
        LetterTile reqTile = null;

        if (considerExisting) // Block when we want to consider tiles with letter existing
        {   
            // Get the neighbours
            neighbours = GetNeighbours(startTile.GetTileIndex());

            // CORNER CASE : words like "fgf" can redirect again to one of the accquired tiles in the past calls. Therefore removing them from the list of neighbours
            foreach (var lastTiles in tiles)
            {
                neighbours.Remove(lastTiles);
            }

            // Among every neighbour of the current tile...
            foreach (var n in neighbours)
            {   
                // if we have similar Letter already then accquire it.
                if (str.Substring(i, 1) == n.GetText())
                {
                    reqTile = n;
                    tiles.Add(reqTile);

                    CollectionPool<List<LetterTile>, LetterTile>.Release(neighbours);

                    if (i == str.Length - 1)
                    {
                        return true;
                    }
                    else 
                    {
                        // Continue the call if word addition is not finished
                        i++;

                        return TryAddWord(ref tiles, ref str, ref reqTile, ref i, considerExisting, isFirst);
                    }
                    
                }

            }

            CollectionPool<List<LetterTile>, LetterTile>.Release(neighbours);
        }


        // Getting the valid Neighbours
        neighbours = GetValidNeighbours(startTile.GetTileIndex());

        // If no valid neighbours present, return false as operation was unsuccessful
        if (neighbours.Count == 0)
        {
            CollectionPool<List<LetterTile>, LetterTile>.Release(neighbours);
            return false;
        }

        if (isFirst)
        {
            reqTile = neighbours[Random.Range(0, neighbours.Count - 1)];
        }
        else 
        {
            // Getting the Tile with Maximum neighbours
            int lastTileNeighbours = GetValidNeighbourCount(neighbours[0].GetTileIndex());
            reqTile = neighbours[0];
            foreach (var n in neighbours)
            {
                int curr = GetValidNeighbourCount(n.GetTileIndex());

                if (lastTileNeighbours < curr)
                {
                    reqTile = n;
                    lastTileNeighbours = curr;
                }
            }
        }

        // Add that tile
        tiles.Add(reqTile);

        // Update the required tile
        reqTile.SetLetter(str.Substring(i, 1));

        // Release the resource
        CollectionPool<List<LetterTile>, LetterTile>.Release(neighbours);

        if (i == str.Length - 1)
        {
            return true;
        }
        else
        {   
            // Continue the call if word addition is not finished
            i++;
            return TryAddWord(ref tiles, ref str, ref reqTile, ref i, considerExisting, isFirst);
        }
        
    }



    #region Helper Methods

    readonly Vector2[] checkDirections =
{
        new Vector2(-1,0), //up // Vector2.left
        new Vector2(1,0), // down // Vector2.right
        new Vector2(0,1), // right // Vector2.up
        new Vector2(0,-1), // left // Vector2.down
        new Vector2(-1,1), // top-right // Vector2.left + Vector2.up
        new Vector2(-1,-1), // top-left // Vector2.left + Vector2.down
        new Vector2(1,-1), // bottom-left // Vector2.right + Vector2.down
        new Vector2(1,1) // bottom-right // Vector2.right + Vector2.up
    };

    private List<LetterTile> GetNeighbours(Vector2 cell)
    {
        List<LetterTile> tiles = CollectionPool<List<LetterTile>, LetterTile>.Get();

        foreach (var dir in checkDirections)
        {
            Vector2 vec = dir + cell;
            if(IsValidCell(ref vec))
            {
                tiles.Add(grid[(int)vec.x, (int)vec.y]);
            }
        }
        return tiles;
    }

    private List<LetterTile> GetValidNeighbours(Vector2 cell)
    {
        List<LetterTile> tiles = GetNeighbours(cell);

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].GetText() != string.Empty)
            {
                tiles.RemoveAt(i);
                i--;
                continue;
            }
        }
        return tiles;
    }

    private int GetValidNeighbourCount(Vector2 cell)
    {
        List<LetterTile> tiles = GetValidNeighbours(cell);
        int count = tiles.Count;
        CollectionPool<List<LetterTile>, LetterTile>.Release(tiles);
        return count;
    }


    private bool IsValidTile(LetterTile tile)
    {
        return string.IsNullOrEmpty(tile.GetText());
    }


    private bool IsValidCell(ref Vector2 cell)
    {   
        return (cell.x >= 0 && cell.x < 4) && (cell.y >= 0 && cell.y < 4);
    }

    #endregion

    private void DeinitListeners()
    {
        EventManager.OnValidateWord.RemoveListener(ValidateSelectedWord);
    }

    private void OnDisable()
    {
        DeinitListeners();
    }

}
