using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WordBoggle;
using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;

public class LetterTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    [SerializeField] TextMeshProUGUI m_Text;
    [SerializeField] Image m_MainImage;
    [SerializeField] Image m_BonusImage;
    [Header("Grid-Related")]
    [SerializeField] Vector2 m_Index;
    [SerializeField, Range(0f, 1f)] float m_SelectedTileScale;

    [Header("Block-Related")]
    [SerializeField] CanvasGroup m_Blockcg;
    [SerializeField] Animator BlockImageAnimator;
    private static int destruction_hash = Animator.StringToHash("Destruction");
    private static int blocked_hash = Animator.StringToHash("Reset");
    private static int notBlocked_hash = Animator.StringToHash("NotBlocked");
    private static float blockAnimationLength;

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
    
    public void Start()
    {
        InitBlockAnimationData();
    }

    private void InitBlockAnimationData()
    {
        if (blockAnimationLength != 0.0f)
            return;

        RuntimeAnimatorController controller = BlockImageAnimator.runtimeAnimatorController;
        foreach (var clip in controller.animationClips)
        {
            if (Animator.StringToHash(clip.name) == destruction_hash)
            {
                Debug.Log("Length: " + clip.length);
                blockAnimationLength = clip.length;
            }
        }
    }

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
        AnimateTileBlocker(value, () =>
        {
            m_IsBlocked = value;
        });
    }

    private async void AnimateTileBlocker(bool isBlocked, Action  OnComplete = null)
    {
        if (isBlocked)
        {
            BlockImageAnimator.Play(blocked_hash);  
        }
        else
        {
            BlockImageAnimator.Play(destruction_hash);
            await UniTask.Delay(Mathf.CeilToInt(blockAnimationLength) * 1000);
        }

        OnComplete?.Invoke();
    }

    private void ResetTileBlocker()
    {
        BlockImageAnimator.Play(notBlocked_hash);
        m_IsBlocked = false;
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
        ResetTileBlocker();
        this.IsSelected = false;
        IsSelectionStarted = false;
    }
}
