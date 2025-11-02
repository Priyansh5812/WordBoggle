using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WordBoggle;

public class StatsView : MonoBehaviour
{
    [Header("Stats-Related")]
    [SerializeField] TextMeshProUGUI m_TimerText;
    [SerializeField] TextMeshProUGUI m_ScoreText;
    [SerializeField] TextMeshProUGUI m_ScoreText_GM;
    [SerializeField] TextMeshProUGUI m_BonusText_GM;
    [SerializeField] TextMeshProUGUI m_TotalWordsText_GM;
    [SerializeField] TextMeshProUGUI m_WordStatus;

    [Header("Canvas Group")]
    [SerializeField] CanvasGroup cg_overlay;
    [SerializeField] CanvasGroup cg_Pause;
    [SerializeField] CanvasGroup cg_GameOver;

    [Header("Past-Words Related")]
    [SerializeField] ExistingWord m_WordPrefab;
    [SerializeField] RectTransform m_ExistingWordParent;
    [SerializeField] float WordStatusDisplayDuration;

    [Header("Buttons")]
    [SerializeField] Button btn_Pause;
    [SerializeField] Button btn_Resume;
    [SerializeField] Button btn_Restart;
    [SerializeField] Button btn_Quit;
    [SerializeField] Button btn_Quit_GM;

    [Header("Config")]
    [SerializeField] GameConfig m_gameConfig;

    [Space(10)]
    [SerializeField] TransitionView transitionView;

    [Header("Others")]
    [SerializeField] RectTransform[] bubbles;
    [SerializeField] ScrollRect scrollView;

    //Others
    StatsController m_Controller = null;
    Coroutine m_Routine = null;
    Coroutine existingWordsOperation = null;
    WaitForSeconds wordStatusDelay;
    Queue<string> wordsToUpdate = new();
    Vector3 restAnchoredPosition = Vector3.zero;
    int c = 0;
    Dictionary<string, (ExistingWord , int)> existingWords;
    void OnEnable()
    {
        m_Controller ??= new(this,m_gameConfig);
        existingWords ??= CollectionPool<Dictionary<string, (ExistingWord, int)>, KeyValuePair<string, (ExistingWord, int)>>.Get();
        InitListeners();
        InitiateBubblesAnimation();
    }

    private void Start()
    {
        transitionView.StartTransition(() =>
        {
            this.StartCoroutine(m_Controller.TimerRoutine());
        });
        
        wordStatusDelay = new(WordStatusDisplayDuration);
    }


    private void InitListeners()
    {
        EventManager.OnValidWordSelected.AddListener(m_Controller.ProcessValidWord);
        EventManager.IsGameOver.AddListener(m_Controller.GetIsGameOver);
        EventManager.OnExistingWordSelected.AddListener(UpdateExistingWordStatus);

        btn_Pause.onClick.AddListener(TriggerGamePause);
        btn_Resume.onClick.AddListener(ResumeGame);
        btn_Quit.onClick.AddListener(QuitGame);
        btn_Quit_GM.onClick.AddListener(QuitGame);
        btn_Restart.onClick.AddListener(RestartGame);
    }

    #region UI Updation

    public void UpdateTimerUI(int mins , int secs)
    {
        m_TimerText?.SetText($"{(mins < 10 ? ("0" + mins.ToString()) : (mins.ToString()))}:{(secs < 10 ? ("0" + secs.ToString()) : (secs.ToString()))}");
    }

    public void UpdateScoreUI(int score)
    {
        m_ScoreText?.SetText(score.ToString());
    }

    public void UpdateExistingWordsUI(string word)
    {
        wordsToUpdate.Enqueue(word);
        existingWordsOperation ??= StartCoroutine(MonitorExistingWordsUIOperation());
    }

    IEnumerator MonitorExistingWordsUIOperation()
    {
        while (wordsToUpdate.Count > 0)
        {   
            ExistingWord eWord = Instantiate(m_WordPrefab, m_ExistingWordParent);
            if (DOTween.IsTweening(scrollView))
                DOTween.Kill(scrollView);
            scrollView.DOVerticalNormalizedPos(0f, 0.5f).SetEase(Ease.OutQuad).SetId(scrollView);
            yield return eWord?.ShowWord(wordsToUpdate.Dequeue(), 0.5f);
        }

        existingWordsOperation = null;
    }

    private void ResetExistingWordsUI()
    {
        if (existingWordsOperation != null)
            StopCoroutine(existingWordsOperation);

        wordsToUpdate.Clear();

        ExistingWord[] existingWords = m_ExistingWordParent.GetComponentsInChildren<ExistingWord>();

        foreach (var i in existingWords)
        {
            Destroy(i.gameObject);
        }
    }



    private void TriggerGamePause()
    {
        if (m_Controller.IsGameOver)
            return;

        if (DOTween.IsTweening(cg_overlay))
            DOTween.Kill(cg_overlay);

        m_Controller.IsGamePaused = true;

        cg_overlay.blocksRaycasts = true;
        cg_Pause.interactable = false;
        restAnchoredPosition = (cg_Pause.transform as RectTransform).anchoredPosition;
        cg_overlay.DOFade(1f, 0.5f).SetEase(Ease.OutQuad).SetId(cg_overlay).OnComplete(() => 
        {
            (cg_Pause.transform as RectTransform)?.DOAnchorPos(Vector3.zero, 1f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                cg_Pause.interactable = cg_Pause.blocksRaycasts = true;
                
            });
        });
        
    }

    private void ResumeGame() 
    {
        if (DOTween.IsTweening(cg_overlay))
            DOTween.Kill(cg_overlay);

        cg_Pause.interactable = cg_Pause.blocksRaycasts = false;
        (cg_Pause.transform as RectTransform)?.DOAnchorPos(restAnchoredPosition, 0.75f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            cg_overlay.DOFade(0.0f, 1f).SetEase(Ease.OutQuad).SetId(cg_overlay).OnComplete(() =>
            {
                cg_overlay.blocksRaycasts = false;
                m_Controller.IsGamePaused = false;
            });
        });

    }

    private void QuitGame()
    {
        cg_GameOver.interactable = cg_GameOver.blocksRaycasts = false;
        cg_Pause.interactable = cg_Pause.blocksRaycasts = false;
        transitionView.EndTransition(() => { SceneManager.LoadScene(0);});
    }

    public void TriggerGameOver((int,int,int) data)
    {
        m_ScoreText_GM?.SetText($"Total Score : {data.Item1}");
        m_BonusText_GM?.SetText($"Total Bonus : {data.Item2}");
        m_TotalWordsText_GM?.SetText($"Total Words Found : {data.Item3}");

        cg_overlay.blocksRaycasts = true;
        cg_GameOver.interactable = false;
        restAnchoredPosition = (cg_GameOver.transform as RectTransform).anchoredPosition;

        if (DOTween.IsTweening(cg_overlay))
            DOTween.Kill(cg_overlay);

        cg_overlay.DOFade(1f, 0.5f).SetEase(Ease.OutQuad).SetId(cg_overlay).OnComplete(() =>
        {
            (cg_GameOver.transform as RectTransform)?.DOAnchorPos(Vector3.zero, 1f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                cg_GameOver.interactable = cg_GameOver.blocksRaycasts = true;
            });
        });

    }

    private void RestartGame()
    {
        cg_GameOver.interactable = cg_GameOver.blocksRaycasts = false;
        System.Action action1 = () =>
        {
            cg_overlay.alpha = 0.0f;
            cg_overlay.blocksRaycasts = false;
            //-----------------------------------------------
            (cg_GameOver.transform as RectTransform).anchoredPosition = restAnchoredPosition;
            //-----------------------------------------------
            m_Controller.ResetStats();
            //-----------------------------------------------
            ResetExistingWordsUI();
            //-----------------------------------------------
            EventManager.OnGameRestart.Invoke();
            //-----------------------------------------------
            System.Action action2 = () =>
            {
                StartCoroutine(m_Controller.TimerRoutine());
            };
            transitionView.StartTransition(action2);

        };
        transitionView.EndTransition(action1);
    }

    private void UpdateExistingWordStatus()
    {
        if (m_Routine != null)
            StopCoroutine(m_Routine);

        m_Routine = StartCoroutine(UpdateWordStatusForExisting());
    }


    IEnumerator UpdateWordStatusForExisting()
    {   
        // Need something which tells that word already exists

        m_WordStatus?.SetText("Word already exists");
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(Constants.EXISTING_COLOR, out color);
        m_WordStatus.color = color;
        yield return wordStatusDelay;
        m_WordStatus.color = Color.clear;
        m_Routine = null;
    }

    #endregion




    private void InitiateBubblesAnimation()
    {
        foreach (var i in bubbles)
        {
            float currValue = i.position.y;
            i.DOMoveY(currValue + 0.05f, 5f).SetDelay(Random.Range(1f, 10f)).SetLoops(-1 , LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
    }


    private void DeinitListeners()
    {
        EventManager.OnValidWordSelected.RemoveListener(m_Controller.ProcessValidWord);
        EventManager.IsGameOver.RemoveListener(m_Controller.GetIsGameOver);
        EventManager.OnExistingWordSelected.RemoveListener(UpdateExistingWordStatus);

        btn_Pause.onClick.RemoveListener(TriggerGamePause);
        btn_Resume.onClick.RemoveListener(ResumeGame);
        btn_Quit.onClick.RemoveListener(QuitGame);
        btn_Restart.onClick.RemoveListener(RestartGame);
        btn_Quit_GM.onClick.RemoveListener(QuitGame);
        
    }

    private void OnDisable()
    {
        DeinitListeners();

        CollectionPool<Dictionary<string, (ExistingWord, int)>, KeyValuePair<string, (ExistingWord, int)>>.Release(existingWords);
        existingWords = null;

        if (DOTween.IsTweening(scrollView))
            DOTween.Kill(scrollView);


        DOTween.KillAll();
    }

    private void OnDestroy()
    {
        m_Controller = null;
    }
}
