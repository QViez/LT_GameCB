using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SplashView : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup imgSmashFest; // Chỉ còn giữ lại logo game

    [Header("Cài đặt Animation")]
    public float fadeDuration = 0.5f;

    private CanvasGroup mainCanvasGroup;

    private void Awake()
    {
        mainCanvasGroup = GetComponent<CanvasGroup>();

        // Giấu SmashFest đi lúc đầu để chuẩn bị hiệu ứng Fade In
        if (imgSmashFest != null)
        {
            imgSmashFest.alpha = 0f;
            imgSmashFest.gameObject.SetActive(false);
        }
    }

    public void ShowSmashFestLogo()
    {
        // Kích hoạt hiệu ứng hiện rõ logo
        StartCoroutine(FadeInSmashFest());
    }

        public void HideSplash()
    {
        // THÊM DÒNG NÀY: Dừng ngay lập tức việc Fade In logo nếu nó vẫn đang chạy
        StopAllCoroutines();

        // Bắt đầu làm mờ toàn bộ tấm rèm Splash để lộ ra Menu Home phía sau
        StartCoroutine(FadeOutAndHide(mainCanvasGroup, fadeDuration));
    }


    // --- LOGIC HIỆN LOGO ---
    private IEnumerator FadeInSmashFest()
    {
        if (imgSmashFest != null)
        {
            imgSmashFest.alpha = 0f;
            imgSmashFest.gameObject.SetActive(true);
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // Tăng dần độ rõ của SmashFest
            if (imgSmashFest != null)
            {
                imgSmashFest.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            }
            yield return null;
        }

        // Đảm bảo SmashFest đã hiện rõ 100%
        if (imgSmashFest != null) imgSmashFest.alpha = 1f;
    }

    private IEnumerator FadeOutAndHide(CanvasGroup target, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        target.alpha = 0f;
        gameObject.SetActive(false);
    }
}