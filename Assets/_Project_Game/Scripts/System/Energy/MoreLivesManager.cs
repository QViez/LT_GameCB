using UnityEngine;
using UnityEngine.UI;

public class MoreLivesManager : MonoBehaviour
{
    [Header("Logic Nạp Mạng")]
    public Button btnRefill;
    public int refillPrice = 900;
    public GameObject PopupShop; // Kéo Panel_Shop vào đây

    private void Awake()
    {
        // Tự động cắm dây nút bấm
        if (btnRefill != null)
        {
            btnRefill.onClick.AddListener(OnClickRefill);
        }
    }

    private void OnClickRefill()
    {
        // 1. Kiểm tra và trừ tiền
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.TrySpendCoins(refillPrice))
        {
            // 2. Trừ thành công -> Bơm full mạng
            if (LivesManager.Instance != null)
            {
                LivesManager.Instance.RefillAllLives();
            }

            // 3. Tắt bảng đi
            gameObject.SetActive(false);
            Debug.Log("<color=green>Nạp mạng thành công!</color>");
        }
        else
        {
            // 4. Thiếu tiền -> Bật Shop
            Debug.LogWarning("Không đủ Vàng! Đang mở bảng Shop...");
            if (PopupShop != null) PopupShop.SetActive(true);
        }
    }
}