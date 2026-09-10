using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlockLevel blockLevelDatabase;
    [SerializeField] private SimpleCannon cannon;
    [SerializeField] private Transform levelParent;

    private readonly List<GameObject> _spawnedBlocks = new List<GameObject>();

    private void Awake()
    {
        if (blockLevelDatabase != null) blockLevelDatabase.Init();
        if (levelParent == null)
        {
            levelParent = new GameObject("[LEVEL_CONTAINER]").transform;
        }
    }

    public void LoadLevelFromJSON(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText))
        {
           
            return;
        }

        LevelData data = JsonUtility.FromJson<LevelData>(jsonText);
        if (data == null || data.blocks == null)
        {
          
            return;
        }

        ClearCurrentLevel();

        
        if (cannon != null)
        {
            cannon.SetMaxBullets(data.MaxBullets);
        }

        foreach (BlockData bData in data.blocks)
        {
            GameObject prefab = blockLevelDatabase != null ? blockLevelDatabase.GetPrefabByName(bData.prefabName) : null;
            if (prefab != null)
            {
                if (prefab.GetComponent<SimpleCannon>() != null)
                {
                   continue; 
                }

                GameObject obj = Instantiate(prefab, bData.position, Quaternion.Euler(bData.rotation), levelParent);
                _spawnedBlocks.Add(obj);

                
                if (obj.TryGetComponent<Block>(out Block blockScript))
                {
                    if (GameRuleController.Instance != null)
                    {
                        GameRuleController.Instance.RegisterBlock(blockScript);
                    }
                }
            }
        }

        Debug.Log($"[LevelLoader] Load thành công Level {data.levelName} ({data.MaxBullets} đạn, {data.blocks.Count} blocks)");
    }

    public void ClearCurrentLevel()
    {
        foreach (var block in _spawnedBlocks)
        {
            if (block != null) Destroy(block);
        }
        _spawnedBlocks.Clear();
    }
}

