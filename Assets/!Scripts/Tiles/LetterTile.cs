using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class LetterTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    [SerializeField] TextMeshProUGUI m_text;
    [SerializeField] List<Image> m_dots;

    [Header("Grid-Related")]
    [SerializeField] Vector2 m_index;


    [Header("Internals")]
    public bool IsBonus
    {
        get;
        set;
    }
    public bool IsBlocked
    {
        get;
        set;
    }

    private bool m_isSelected = false;
    public bool IsSelected
    {
        get => m_isSelected;
        set
        {   

            OnSelection(value);
            m_isSelected = value;
        }
    }

    public static bool IsSelectionStarted = false;
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

    private void OnSelection(bool value)
    { 
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsSelected || !IsSelectionStarted)
            return;

        EventManager.OnSelect?.Invoke(this);
        IsSelected = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsSelected)
            return;

        EventManager.OnSelectionStarted?.Invoke(this);
        IsSelected = true;
        IsSelectionStarted = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventManager.OnSelectionEnded.Invoke();
    }
}
