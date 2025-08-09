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
        LoadingIndication?.SetActive(false);
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
