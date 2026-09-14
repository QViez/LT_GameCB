using UnityEngine;
using System;

public class BoosterShopManager : MonoBehaviour
{
    public static BoosterShopManager Instance { get; private set; }

    public event Action OnBoosterPurchased;

    [Header("Booster mặc định")]
    public BoosterSO[] boosters;
    public int defaultBoosterCount = 3;

    private void Awake()
    {
        Instance = this;

        InitializeBoosters();
    }
    private void InitializeBoosters()
    {
        foreach (BoosterSO booster in boosters)
        {
            if (booster == null) continue;

            string key = $"BOOSTER_{booster.boosterID}";

            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, defaultBoosterCount);
            }
        }

        PlayerPrefs.Save();
    }

    public int GetBoosterCount(string boosterID)
    {
        return PlayerPrefs.GetInt($"BOOSTER_{boosterID}", defaultBoosterCount);
    }

    public bool BuyBoosterDirectly(BoosterSO booster)
    {
        if (booster == null) return false;

        if (CurrencyManager.Instance == null)
        {
            return false;
        }

        if (CurrencyManager.Instance.TrySpendCoins(booster.price))
        {
            int currentAmount = GetBoosterCount(booster.boosterID);

            PlayerPrefs.SetInt($"BOOSTER_{booster.boosterID}",currentAmount + 1);

            PlayerPrefs.Save();

            Debug.Log("Mua thành công 1 {booster.boosterName}! " +"Số lượng hiện có: {currentAmount + 1}");

            OnBoosterPurchased?.Invoke();
            return true;
        }

        Debug.LogWarning("Không đủ tiền");
        return false;
    }
}