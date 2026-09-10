using UnityEngine;

public class TabPanel : MonoBehaviour
{
    [Header("Cấu hình Tab")]
    public string myTabName; 
    public StringEvent onTabSelectedEvent;

    [Header("Giao diện cần bật/tắt")]
    public GameObject panelToToggle; 

    private void OnEnable()
    {
        if (onTabSelectedEvent != null)
            onTabSelectedEvent.OnEventRaised += HandleTabChanged;
    }

    private void OnDisable()
    {
        if (onTabSelectedEvent != null)
            onTabSelectedEvent.OnEventRaised -= HandleTabChanged;
    }

    private void HandleTabChanged(string activeTabName)
    {
        if (panelToToggle != null)
        {
            panelToToggle.SetActive(activeTabName == myTabName);
        }
    }
}