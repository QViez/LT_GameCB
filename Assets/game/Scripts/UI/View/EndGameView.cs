using System;
using UnityEngine;
using UnityEngine.UI;

public class EndGameView : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject continuePanel;

    [Header("Nút Bấm ")]
    public Button btnTryAgain;
    public Button btnHome;
    public Button btnPlayOn;         
    public Button btnCloseContinue;  

    // Cổng phát thanh sự kiện
    public event Action OnTryAgainClicked;
    public event Action OnHomeClicked;
    public event Action OnPlayOnClicked;        
    public event Action OnContinueCloseClicked; 
    private void Awake()
    {
        if (btnTryAgain != null) btnTryAgain.onClick.AddListener(() => OnTryAgainClicked?.Invoke());
        if (btnHome != null) btnHome.onClick.AddListener(() => OnHomeClicked?.Invoke());
        if (btnPlayOn != null) btnPlayOn.onClick.AddListener(() => OnPlayOnClicked?.Invoke());
        if (btnCloseContinue != null) btnCloseContinue.onClick.AddListener(() => OnContinueCloseClicked?.Invoke());
    }

    public void ShowWin()
    {
        HideAll();
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void ShowLose()
    {
        HideAll();
        if (losePanel != null) losePanel.SetActive(true);
    }

    public void ShowContinue()
    {
        HideAll();
        if (continuePanel != null) continuePanel.SetActive(true);
    }

    public void HideAll()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (continuePanel != null) continuePanel.SetActive(false);
    }
}