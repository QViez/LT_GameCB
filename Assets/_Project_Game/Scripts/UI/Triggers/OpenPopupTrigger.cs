using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] 
public class OpenPopupTrigger : MonoBehaviour
{
    [Header("Cài đặt")]
    public GameObject panelToOpen;

    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OpenPopup);
    }

    private void OpenPopup()
    {
        if (panelToOpen != null)
        {
            panelToOpen.SetActive(true);
        }
    }
}