using UnityEngine;

public class ContinueManager : MonoBehaviour
{
    [Header("Giá mua")]
    public int continuePrice = 900;

    public void OnClickPlayOn()
    {
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.TrySpendCoins(continuePrice))
        {
            Debug.Log("Mua thành công");

            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Không đủ tiền ");
        }
    }
}