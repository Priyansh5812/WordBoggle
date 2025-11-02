using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExistingWord : MonoBehaviour
{
    [SerializeField] Image highlight;
    [SerializeField] TextMeshProUGUI word;
    [SerializeField] CanvasGroup cg_main;
    CancellationTokenSource cts;
    [SerializeField] Color targetColor;
    private void OnEnable()
    {
        highlight.color = Color.clear;
        cts = new CancellationTokenSource();
    }

    public async UniTask ShowWord(string text , float duration)
    {
        text =text.ToUpper();
        text = text[0] + text.Substring(1, text.Length - 1).ToLower();
        word?.SetText(text);
        RectTransform rectTransform = this.transform as RectTransform;
        rectTransform.localScale = Vector3.zero;
        try
        {
            rectTransform.DOScale(Vector3.one, duration / 2).SetId(this).SetEase(Ease.OutBounce);
            cg_main.DOFade(1f, duration).SetEase(Ease.OutQuad).SetId(this);
            await UniTask.Delay(Mathf.CeilToInt(duration) * 1000, cancellationToken: cts.Token);
        }
        catch
        {   
            return;
        }
    }

    public void Hightlight(System.Action OnComplete = null) 
    {
        highlight.DOColor(targetColor, 1f).SetEase(Ease.OutQuad).SetId(highlight).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }


    private void OnDisable()
    {
        if (DOTween.IsTweening(this))
            DOTween.Kill(this);

        if (DOTween.IsTweening(highlight))
            DOTween.Kill(highlight);

        cts?.Cancel();
        cts?.Dispose();
    }


}
