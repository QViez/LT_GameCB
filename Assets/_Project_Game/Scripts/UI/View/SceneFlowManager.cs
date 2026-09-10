using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance;

    [Header("Liên kết Giao diện")]
    public PlayPanelController playPanelController;

    // Biến static để nhớ xem đã chiếu Splash Screen lần nào chưa
    private static bool hasShownSplash = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // ================= XỬ LÝ MÀN HÌNH KHỞI ĐỘNG (SMASHFEST) =================
        GameObject splashPanel = GameObject.Find("Panel_Splash_Startup");
        if (!hasShownSplash)
        {
            hasShownSplash = true; // Lần đầu mở app
        }
        else
        {
            if (splashPanel != null) splashPanel.SetActive(false); // Các lần load lại ẩn đi
        }

        // ================= XỬ LÝ ẨN/HIỆN RÈM LOADING =================
        if (playPanelController != null)
        {
            bool autoStart = PlayerPrefs.GetInt("AutoStartGame", 0) == 1;

            if (playPanelController.canvasUI != null) playPanelController.canvasUI.SetActive(!autoStart);
            if (playPanelController.gameplayRoot != null) playPanelController.gameplayRoot.SetActive(autoStart);
            if (playPanelController.canvasInGame != null) playPanelController.canvasInGame.SetActive(autoStart);

            if (playPanelController.loadingView != null)
            {
                playPanelController.loadingView.gameObject.SetActive(autoStart);
                CanvasGroup cg = playPanelController.loadingView.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = autoStart ? 1f : 0f;
            }
        }
    }

    private void Start()
    {
        // Kéo rèm Loading ra nếu đang vào ván
        if (PlayerPrefs.GetInt("AutoStartGame", 0) == 1)
        {
            PlayerPrefs.SetInt("AutoStartGame", 0);
            PlayerPrefs.Save();
            StartCoroutine(FadeOutLoadingRoutine());
        }
    }

    private IEnumerator FadeOutLoadingRoutine()
    {
        yield return new WaitForSeconds(0.1f); // Đợi vật lý khởi tạo
        // Fade mờ cái rèm đen
        if (playPanelController != null && playPanelController.loadingView != null)
        {
            CanvasGroup cg = playPanelController.loadingView.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                float fadeDuration = 0.2f;
                float elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                    yield return null;
                }
                cg.alpha = 0f;
            }
            playPanelController.loadingView.gameObject.SetActive(false);
        }
    }

    // Hàm gọi để chuyển cảnh (Vào lại game hoặc Về Home)
    public void ReloadScene(bool isEnteringGame)
    {
        PlayerPrefs.SetInt("AutoStartGame", isEnteringGame ? 1 : 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}