using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private int currentLevelIndex = 1;

    private const string LEVEL_KEY = "CURRENT_LEVEL_INDEX";

    private void Start()
    {
        currentLevelIndex = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>($"Levels/Level_{currentLevelIndex}");

        if (jsonAsset != null)
        {
            levelLoader.LoadLevelFromJSON(jsonAsset.text);
        }
        else
        {
            currentLevelIndex = 1;
            PlayerPrefs.SetInt(LEVEL_KEY, 1);
            PlayerPrefs.Save();

            TextAsset firstLevelAsset = Resources.Load<TextAsset>("Levels/Level_1");
            if (firstLevelAsset != null) levelLoader.LoadLevelFromJSON(firstLevelAsset.text);
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        PlayerPrefs.SetInt(LEVEL_KEY, currentLevelIndex);
        PlayerPrefs.Save();
        LoadCurrentLevel();
    }
    public void RestartLevel()
    {
        LoadCurrentLevel();
    }
}

