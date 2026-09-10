using UnityEngine;

public class ImpactVFX : MonoBehaviour
{
    [Header("VFX Khi Xuất Hiện")]
    [SerializeField] private GameObject spawnVFXPrefab;

    [Header("VFX Khi Va Chạm ")]
    [SerializeField] private GameObject impactVFXPrefab;
    [SerializeField] private string targetTag = "block";

    [Header("Settings")]
    [SerializeField] private float vfxLifetime = 2f;

    private bool hasCollided = false;
    private void OnEnable()
    {
        hasCollided = false; 

        if (spawnVFXPrefab != null)
        {
            GameObject vfxInstance = Instantiate(spawnVFXPrefab, transform.position, transform.rotation);
            Destroy(vfxInstance, vfxLifetime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;

        if (!string.IsNullOrEmpty(targetTag) && !collision.gameObject.CompareTag(targetTag))
            return;

        hasCollided = true;

        if (impactVFXPrefab != null)
        {
            ContactPoint contact = collision.contacts[0];

            GameObject vfxInstance = Instantiate(impactVFXPrefab, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(vfxInstance, vfxLifetime);
        }

        if (!TryGetComponent<Bullet>(out _))
        {
            gameObject.SetActive(false);
        }
    }
}