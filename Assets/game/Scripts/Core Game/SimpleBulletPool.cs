using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletPool : MonoBehaviour
{
    public static SimpleBulletPool Instance { get; private set; }

    [System.Serializable]
    public class PoolItem
    {
        public string name;
        public GameObject prefab;
        public int initialSize = 5;
        public float autoReturnDelay = 1.5f; 
    }

    [Header("Danh sách Prefab ")]
    [SerializeField] private List<PoolItem> prewarmItems = new List<PoolItem>();

    [Header("Đạn mặc định ")]
    [SerializeField] private GameObject defaultBulletPrefab;
    [SerializeField] private int bulletSize = 30;

    private readonly Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    private readonly Dictionary<GameObject, HashSet<GameObject>> poolHashSet = new Dictionary<GameObject, HashSet<GameObject>>();
    private readonly Dictionary<GameObject, float> autoReturnTimes = new Dictionary<GameObject, float>();
    private readonly Dictionary<float, WaitForSeconds> waitCache = new Dictionary<float, WaitForSeconds>();

    private WaitForSeconds GetWait(float delay)
    {
        if (!waitCache.TryGetValue(delay, out var wait))
        {
            wait = new WaitForSeconds(delay);
            waitCache[delay] = wait;
        }
        return wait;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var item in prewarmItems)
        {
            if (item.prefab == null) continue;
            if (!autoReturnTimes.ContainsKey(item.prefab))
            {
                autoReturnTimes.Add(item.prefab, item.autoReturnDelay);
            }

            for (int i = 0; i < item.initialSize; i++)
            {
                CreateNewInstance(item.prefab);
            }
        }

        if (defaultBulletPrefab != null && !poolDictionary.ContainsKey(defaultBulletPrefab))
        {
            for (int i = 0; i < bulletSize; i++)
            {
                CreateNewInstance(defaultBulletPrefab);
            }
        }
    }

    private GameObject CreateNewInstance(GameObject prefab)
    {
        if (!poolDictionary.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            poolDictionary[prefab] = queue;
        }

        if (!poolHashSet.TryGetValue(prefab, out HashSet<GameObject> hashSet))
        {
            hashSet = new HashSet<GameObject>();
            poolHashSet[prefab] = hashSet;
        }

        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        queue.Enqueue(obj);
        hashSet.Add(obj);
        return obj;
    }


    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float overrideDelay = -1f)
    {
        if (prefab == null) return null;

        if (!poolDictionary.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            poolDictionary[prefab] = queue;
        }

        if (!poolHashSet.TryGetValue(prefab, out HashSet<GameObject> hashSet))
        {
            hashSet = new HashSet<GameObject>();
            poolHashSet[prefab] = hashSet;
        }

        GameObject obj = null;
        while (queue.Count > 0)
        {
            obj = queue.Dequeue();
            if (obj != null)
            {
                hashSet.Remove(obj);
                break;
            }
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(null);
        obj.SetActive(true);

        float delay = overrideDelay;
        if (delay <= 0f && autoReturnTimes.TryGetValue(prefab, out float defaultDelay))
        {
            delay = defaultDelay;
        }

        if (delay > 0f)
        {
            StartCoroutine(AutoReturnRoutine(prefab, obj, delay));
        }

        return obj;
    }

    public  IEnumerator AutoReturnRoutine(GameObject prefab, GameObject instance, float delay)
    {
        yield return GetWait(delay);

        if (instance != null && instance.activeSelf)
        {
            ReturnToPool(prefab, instance);
        }
    }

    public void ReturnToPool(GameObject prefab, GameObject instance)
    {
        if (prefab == null || instance == null || !instance.activeSelf) return;

        instance.SetActive(false);
        instance.transform.SetParent(transform);

        if (!poolDictionary.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            poolDictionary[prefab] = queue;
        }

            if (!poolHashSet.TryGetValue(prefab, out HashSet<GameObject> hashSet))
            {
                hashSet = new HashSet<GameObject>();
                poolHashSet[prefab] = hashSet;
            }

        if (hashSet.Add(instance))
        {
            queue.Enqueue(instance);
        }
    }

    public GameObject GetBullet()
    {
        return Spawn(defaultBulletPrefab, transform.position, transform.rotation);
    }

    public void ReturnBullet(GameObject bullet)
    {
        ReturnToPool(defaultBulletPrefab, bullet);
    }
}