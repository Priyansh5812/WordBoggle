using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    [SerializeField] private CanvasGroup cg_main;
    [SerializeField] private Button btn_start;
    [SerializeField] private GameObject LoadingIndication;
    private void OnEnable()
    {
        cg_main.interactable = false;
        LoadingIndication?.SetActive(false);
        WordsManager.OnWordsLoaded.AddListener(SetMenuInteractable);
        btn_start?.onClick.AddListener(SetupLoadingGameScene);
    }

    private void SetMenuInteractable() => cg_main.interactable = true;

    private void SetupLoadingGameScene()
    {
        cg_main.interactable = false;
        SceneManager.LoadScene(1);
        LoadingIndication?.SetActive(true);
    }


    private void OnDisable()
    {
        WordsManager.OnWordsLoaded.RemoveListener(SetMenuInteractable);
        btn_start?.onClick.RemoveListener(SetupLoadingGameScene);
    }

}
