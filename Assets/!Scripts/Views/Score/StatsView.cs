using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class StatsView : MonoBehaviour
{
    [Header("Stats-Related")]
    [SerializeField] TextMeshProUGUI m_TimerText;
    [SerializeField] TextMeshProUGUI m_ScoreText;
    [SerializeField] TextMeshProUGUI m_ScoreText_GM;
    [SerializeField] TextMeshProUGUI m_BonusText_GM;
    [SerializeField] TextMeshProUGUI m_TotalWordsText_GM;

    [Header("Canvas Group")]
    [SerializeField] CanvasGroup cg_Pause;
    [SerializeField] CanvasGroup cg_GameOver;
    [SerializeField] Image m_gridGuard;

    [Header("Past-Words Related")]
    [SerializeField] TextMeshProUGUI m_WordPrefab;
    [SerializeField] RectTransform m_ExistingWordParent;

    [Header("Buttons")]
    [SerializeField] Button btn_Pause;
    [SerializeField] Button btn_Resume;
    [SerializeField] Button btn_Restart;
    [SerializeField] Button btn_Quit;
    [SerializeField] Button btn_Quit_GM;

    [Header("Config")]
    [SerializeField] GameConfig m_gameConfig;
    //Others
    StatsController m_Controller = null;


    void OnEnable()
    {
        m_Controller ??= new(this,m_gameConfig);
        InitListeners();
    }

    private void Start()
    {
        StartCoroutine(m_Controller.TimerRoutine());
    }


    private void InitListeners()
    {
        EventManager.OnValidWordSelected.AddListener(m_Controller.ProcessValidWord);
        EventManager.IsGameOver.AddListener(m_Controller.GetIsGameOver);
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
    #endregion

    private void DeinitListeners()
    {
        EventManager.OnValidWordSelected.RemoveListener(m_Controller.ProcessValidWord);
        EventManager.IsGameOver.RemoveListener(m_Controller.GetIsGameOver);
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
