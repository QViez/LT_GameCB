using UnityEngine;
using UnityEngine.UI;

public class PlayPanelController : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button btnClose;
    public Button btnPlayGame;

    [Header("Views & Hệ thống")]
    public LoadingView loadingView;     
    public GameObject gameplayRoot;
    public GameObject canvasUI;
    public GameObject canvasInGame;

    [Header("Loading")]
    public float loadingTime = 2.0f;

    private void Awake()
    {
        if (btnClose != null) btnClose.onClick.AddListener(ClosePopup);
        if (btnPlayGame != null) btnPlayGame.onClick.AddListener(StartGameplay);

        if (loadingView != null) loadingView.HideLoading();
    }

    public void OpenPopup()
    {
        gameObject.SetActive(true);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public void StartGameplay()
    {

        if (loadingView != null)
        {
            loadingView.ShowLoading(loadingTime, SetupGame);
        }

        gameObject.SetActive(false);
    }


    private void SetupGame()
    {
        if (canvasUI != null) canvasUI.SetActive(false);
        if (gameplayRoot != null) gameplayRoot.SetActive(true);
        if (canvasInGame != null) canvasInGame.SetActive(true);

    }
}