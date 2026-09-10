using System;
using UnityEngine;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance;

    [Header("Cấu hình Mạng")]
    public int maxLives = 5;
    public int timeToRecoverMinutes = 5; 

    private int currentLives;
    private DateTime nextLifeTime;

    public event Action<int, string> OnLivesUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadLives(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
          if (currentLives < maxLives)
        {
            TimeSpan timeRemaining = nextLifeTime - DateTime.Now;

          if (timeRemaining.TotalSeconds <= 0)
            {
                currentLives++;
                if (currentLives < maxLives)
                {
                    nextLifeTime = DateTime.Now.AddMinutes(timeToRecoverMinutes).AddSeconds(timeRemaining.TotalSeconds);
                }
                SaveLives();
            }
            UpdateUI();
        }
    }

    public void LoseLife()
    {
        if (currentLives == maxLives)
        {
            nextLifeTime = DateTime.Now.AddMinutes(timeToRecoverMinutes);
        }

        if (currentLives > 0)
        {
            currentLives--;
            SaveLives();
        }
    }

    public int GetCurrentLives() => currentLives;

    public void RefillAllLives()
    {
        currentLives = maxLives;
        SaveLives();
    }

    public void ForceUpdateUI()
    {
        UpdateUI();
    }

    private void LoadLives()
    {
        currentLives = PlayerPrefs.GetInt("CurrentLives", maxLives);
        string timeString = PlayerPrefs.GetString("NextLifeTime", "");

        if (currentLives < maxLives && !string.IsNullOrEmpty(timeString))
        {
            nextLifeTime = DateTime.Parse(timeString);

            while (currentLives < maxLives && DateTime.Now >= nextLifeTime)
            {
                currentLives++;
                if (currentLives < maxLives)
                {
                    nextLifeTime = nextLifeTime.AddMinutes(timeToRecoverMinutes);
                }
            }
        }
        SaveLives();
    }

    private void SaveLives()
    {
        PlayerPrefs.SetInt("CurrentLives", currentLives);
        if (currentLives < maxLives)
            PlayerPrefs.SetString("NextLifeTime", nextLifeTime.ToString());
        else
            PlayerPrefs.SetString("NextLifeTime", "");

        PlayerPrefs.Save();
        UpdateUI();
    }

    private void UpdateUI()
    {
        string timeStr = "Full";
        if (currentLives < maxLives)
        {
            TimeSpan timeRemaining = nextLifeTime - DateTime.Now;
            if (timeRemaining.TotalSeconds < 0) timeRemaining = TimeSpan.Zero;
            timeStr = string.Format("{0:D2}:{1:D2}", timeRemaining.Minutes, timeRemaining.Seconds);
        }
        OnLivesUpdated?.Invoke(currentLives, timeStr);
    }
}