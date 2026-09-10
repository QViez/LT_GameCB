using System.Collections.Generic;
using UnityEngine;

public class BlockLevel : ScriptableObject
{
    [SerializeField] private List<GameObject> blockPrefabs = new List<GameObject>();
    private Dictionary<string, GameObject> _lookupDict;

    public void Init()
    {
        _lookupDict = new Dictionary<string, GameObject>();
        foreach (var prefab in blockPrefabs)
        {
            if (prefab != null && !_lookupDict.ContainsKey(prefab.name))
            {
                _lookupDict.Add(prefab.name, prefab);
            }
        }
    }

    public GameObject GetPrefabByName(string prefabName)
    {
        if (_lookupDict == null) Init();

        _lookupDict.TryGetValue(prefabName, out GameObject prefab);
        return prefab;
    }
}

