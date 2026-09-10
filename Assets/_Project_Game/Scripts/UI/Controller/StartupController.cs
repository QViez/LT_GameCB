using System.Collections;
using UnityEngine;

public class StartupController : MonoBehaviour
{
    [Header("Liên kết View")]
    public SplashView splashView; 
    [Header("Logic")]
    public float timeShowSmashFest = 2.0f;
    public StringEvent onTabSelectedEvent;

    private static bool hasShownSplash = false;

    private void Start()
    {
        if (hasShownSplash == true)
        {
            if (splashView != null)
            {
                splashView.gameObject.SetActive(false);
            }
            if (onTabSelectedEvent != null)
            {
                onTabSelectedEvent.Raise("Home");
            }
        }
        else
        {
            hasShownSplash = true;

            StartCoroutine(PlaySplashScreenRoutine());
        }
    }

    private IEnumerator PlaySplashScreenRoutine()
    {
        if (splashView != null) splashView.ShowSmashFestLogo();

        yield return new WaitForSeconds(timeShowSmashFest);

        if (splashView != null) splashView.HideSplash();

        if (onTabSelectedEvent != null)
        {
            onTabSelectedEvent.Raise("Home");
        }
    }
}