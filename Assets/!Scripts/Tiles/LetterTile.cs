using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class LetterTile : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_text;
    [SerializeField] List<Image> m_dots;

    [Header("Grid-Related")]
    [SerializeField] Vector2 m_index;


    [Header("Internals")]
    [SerializeField] int m_score = 0;
    [SerializeField] bool m_isBonus = false;
    [SerializeField] bool isBlocked = false;
    [field: SerializeField]
    bool IsSelected
    {
        get;
        set;
    } = false;

    public void SetGridIndex(in Vector2 GridIndex)
    { 
        m_index = GridIndex;
    }

    public void SetLetter(string le)
    {   
        m_text?.SetText(le);
    }

    public string GetText() => m_text.text;
    public Vector2 GetTileIndex() => m_index;

    
}
