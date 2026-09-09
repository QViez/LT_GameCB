using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUIController : MonoBehaviour
{
    [Header("In Game HUD")]
    public Button btnSettings;       
    public GameObject expandedMenu;  

    [Header("Setting In Game")]
    public Button btnNhac;
    public Button btnLoa;
    public Button btnRung;

    public GameObject nhacOn;
    public GameObject nhacOff;

    public GameObject soundOn;
    public GameObject soundOff;

    public GameObject vibrationOn;
    public GameObject vibrationOff;


    [Header("Quit Popup")]
    public Button btnOpenQuitPopup;  
    public GameObject panelQuitConfirm; 
    public Button btnCloseQuitPopup; 
    public Button btnConfirmQuit;    

    [Header("Hệ thống chuyển cảnh")]
    public GameObject gameplayRoot;  
    public GameObject mainCanvasUI;  
    public StringEvent onTabSelectedEvent;

    private void Awake()
    {
        if (btnSettings != null)
        {
            btnSettings.onClick.AddListener(() =>
            {
                expandedMenu.SetActive(!expandedMenu.activeSelf);
            });
        }

        btnNhac.onClick.AddListener(ToggleMusicInGame);

        btnLoa.onClick.AddListener(ToggleSoundInGame);

        btnRung.onClick.AddListener(ToggleVibrationInGame);

        UpdateMusicInGameUI();
        UpdateSoundInGameUI();
        UpdateVibrationInGameUI();


        if (btnOpenQuitPopup != null)
        {
            btnOpenQuitPopup.onClick.AddListener(() =>
            {
                panelQuitConfirm.SetActive(true);
                expandedMenu.SetActive(false); 
            });
        }

           if (btnCloseQuitPopup != null)
        {
            btnCloseQuitPopup.onClick.AddListener(() =>
            {
                panelQuitConfirm.SetActive(false);
            });
        }

        if (btnConfirmQuit != null)
        {
            btnConfirmQuit.onClick.AddListener(QuitToHome);
        }
    }


    private void OnEnable()
    {
        if (expandedMenu != null) expandedMenu.SetActive(false);
        if (panelQuitConfirm != null) panelQuitConfirm.SetActive(false);
    }

    private void QuitToHome()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void UpdateMusicInGameUI()
    {
        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        nhacOn.SetActive(musicOn);
        nhacOff.SetActive(!musicOn);
    }

    private void UpdateSoundInGameUI()
    {
        bool soundOnState =
            PlayerPrefs.GetInt("SoundOn", 1) == 1;

        soundOn.SetActive(soundOnState);
        soundOff.SetActive(!soundOnState);
    }

    private void UpdateVibrationInGameUI()
    {
        bool vibrationOnState =
            PlayerPrefs.GetInt("VibrationOn", 1) == 1;
        vibrationOn.SetActive(vibrationOnState);
        vibrationOff.SetActive(!vibrationOnState);
    }


      private void ToggleMusicInGame()
    {
        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        musicOn = !musicOn;

        PlayerPrefs.SetInt("MusicOn", musicOn ? 1 : 0);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusic(musicOn);
        }
        UpdateMusicInGameUI();
    }

    private void ToggleSoundInGame()
    {
        bool soundOnState =
            PlayerPrefs.GetInt("SoundOn", 1) == 1;

        soundOnState = !soundOnState;

        PlayerPrefs.SetInt(
            "SoundOn",
            soundOnState ? 1 : 0
        );

        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
        {
            if (soundOnState)
            {

                AudioManager.Instance.SetSound(true);

                AudioManager.Instance.PlayButtonClick();
            }
            else
            {
   
                AudioManager.Instance.SetSound(false);
            }
        }

        UpdateSoundInGameUI();
    }

    private void ToggleVibrationInGame()
    {
        bool vibrationOnState =
            PlayerPrefs.GetInt("VibrationOn", 1) == 1;

        vibrationOnState = !vibrationOnState;

        PlayerPrefs.SetInt(
            "VibrationOn",
            vibrationOnState ? 1 : 0
        );

        PlayerPrefs.Save();

        UpdateVibrationInGameUI();

        if (vibrationOnState)
        {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
        }
    }
}