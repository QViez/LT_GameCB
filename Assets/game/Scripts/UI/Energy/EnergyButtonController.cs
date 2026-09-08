using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] // Tự động bắt buộc phải có Button
public class EnergyButtonController : MonoBehaviour
{
    [Header("Bảng cần mở")]
    public GameObject panelMoreLives; // Kéo Panel_MoreLives vào đây

    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();

        // Lắng nghe sự kiện bấm nút
        myButton.onClick.AddListener(OnEnergyButtonClicked);
    }

    private void OnEnergyButtonClicked()
    {
        // 1. Giao tiếp với Model (LivesManager) để kiểm tra luật chơi
        if (LivesManager.Instance != null)
        {
            int currentLives = LivesManager.Instance.GetCurrentLives();
            int maxLives = LivesManager.Instance.maxLives;

            if (currentLives < maxLives)
            {
                // 2. Logic đúng (đang thiếu mạng) -> Gọi View mở bảng
                if (panelMoreLives != null)
                {
                    panelMoreLives.SetActive(true);
                }
            }
            else
            {
                // 3. Logic sai (đã full mạng) -> Chặn lại không cho mở
                Debug.Log("<color=yellow>Đã đầy mạng, không thể mở bảng nạp thêm!</color>");

                // (Tùy chọn) Có thể gọi hệ thống AudioManager phát ra âm thanh "tít tít" báo lỗi ở đây
            }
        }
    }
}