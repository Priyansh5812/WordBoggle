using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
public class BoggleView : MonoBehaviour
{
    private List<LetterTile> tiles = null;



    private void OnEnable()
    {
        InitListeners();   
    }

    private void InitListeners()
    {
        
    }

    private void StartSelectionLogic(LetterTile tile)
    {
        if(tiles == null)
            tiles = CollectionPool<List<LetterTile> , LetterTile>.Get();
        
        tiles.Add(tile);
    }

    private void OnSelectLogic(LetterTile tile)
    { 
        tiles.Add(tile);
    }

    private void OnEndSelectionLogic()
    { 
        
    }
    

    private void DeinitListeners()
    {

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

public class BoggleController
{
    public BoggleController()
    { 
        
    }



}
