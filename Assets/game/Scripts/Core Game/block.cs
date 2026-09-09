using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Block : MonoBehaviour
{
    [Header("Data Reference")]
    [SerializeField] private BlockDataBase data;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask groundLayers;

    private Rigidbody rb;
    public event Action<Block> OnBlockDestroyed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (GameRuleController.Instance != null)
        {
            GameRuleController.Instance.RegisterBlock(this);
        }
    }

    private void OnEnable()
    {
        if (data != null && rb != null)
        {
            rb.mass = data.mass;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
 
        if ((groundLayers.value & (1 << collision.gameObject.layer)) != 0)
        {

            if (data != null && data.vfxPrefab != null && collision.contacts.Length > 0)
            {
                ContactPoint contact = collision.contacts[0];
                SimpleBulletPool.Instance.Spawn(
                    data.vfxPrefab,
                    contact.point,
                    Quaternion.LookRotation(contact.normal)
                );
                SimpleBulletPool.Instance.ReturnToPool(data.vfxPrefab, data.vfxPrefab);
            }

            OnBlockDestroyed?.Invoke(this);

            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {

        CancelInvoke();
    }

    public void ForceDestroy()
    {
        if (data != null && data.vfxPrefab != null)
        {
            SimpleBulletPool.Instance.Spawn(data.vfxPrefab, transform.position, Quaternion.identity);
            SimpleBulletPool.Instance.ReturnToPool(data.vfxPrefab, data.vfxPrefab);
        }

        OnBlockDestroyed?.Invoke(this);
        
        gameObject.SetActive(false);
    }
}

