using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{   
    [SerializeField] private CanvasGroup cg_main;
    [SerializeField] private Button btn_start;
    [SerializeField] private Button btn_settings;

    [Header("Black Radial Overlay")]
    [SerializeField] TransitionView transitionView;

    [Header("Menu Color Overlay")]
    [SerializeField] Image menuBg;

    [Header("Menu Props")]
    [SerializeField] private Image wordImg;
    [SerializeField] private Image BoggleImg;
    [SerializeField] List<Image> otherProps = new();

    [Header("Complementary Config")]
    [SerializeField] ColorConfig compConfig;

    [Space(10)]
    [SerializeField] private LoadingView loadingView;
    private void OnEnable()
    {   
        // Wait for callback if words are not loaded
        if (!WordsManager.areWordsLoaded)
        {
            cg_main.interactable = false;
            
        }
        else // If words are already loaded then allow interactions from the start
        {
            SetMenuInteractable();
        }

        WordsManager.OnWordsLoaded.AddListener(SetMenuInteractable);
        btn_start?.onClick.AddListener(SetupLoadingGameScene);
    }

    private async void Start()
    {
        InitiateBGColorTweening();
        await PrepareStartup();
        InitiatePropAnimations();
    }

    private async UniTask PrepareStartup()
    {
        foreach (var i in otherProps)
        {
            i.rectTransform.localScale = Vector3.zero;
        }

        Tween t = transitionView.StartTransition();
        await UniTask.WaitUntil(() => !t.IsActive());
    }

    


    private async void InitiatePropAnimations()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(wordImg.rectTransform.DOAnchorPos(Vector3.zero, 0.75f).SetEase(Ease.OutBounce).SetEase(Ease.OutQuad));
        seq.Join(BoggleImg.rectTransform.DOAnchorPos(Vector3.zero, 0.75f).SetDelay(0.5f).SetEase(Ease.OutBounce).SetEase(Ease.OutQuad));
        seq.AppendInterval(0.25f);
        bool t = true;
        foreach (var i in otherProps)
        {   
            Image prop = i;
            if (t)
            { 
                seq.Append(prop.rectTransform.DOScale(Vector3.one*1.35f, 0.75f).SetEase(Ease.OutElastic));
                t = false;
            }
            else
                seq.Join(prop.rectTransform.DOScale(Vector3.one*1.35f, 0.75f).SetEase(Ease.OutElastic));
        }

        foreach (var i in otherProps)
        {
            i.rectTransform.DOAnchorPosY(i.rectTransform.anchoredPosition.y + 10.5f, 0.75f).SetDelay(Random.Range(0f, 0.5f)).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo);
        }


        Sequence seq1 = DOTween.Sequence();
        seq1.SetDelay(1.5f);
        seq1.Append(btn_start.targetGraphic.rectTransform.DOAnchorPos(Vector3.zero, 0.75f).SetEase(Ease.OutBounce));
        seq1.Join(btn_settings.targetGraphic.rectTransform.DOAnchorPos(Vector3.zero, 0.75f).SetEase(Ease.OutBounce));

        await UniTask.WaitUntil(() => !seq1.active);

        btn_start.targetGraphic.rectTransform.DOAnchorPosY(btn_start.targetGraphic.rectTransform.anchoredPosition.y + 2.5f, 0.75f).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo);
        btn_settings.targetGraphic.rectTransform.DOAnchorPosY(btn_settings.targetGraphic.rectTransform.anchoredPosition.y + 2.5f, 0.75f).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo);
    }

    private void InitiateBGColorTweening()
    {
        menuBg.material = new Material(menuBg.material);
        Color randColor = compConfig.GetRandomColor();
        menuBg.material.SetColor("_ColorB", randColor);
        Color color = randColor;
        Color comp = compConfig.GetComplementary(color);
        DOTween.To(() => color, (Color value) => color = value, comp, 5f).SetDelay(1).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).OnUpdate(() => 
        {
            menuBg.material.SetColor("_ColorB", color);
        });
    }



    private void SetMenuInteractable() => cg_main.interactable = true;

    private async void SetupLoadingGameScene()
    {
        cg_main.interactable = false;

        loadingView.InitiateLoading();

        await UniTask.Yield();
        AsyncOperation op = SceneManager.LoadSceneAsync(1);
        op.allowSceneActivation = false;

        await UniTask.WaitUntil(() => op.progress >= 0.9f);

        await UniTask.Delay(500);
        loadingView.StopLoading();
        transitionView.EndTransition(() => { op.allowSceneActivation = true; });
    }


    private void OnDisable()
    {
        WordsManager.OnWordsLoaded.RemoveListener(SetMenuInteractable);
        btn_start?.onClick.RemoveListener(SetupLoadingGameScene);
        DOTween.KillAll();
    }

}
