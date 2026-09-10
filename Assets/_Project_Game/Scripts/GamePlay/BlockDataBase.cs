using UnityEngine;

public class BlockDataBase : ScriptableObject
{
    [Header("Block Info")]
    public string blockName;
    public float mass = 1f;

    [Header("Visual & Effects")]
    public GameObject vfxPrefab; 
}