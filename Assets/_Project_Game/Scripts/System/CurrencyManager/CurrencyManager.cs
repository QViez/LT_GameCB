using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int currentCoins = 10000; 

    public event Action<int> OnCoinChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
 
            transform.SetParent(null); 
            DontDestroyOnLoad(gameObject);
            currentCoins = PlayerPrefs.GetInt("SAVE_COIN_DATA", currentCoins);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetCoins() => currentCoins;

    public bool TrySpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;

            PlayerPrefs.SetInt("SAVE_COIN_DATA", currentCoins);
            PlayerPrefs.Save();
            
            OnCoinChanged?.Invoke(currentCoins);
            Debug.Log($"Đã trừ {amount} coin. Số coin còn lại: {currentCoins}");
            return true;
        }

        Debug.Log("Không đủ coin!");
        return false;
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;

        PlayerPrefs.SetInt("SAVE_COIN_DATA", currentCoins);
        PlayerPrefs.Save();
        
        OnCoinChanged?.Invoke(currentCoins);
    }

    [ContextMenu("Reset Coin Data")]
    public void ResetCoinData()
    {
        PlayerPrefs.DeleteKey("SAVE_COIN_DATA");
        currentCoins = 10000;
        OnCoinChanged?.Invoke(currentCoins);
        Debug.Log("Đã reset tiền về mặc định!");
    }
}