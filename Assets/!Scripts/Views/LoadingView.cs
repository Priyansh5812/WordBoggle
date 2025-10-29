using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(CanvasGroup))]
public class LoadingView : MonoBehaviour
{
    [SerializeField] RectTransform cog1, cog2;
    private CanvasGroup cg_main;
    private HashSet<Tween> loadingTweens;
    private bool isLoading = false;
    private void OnEnable()
    {
        loadingTweens ??= CollectionPool<HashSet<Tween>, Tween>.Get();
        cg_main ??= GetComponent<CanvasGroup>();
        PrepareStartup();
    }

    private void PrepareStartup()
    {
        if (cg_main != null)
            cg_main.alpha = 0.0f;


    }

    public void InitiateLoading()
    {
        if (isLoading)
            return;

        isLoading = true;

        RegisterTween(cg_main.DOFade(1f, 0.5f).SetEase(Ease.OutQuad));
        RegisterTween(cog1.DORotate(Vector3.forward * 180f, 5f).SetLoops(-1, LoopType.Incremental));
        RegisterTween(cog2.DORotate(Vector3.forward * 180f, 5f).SetLoops(-1, LoopType.Incremental));
    }

    public void StopLoading()
    {
        if (!isLoading)
            return;

        isLoading = false;

        foreach (var i in loadingTweens)
        {
            if (i.IsPlaying())
                DOTween.Kill(i);
        }

        loadingTweens.Clear();

        RegisterTween(cg_main.DOFade(0f, 0.5f).SetEase(Ease.OutQuad));
    }


    private void ClearTweens()
    {
        foreach (var i in loadingTweens)
        { 
            if(i.IsPlaying())
                DOTween.Kill(i);
        }

        loadingTweens.Clear();
    }

    private void RegisterTween(Tween t)
    {
        loadingTweens.Add(t);
        t.OnComplete(() =>
        {
            loadingTweens.Remove(t);
        });
    }

    private void OnDisable()
    {
        ClearTweens();
        CollectionPool<HashSet<Tween>, Tween>.Release(loadingTweens);  
    }

}
