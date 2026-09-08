using System.Collections;
using UnityEngine;

public class StartupController : MonoBehaviour
{
    [Header("Liên kết tới View")]
    public SplashView splashView; // Controller chỉ liên lạc qua View

    [Header("Cài đặt Logic")]
    public float timeShowSmashFest = 2.0f;
    public StringEvent onTabSelectedEvent;

    // Dùng từ khóa "static" để nó ghi nhớ xuyên suốt phiên chơi game
    private static bool hasShownSplash = false;

    private void Start()
    {
        // Kiểm tra xem đã từng xem màn hình Splash lần nào chưa?
        if (hasShownSplash == true)
        {
            // NẾU ĐÃ XEM RỒI (Load lại Scene)
            // THAY ĐỔI QUAN TRỌNG: Tắt Gameobject ngay lập tức thay vì gọi HideSplash() 
            // Điều này chặn đứng hiện tượng bóng ma màn hình che mất 0.5 giây.
            if (splashView != null)
            {
                splashView.gameObject.SetActive(false);
            }

            // Kích hoạt thẳng luồng Menu Home
            if (onTabSelectedEvent != null)
            {
                onTabSelectedEvent.Raise("Home");
            }
        }
        else
        {
            // NẾU CHƯA XEM (Mới mở App)
            hasShownSplash = true;

            // Bắt đầu đếm ngược chiếu Logo duy nhất
            StartCoroutine(PlaySplashScreenRoutine());
        }
    }

    private IEnumerator PlaySplashScreenRoutine()
    {
        // 1. Ra lệnh View bật Logo Game luôn
        if (splashView != null) splashView.ShowSmashFestLogo();

        yield return new WaitForSeconds(timeShowSmashFest);

        // 2. Lần đầu tiên xem thì cho phép tắt từ từ (Fade out)
        if (splashView != null) splashView.HideSplash();

        // 3. Kích hoạt luồng Menu chính
        if (onTabSelectedEvent != null)
        {
            onTabSelectedEvent.Raise("Home");
        }
    }
}