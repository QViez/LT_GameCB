using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    [Header("Tab Info")]
    public string myTabName; 
    public StringEvent onTabSelectedEvent; 

    [Header("UI References")]
    public GameObject iconUnselected;
    public GameObject imgSelected;

    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        myButton.onClick.AddListener(BroadcastMyName);
        onTabSelectedEvent.OnEventRaised += HandleAnyTabChanged;
    }

    private void OnDisable()
    {
        myButton.onClick.RemoveListener(BroadcastMyName);
        onTabSelectedEvent.OnEventRaised -= HandleAnyTabChanged;
    }

    private void BroadcastMyName()
    {
        if (onTabSelectedEvent != null)
            onTabSelectedEvent.Raise(myTabName);
    }

    private void HandleAnyTabChanged(string activeTabName)
    {

        bool isMe = (activeTabName == myTabName);

        iconUnselected.SetActive(!isMe);
        imgSelected.SetActive(isMe);
    }
}