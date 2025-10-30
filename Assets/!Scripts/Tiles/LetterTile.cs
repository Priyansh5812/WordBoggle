using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WordBoggle;
using DG.Tweening;
using System.Collections;

public class LetterTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    [SerializeField] TextMeshProUGUI m_Text;
    [SerializeField] Image m_MainImage;
    [SerializeField] Image m_BonusImage;
    [SerializeField] Image m_BlockImage;

    [Header("Grid-Related")]
    [SerializeField] Vector2 m_Index;
    [SerializeField, Range(0f, 1f)] float m_SelectedTileScale;

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
            OnSelected(value);
        }
    }

    public static bool IsSelectionStarted = false;

    private Color mainColor;
    

    public void SetGridIndex(in Vector2 GridIndex)
    { 
        m_Index = GridIndex;
    }

    public void SetLetter(string le)
    {   
        m_Text?.SetText(le);
    }


    private void OnSelected(bool value)
    {
        
        if (DOTween.IsTweening(m_MainImage))
        {
            DOTween.Kill(m_MainImage);
        }

        m_MainImage.rectTransform.DOScale(Vector3.one * (value ? m_SelectedTileScale : 1.0f), 0.25f).SetEase(Ease.OutQuad).SetId(m_MainImage);
        m_MainImage.color = value ? Color.white : mainColor;
    }


    public string GetText() => m_Text.text;
    public Vector2 GetTileIndex() => m_Index;

    private void SetAsBonusTile(bool value)
    {
        m_IsBonus = value;
        m_BonusImage.gameObject.SetActive(value);
    }

    private void SetAsBlockedTile(bool value)
    {
        m_IsBlocked = value;
        m_BlockImage.gameObject.SetActive(value);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsSelected || !IsSelectionStarted)
            return;

        bool? value = EventManager.OnSelect?.Invoke(this);
        
        if(value.HasValue && value.Value)
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

    public void OnMainColorUpdated(Color newColor , float transitionDelay)
    {
        //mainColor = newColor;
        StartCoroutine(UpdateColor(newColor , transitionDelay));
    }

    IEnumerator UpdateColor(Color newColor , float delay)
    {
        float t = delay;
        Color currColor = m_MainImage.color;
        while (t > 0)
        {            
            t-= Time.deltaTime;

            mainColor = Color.Lerp(currColor, newColor, 1 - t);

            if(!IsSelected)
                m_MainImage.color = Color.Lerp(currColor, newColor, 1 - t);

            yield return null;
        }

        if(!IsSelected)
            m_MainImage.color = newColor;
        mainColor = newColor;


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
