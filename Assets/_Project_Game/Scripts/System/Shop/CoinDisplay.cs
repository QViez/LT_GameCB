using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    public TextMeshProUGUI txtCoin;
    private bool isSubscribed = false;

    private void OnEnable()
    {
        SetupCoin();
    }

    private void Start()
    {
        SetupCoin();
    }

    private void SetupCoin()
    {
        if (!isSubscribed && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinChanged += UpdateCoinUI;
            UpdateCoinUI(CurrencyManager.Instance.GetCoins());
            isSubscribed = true;
        }
    }

    private void OnDisable()
    {
        if (isSubscribed && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinChanged -= UpdateCoinUI;
            isSubscribed = false; 
        }
    }

    private void UpdateCoinUI(int currentCoins)
    {
        if (txtCoin != null)
        {
            txtCoin.text = currentCoins.ToString();
        }
    }
}