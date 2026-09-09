using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] 
public class ClosePopupTrigger : MonoBehaviour
{
    public GameObject panelToClose;
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(ClosePopup);
    }

    private void ClosePopup()
    {
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
        }
        else
        {
            Transform parentPopup = GetTopLevelPanel(transform);
            if (parentPopup != null)
            {
                parentPopup.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy Panel nào để tắt!");
            }
        }
    }
    private Transform GetTopLevelPanel(Transform current)
    {
        if (current.parent == null || current.parent.GetComponent<Canvas>() != null)
        {
            return current;
        }
        return GetTopLevelPanel(current.parent);
    }
}