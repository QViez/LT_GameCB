using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SplashView : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup imgSmashFest; 

    [Header("Animation")]
    public float fadeDuration = 0.5f;

    private CanvasGroup mainCanvasGroup;

    private void Awake()
    {
        mainCanvasGroup = GetComponent<CanvasGroup>();
        if (imgSmashFest != null)
        {
            imgSmashFest.alpha = 0f;
            imgSmashFest.gameObject.SetActive(false);
        }
    }

    public void ShowSmashFestLogo()
    {
        StartCoroutine(FadeInSmashFest());
    }

        public void HideSplash()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutAndHide(mainCanvasGroup, fadeDuration));
    }

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

            if (imgSmashFest != null)
            {
                imgSmashFest.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            }
            yield return null;
        }

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