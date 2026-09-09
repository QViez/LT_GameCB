using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;   
    public AudioSource soundSource;   
    public AudioSource CannonSource;  

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


    // =====================================================
    // MUSIC
    // =====================================================

    public void SetMusic(bool isOn)
    {
        if (musicSource != null)
        {
            // ON  -> mute = false
            // OFF -> mute = true
            musicSource.mute = !isOn;
        }
    }


    // =====================================================
    // SOUND
    // =====================================================

    public void SetSound(bool isOn)
    {
        if (soundSource != null)
        {
            soundSource.mute = !isOn;
        }
    }


    // Phát tiếng click của Button
    public void PlayButtonClick()
    {
        // Đọc trạng thái Sound đã lưu
        bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;

        // Sound OFF thì không phát
        if (!soundOn)
        {
            return;
        }

        if (soundSource != null && buttonClickClip != null)
        {
            soundSource.PlayOneShot(buttonClickClip);
        }
    }


    // =====================================================
    // CANNON SOUND
    // =====================================================

    // Gọi hàm này mỗi khi khẩu pháo bắn
    public void PlayCannonShot()
    {
        // Kiểm tra Sound trong Setting
        bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;

        // Nếu Sound đang OFF thì không phát
        if (!soundOn)
        {
            return;
        }

        // Phát tiếng pháo đúng 1 lần
        if (soundSource != null && cannonShotClip != null)
        {
            soundSource.PlayOneShot(cannonShotClip);
        }
    }


    // =====================================================
    // TỰ GẮN SOUND VÀO CÁC BUTTON
    // =====================================================

    public void AddSoundToButtons(Button btnSound)
    {
        // Tìm tất cả Button trong Scene
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button btn in buttons)
        {
            // Btn_Sound xử lý riêng trong SettingController
            if (btn == btnSound)
            {
                continue;
            }

            // Tránh bị add nhiều lần
            btn.onClick.RemoveListener(PlayButtonClick);

            // Tự thêm tiếng click
            btn.onClick.AddListener(PlayButtonClick);
        }
    }


    // =====================================================
    // PHÁT SOUND EFFECT KHÁC
    // =====================================================

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