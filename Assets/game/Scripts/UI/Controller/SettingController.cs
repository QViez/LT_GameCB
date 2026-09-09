using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    [Header("Music")]
    public Button btnMusic;
    public GameObject musicOn;
    public GameObject musicOff;
    public Image musicIcon;

    [Header("Sound")]
    public Button btnSound;
    public GameObject soundOn;
    public GameObject soundOff;
    public Image soundIcon;

    [Header("Vibration")]
    public Button btnVibration;
    public GameObject vibrationOn;
    public GameObject vibrationOff;
    public Image vibrationIcon;

    [Header("Icon Colors")]
    public Color iconOnColor = Color.white;
    public Color iconOffColor;

    private bool isMusicOn;
    private bool isSoundOn;
    private bool isVibrationOn;

    private void Start()
    {
        btnMusic.onClick.AddListener(ToggleMusic);
        btnSound.onClick.AddListener(ToggleSound);
        btnVibration.onClick.AddListener(ToggleVibration);

        LoadSettings();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.AddSoundToButtons(btnSound);
        }
    }

    public void LoadSettings()
    {
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        isSoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        isVibrationOn = PlayerPrefs.GetInt("VibrationOn", 1) == 1;

        UpdateMusicUI();
        UpdateSoundUI();
        UpdateVibrationUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusic(isMusicOn);
            AudioManager.Instance.SetSound(isSoundOn);
        }
    }

    private void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        PlayerPrefs.SetInt(
            "MusicOn",
            isMusicOn ? 1 : 0
        );

        PlayerPrefs.Save();

        UpdateMusicUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusic(isMusicOn);
        }
    }

    private void UpdateMusicUI()
    {
        musicOn.SetActive(isMusicOn);
        musicOff.SetActive(!isMusicOn);

        musicIcon.color =
            isMusicOn
                ? iconOnColor
                : iconOffColor;
    }

    private void ToggleSound()
    {
        isSoundOn = !isSoundOn;

        PlayerPrefs.SetInt(
            "SoundOn",
            isSoundOn ? 1 : 0
        );

        PlayerPrefs.Save();

        UpdateSoundUI();

        if (AudioManager.Instance != null)
        {
            if (isSoundOn)
            {
                AudioManager.Instance.SetSound(true);
                AudioManager.Instance.PlayButtonClick();
            }
            else
            {
                AudioManager.Instance.SetSound(false);
            }
        }
    }

    private void UpdateSoundUI()
    {
        soundOn.SetActive(isSoundOn);
        soundOff.SetActive(!isSoundOn);

        soundIcon.color =
            isSoundOn
                ? iconOnColor
                : iconOffColor;
    }

    private void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;

        PlayerPrefs.SetInt(
            "VibrationOn",
            isVibrationOn ? 1 : 0
        );

        PlayerPrefs.Save();

        UpdateVibrationUI();

        if (isVibrationOn)
        {
            Vibrate();
        }
    }

    private void UpdateVibrationUI()
    {
        vibrationOn.SetActive(isVibrationOn);
        vibrationOff.SetActive(!isVibrationOn);

        vibrationIcon.color =
            isVibrationOn
                ? iconOnColor
                : iconOffColor;
    }

    public void Vibrate()
    {
        bool vibrationEnabled =
            PlayerPrefs.GetInt("VibrationOn", 1) == 1;

        if (!vibrationEnabled)
        {
            return;
        }

#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    public bool IsMusicOn()
    {
        return PlayerPrefs.GetInt("MusicOn", 1) == 1;
    }

    public bool IsSoundOn()
    {
        return PlayerPrefs.GetInt("SoundOn", 1) == 1;
    }

    public bool IsVibrationOn()
    {
        return PlayerPrefs.GetInt("VibrationOn", 1) == 1;
    }

    private void OnDestroy()
    {
        if (btnMusic != null)
        {
            btnMusic.onClick.RemoveListener(ToggleMusic);
        }

        if (btnSound != null)
        {
            btnSound.onClick.RemoveListener(ToggleSound);
        }

        if (btnVibration != null)
        {
            btnVibration.onClick.RemoveListener(ToggleVibration);
        }
    }
}