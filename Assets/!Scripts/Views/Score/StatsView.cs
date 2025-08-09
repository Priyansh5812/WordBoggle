using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class StatsView : MonoBehaviour
{
    [Header("Stats-Related")]
    [SerializeField] TextMeshProUGUI m_TimerText;
    [SerializeField] TextMeshProUGUI m_ScoreText;

    [Header("Past-Words Related")]
    [SerializeField] GameObject m_WordPrefab;
    [SerializeField] RectTransform m_ExistingWordParent;

    [Header("Buttons")]
    [SerializeField] Button btn_Pause;

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
