using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LevelButtonTrigger : MonoBehaviour
{
    public PlayPanelController playController;
    public TextMeshProUGUI levelTextUI;

    public GameObject panelMoreLives; 

    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnLevelButtonClicked);
    }

    private void OnEnable()
    {
        if (levelTextUI != null)
        {
            int currentLevel = PlayerPrefs.GetInt("CURRENT_LEVEL_INDEX", 1);
            levelTextUI.text = "LEVEL " + currentLevel;
        }
    }

    private void OnLevelButtonClicked()
    {

        if (LivesManager.Instance != null && LivesManager.Instance.GetCurrentLives() <= 0)
        {
            if (panelMoreLives != null) panelMoreLives.SetActive(true);
            return; 
        }
        if (playController != null) playController.OpenPopup();
    }
}