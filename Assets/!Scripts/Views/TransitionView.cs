using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TransitionView : MonoBehaviour
{
    [SerializeField] Image circleMask;
    [SerializeField] InverseMask blackOverlay;
    [SerializeField, Range(0f, 2f)] float transitionDuration = 1.25f;
    [SerializeField, Range(0f, 1f)] float StartDelay = 0f;
    [SerializeField] Ease easeType = Ease.OutQuad;
    private RectTransform canvasRect;
    
    private void OnEnable()
    {
        canvasRect = circleMask.canvas.transform as RectTransform;
        blackOverlay.SetMaterialDirty();
    }

    private void Start()
    {
        PrepareStartup();
    }

    private void PrepareStartup()
    {
        canvasRect ??= circleMask.canvas.transform as RectTransform;
        circleMask.rectTransform.sizeDelta = Vector2.up * (canvasRect.rect.height + 500.0f);
        
    }

    
    public Tween StartTransition(Action OnComplete = null)
    {
        if (DOTween.IsTweening(this))
            DOTween.Kill(this);

        blackOverlay.SetMaterialDirty();
        Canvas.ForceUpdateCanvases();

        canvasRect ??= circleMask.canvas.transform as RectTransform;
        blackOverlay.rectTransform.sizeDelta = new Vector2(canvasRect.rect.width, canvasRect.rect.height);
        circleMask.rectTransform.sizeDelta = Vector2.up * (canvasRect.rect.height + 500.0f);
        Vector2 targetSizeDelta = Vector2.one * (canvasRect.rect.height + 500.0f);
        return circleMask.rectTransform.DOSizeDelta(targetSizeDelta, transitionDuration).SetDelay(StartDelay).SetId(this).SetEase(easeType).OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }

    [ContextMenu("Start")]
    public void StartIt() => StartTransition(null);

    [ContextMenu("Stop")]
    public void Stop() => EndTransition(null);
    

    public Tween EndTransition(Action OnComplete = null)
    {
        if (DOTween.IsTweening(this))
            DOTween.Kill(this);

        canvasRect ??= circleMask.canvas.transform as RectTransform;
        blackOverlay.rectTransform.sizeDelta = new Vector2(canvasRect.rect.width, canvasRect.rect.height);
        circleMask.rectTransform.sizeDelta = Vector2.one * (canvasRect.rect.height + 500.0f);
        Vector2 targetSizeDelta = Vector2.up * (canvasRect.rect.height + 500.0f);
        return circleMask.rectTransform.DOSizeDelta(targetSizeDelta, transitionDuration).SetId(this).SetDelay(StartDelay).SetEase(easeType).OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }

    private void OnDisable()
    {
        DOTween.KillAll();
    }

}
