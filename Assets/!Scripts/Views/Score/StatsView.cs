using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using WordBoggle;
using System.Collections;
using Cysharp.Threading.Tasks;

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
    [SerializeField] CanvasGroup cg_Pause;
    [SerializeField] CanvasGroup cg_GameOver;

    [Header("Past-Words Related")]
    [SerializeField] TextMeshProUGUI m_WordPrefab;
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

    //Others
    StatsController m_Controller = null;
    Coroutine m_Routine = null;
    WaitForSeconds wordStatusDelay;

    void OnEnable()
    {
        m_Controller ??= new(this,m_gameConfig);
        InitListeners();
    }

    private void Start()
    {
        transitionView.StartTransition(() =>
        {
            this.StartCoroutine(m_Controller.TimerRoutine());
        });
        //StartCoroutine(m_Controller.TimerRoutine());
        wordStatusDelay = new(WordStatusDisplayDuration);
    }


    private void InitListeners()
    {
        EventManager.OnValidWordSelected.AddListener(m_Controller.ProcessValidWord);
        EventManager.IsGameOver.AddListener(m_Controller.GetIsGameOver);
        EventManager.OnValidWordSelected.AddListener(UpdateValidWordStatus);
        EventManager.OnExistingWordSelected.AddListener(UpdateExistingWordStatus);
        EventManager.OnInvalidWordSelected.AddListener(UpdateInvalidWordStatus);

        btn_Pause.onClick.AddListener(TriggerGamePause);
        btn_Resume.onClick.AddListener(ResumeGame);
        btn_Quit.onClick.AddListener(QuitGame);
        btn_Quit_GM.onClick.AddListener(QuitGame);
        btn_Restart.onClick.AddListener(RestartGame);
    }

    #region UI Updation

    public void UpdateTimerUI(int mins , int secs)
    {
        m_TimerText?.SetText($"{(mins < 10 ? ("0" + mins.ToString()) : (mins.ToString()))} : {(secs < 10 ? ("0" + secs.ToString()) : (secs.ToString()))}");
    }

    public void UpdateScoreUI(int score)
    {
        m_ScoreText?.SetText(score.ToString());
    }

    public void UpdateExistingWordsUI(string word)
    {
        TextMeshProUGUI wordText = Instantiate(m_WordPrefab, m_ExistingWordParent);
        wordText?.SetText(word);
    }

    private void ResetExistingWordsUI()
    {
        TextMeshProUGUI[] existingWords = m_ExistingWordParent.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var i in existingWords)
        {
            Destroy(i.gameObject);
        }
    }

    private void TriggerGamePause()
    {
        if (m_Controller.IsGameOver)
            return;

        cg_Pause.alpha = 1.0f;
        cg_Pause.interactable = cg_Pause.blocksRaycasts = true;
        m_Controller.IsGamePaused = true;
    }

    private void ResumeGame()
    {
        cg_Pause.alpha = 0.0f;
        cg_Pause.interactable = cg_Pause.blocksRaycasts = false;
        m_Controller.IsGamePaused = false;
    }

    private void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void TriggerGameOver((int,int,int) data)
    {
        m_ScoreText_GM?.SetText($"Total Score : {data.Item1}");
        m_BonusText_GM?.SetText($"Total Bonus : {data.Item2}");
        m_TotalWordsText_GM?.SetText($"Total Words Found : {data.Item3}");


        cg_GameOver.alpha = 1.0f;
        cg_GameOver.interactable = cg_GameOver.blocksRaycasts = true;
    }

    private void RestartGame()
    {
        cg_GameOver.alpha = 0.0f;
        cg_GameOver.interactable = cg_GameOver.blocksRaycasts = false;

        //-----------------------------------------------
        
        m_Controller.ResetStats();

        //-----------------------------------------------

        ResetExistingWordsUI();

        StartCoroutine(m_Controller.TimerRoutine());

        //-----------------------------------------------
        EventManager.OnGameRestart.Invoke();

        
    }

    private void UpdateValidWordStatus(List<LetterTile> tiles)
    {
        if (m_Routine != null)
            StopCoroutine(m_Routine);

        m_Routine = StartCoroutine(UpdateWordStatusForValid());
    }

    private void UpdateExistingWordStatus()
    {
        if (m_Routine != null)
            StopCoroutine(m_Routine);

        m_Routine = StartCoroutine(UpdateWordStatusForExisting());
    }

    private void UpdateInvalidWordStatus()
    {
        if (m_Routine != null)
            StopCoroutine(m_Routine);

        m_Routine = StartCoroutine(UpdateWordStatusForInvalid());
    }



    IEnumerator UpdateWordStatusForValid()
    {
        m_WordStatus?.SetText("New Word Found !");
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(Constants.VALID_COLOR, out color);
        m_WordStatus.color = color;
        yield return wordStatusDelay;
        m_WordStatus.color = Color.clear;
        m_Routine = null;
    }
    IEnumerator UpdateWordStatusForExisting()
    {
        m_WordStatus?.SetText("Word already exists");
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(Constants.EXISTING_COLOR, out color);
        m_WordStatus.color = color;
        yield return wordStatusDelay;
        m_WordStatus.color = Color.clear;
        m_Routine = null;
    }
    IEnumerator UpdateWordStatusForInvalid()
    {
        m_WordStatus?.SetText("Word does not exists");
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(Constants.INVALID_COLOR, out color);
        m_WordStatus.color = color;
        yield return wordStatusDelay;
        m_WordStatus.color = Color.clear;
        m_Routine = null;
    }

    #endregion

    private void DeinitListeners()
    {
        EventManager.OnValidWordSelected.RemoveListener(m_Controller.ProcessValidWord);
        EventManager.IsGameOver.RemoveListener(m_Controller.GetIsGameOver);
        EventManager.OnValidWordSelected.RemoveListener(UpdateValidWordStatus);
        EventManager.OnExistingWordSelected.RemoveListener(UpdateExistingWordStatus);
        EventManager.OnInvalidWordSelected.RemoveListener(UpdateInvalidWordStatus);

        btn_Pause.onClick.RemoveListener(TriggerGamePause);
        btn_Resume.onClick.RemoveListener(ResumeGame);
        btn_Quit.onClick.RemoveListener(QuitGame);
        btn_Restart.onClick.RemoveListener(RestartGame);
        btn_Quit_GM.onClick.RemoveListener(QuitGame);
        
    }

    private void OnDisable()
    {
        DeinitListeners();
    }

    private void OnDestroy()
    {
        m_Controller = null;
    }
}
