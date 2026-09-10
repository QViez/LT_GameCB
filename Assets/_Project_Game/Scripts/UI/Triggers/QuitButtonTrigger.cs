using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] 
public class QuitButtonTrigger : MonoBehaviour
{
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnQuitClicked()
    {
        if (GameRuleController.Instance != null)
        {
            GameRuleController.Instance.QuitGameAndLoseLife();
        }
        else
        {
            Debug.LogError("Không tìm thấy GameRuleController trên Scene!");
        }
    }
}