using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;   
    public AudioSource soundSource;    

    [Header("Button Sound")]
    public AudioClip buttonClickClip; 



    [Header("Cannon Sound")]
    public AudioClip cannonShotClip; 


    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;

            // Giữ AudioManager khi chuyển Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusic(bool isOn)
    {
        if (musicSource != null)
        {
            musicSource.mute = !isOn;
        }
    }


    public void SetSound(bool isOn)
    {
        if (soundSource != null)
        {
            soundSource.mute = !isOn;
        }
    }


    public void PlayButtonClick()
    {
        bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        if (!soundOn)
        {
            return;
        }

        if (soundSource != null && buttonClickClip != null)
        {
            soundSource.PlayOneShot(buttonClickClip);
        }
    }

    public void PlayCannonShot()
    {

        bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        if (!soundOn)
        {
            return;
        }
        if (soundSource != null && cannonShotClip != null)
        {
            soundSource.PlayOneShot(cannonShotClip);
        }
    }

    public void AddSoundToButtons(Button btnSound)
    {
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button btn in buttons)
        {
            if (btn == btnSound)
            {
                continue;
            }
            btn.onClick.RemoveListener(PlayButtonClick);
            btn.onClick.AddListener(PlayButtonClick);
        }
    }
    public void PlaySound(AudioClip clip)
    {
        bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;

        if (!soundOn)
        {
            return;
        }

        if (soundSource != null && clip != null)
        {
            soundSource.PlayOneShot(clip);
        }
    }
}