using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class LetterTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    [SerializeField] TextMeshProUGUI m_text;
    [SerializeField] Image BonusImage;
    [SerializeField] Image BlockImage;

    [Header("Grid-Related")]
    [SerializeField] Vector2 m_index;


    [Header("Internals")]
    private bool m_IsBonus = false;
    private bool m_IsBlocked = false;
    public bool IsBonus
    {
        get => m_IsBonus;
        set
        {
            SetAsBonusTile(value);
        }
    }
    public bool IsBlocked
    {
        get => m_IsBlocked;
        set
        { 
            SetAsBlockedTile(value);
        }
    }

    private bool m_isSelected = false;
    public bool IsSelected
    {
        get => m_isSelected;
        set
        {   
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

    private void SetAsBonusTile(bool value)
    {
        m_IsBonus = value;
        BonusImage.gameObject.SetActive(value);
    }

    private void SetAsBlockedTile(bool value)
    {
        m_IsBlocked = value;
        BlockImage.gameObject.SetActive(value);
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


    public void ResetTile()
    {
        this.SetLetter(string.Empty);
        this.IsBonus = false;
        this.IsBlocked = false;
        this.IsSelected = false;
        IsSelectionStarted = false;
    }
}
