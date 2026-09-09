using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] 
public class EnergyButtonController : MonoBehaviour
{
    [Header("Bảng cần mở")]
    public GameObject panelMoreLives; 

    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();

        myButton.onClick.AddListener(OnEnergyButtonClicked);
    }

    private void OnEnergyButtonClicked()
    {
         if (LivesManager.Instance != null)
        {
            int currentLives = LivesManager.Instance.GetCurrentLives();
            int maxLives = LivesManager.Instance.maxLives;

            if (currentLives < maxLives)
            {
                if (panelMoreLives != null)
                {
                    panelMoreLives.SetActive(true);
                }
            }
            else
            {
                
                Debug.Log("Đã đầy mạng");

                
            }
        }
    }
}