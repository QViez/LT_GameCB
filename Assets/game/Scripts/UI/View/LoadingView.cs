using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class LoadingView : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeTime = 0.3f; 
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowLoading(float duration, Action onLoadingComplete)
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadingRoutine(duration, onLoadingComplete));
    }

    private IEnumerator LoadingRoutine(float duration, Action onLoadingComplete)
    {
        yield return StartCoroutine(FadeAlpha(0f, 1f, fadeTime));

        onLoadingComplete?.Invoke();

        float waitTime = duration - (fadeTime * 2);
        if (waitTime > 0) yield return new WaitForSeconds(waitTime);


        yield return StartCoroutine(FadeAlpha(1f, 0f, fadeTime));

        gameObject.SetActive(false);
    }

    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }

    public void HideLoading()
    {
        gameObject.SetActive(false);
    }
}