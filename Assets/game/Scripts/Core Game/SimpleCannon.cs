using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class SimpleCannon : MonoBehaviour
{
    public static SimpleCannon Instance;

    [Header("Cannon Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform cannonBasePoint;
    [SerializeField] private float bulletSpeed = 50f;
    [SerializeField] private float raycastDistance = 20f;

    [Header("Ammo Settings")]
    public int maxBullets = 30;
    private int currentBullets;

    [Header("Bullet Scale Settings")]
    [SerializeField] private float normalBulletScaleMultiplier = 1f;
    private float bigBulletScaleMultiplier = 2.5f;

    [Header("Muzzle VFX")]
    [SerializeField] private GameObject muzzleVFXPrefab;
    [SerializeField] private GameObject bigBulletChargeVFXPrefab;
    private GameObject currentChargeVFX;
    [SerializeField] private GameObject infiniteAmmoVFXPrefab;
    private GameObject currentInfiniteAmmoVFX;

    private bool isBigBulletActive = false;
    private bool isInfiniteAmmoActive = false;
    private Coroutine infiniteAmmoCoroutine;

    private Vector3 originalBulletScale = Vector3.one;
    private bool isScaleSaved = false;
    private bool isAiming = false; 

    public event Action<int> OnAmmoChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ResetAmmo();
    }

    public void SetMaxBullets(int amount)
    {
        maxBullets = amount;
        ResetAmmo();
    }

    public void ResetAmmo()
    {
        currentBullets = maxBullets;
        isBigBulletActive = false;
        isInfiniteAmmoActive = false;
        isAiming = false;

        if (infiniteAmmoCoroutine != null)
        {
            StopCoroutine(infiniteAmmoCoroutine);
        }

        if (currentChargeVFX != null)
        {
            Destroy(currentChargeVFX);
            currentChargeVFX = null;
        }

        if (currentInfiniteAmmoVFX != null)
        {
            Destroy(currentInfiniteAmmoVFX);
            currentInfiniteAmmoVFX = null;
        }

        OnAmmoChanged?.Invoke(currentBullets);
    }

    public int GetCurrentBullets() => currentBullets;

    public bool ActivateBigBullet(float scale)
    {
        if (isBigBulletActive)
        {
            return false;
        }

        isBigBulletActive = true;
        bigBulletScaleMultiplier = scale;

        if (bigBulletChargeVFXPrefab != null && firePoint != null)
        {
            if (currentChargeVFX != null)
            {
                Destroy(currentChargeVFX);
            }

            currentChargeVFX = Instantiate(
                bigBulletChargeVFXPrefab,
                firePoint.position,
                firePoint.rotation,
                firePoint
            );
        }

        return true;
    }

    public bool ActivateInfiniteAmmo(float duration)
    {
        if (isInfiniteAmmoActive)
        {
            return false;
        }

        if (infiniteAmmoCoroutine != null)
        {
            StopCoroutine(infiniteAmmoCoroutine);
        }

        infiniteAmmoCoroutine = StartCoroutine(InfiniteAmmoRoutine(duration));

        return true;
    }

    private IEnumerator InfiniteAmmoRoutine(float duration)
    {
        isInfiniteAmmoActive = true;

        if (infiniteAmmoVFXPrefab != null)
        {
            if (currentInfiniteAmmoVFX != null)
            {
                Destroy(currentInfiniteAmmoVFX);
            }

            Transform basePos = cannonBasePoint != null ? cannonBasePoint : transform;

            currentInfiniteAmmoVFX = Instantiate(
                infiniteAmmoVFXPrefab,
                basePos.position,
                infiniteAmmoVFXPrefab.transform.rotation
            );
        }

        yield return new WaitForSeconds(duration);

        isInfiniteAmmoActive = false;

        if (currentInfiniteAmmoVFX != null)
        {
            Destroy(currentInfiniteAmmoVFX);
            currentInfiniteAmmoVFX = null;
        }
    }

    private void Update()
    {
        bool isPointerDown = false;
        bool isPointerHeld = false;
        bool isPointerUp = false;
        Vector2 screenPosition = Vector2.zero;

        if (Input.GetMouseButtonDown(0))
        {
            isPointerDown = true;
            screenPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            isPointerHeld = true;
            screenPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPointerUp = true;
            screenPosition = Input.mousePosition;
        }

        if (isPointerDown)
        {

            if (IsPointerOverUI())
            {
                isAiming = false; 
                return;
            }

            if (currentBullets > 0 || isInfiniteAmmoActive || isBigBulletActive)
            {
                isAiming = true;
            }
        }

        if (isAiming && isPointerHeld)
        {
            Aim(screenPosition);
        }

        if (isAiming && isPointerUp)
        {
            isAiming = false; 
            ExecuteShoot();   
        }
    }

    private void Aim(Vector2 screenPos)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null || firePoint == null) return;

        Ray ray = mainCam.ScreenPointToRay(screenPos);

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance)
            ? hitInfo.point
            : ray.GetPoint(raycastDistance);

        Vector3 cannonLookDirection = (targetPoint - transform.position).normalized;

        if (cannonLookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(cannonLookDirection);
        }
    }
    private void ExecuteShoot()
    {

        if (!(currentBullets > 0 || isInfiniteAmmoActive || isBigBulletActive)) return;

        bool wasBigBullet = isBigBulletActive;

        if (SimpleBulletPool.Instance == null) return;
        GameObject bullet = SimpleBulletPool.Instance.GetBullet();

        if (bullet != null)
        {

            Vector3 shootDirection = firePoint.forward;

            bullet.transform.SetPositionAndRotation(firePoint.position, Quaternion.LookRotation(shootDirection));

            if (!isScaleSaved)
            {
                originalBulletScale = bullet.transform.localScale;
                isScaleSaved = true;
            }
            Vector3 baseNormalScale = originalBulletScale * normalBulletScaleMultiplier;

            if (isBigBulletActive)
            {
                bullet.transform.localScale = baseNormalScale * bigBulletScaleMultiplier;
                isBigBulletActive = false;
                if (currentChargeVFX != null)
                {
                    Destroy(currentChargeVFX);
                    currentChargeVFX = null;
                }
            }
            else
            {
                bullet.transform.localScale = baseNormalScale;
            }

            if (bullet.TryGetComponent<Bullet>(out Bullet bulletScript))
            {
                bulletScript.OnRelease = (go) =>
                {
                    go.transform.localScale = originalBulletScale;
                    SimpleBulletPool.Instance.ReturnBullet(go);
                    if (GameRuleController.Instance != null) GameRuleController.Instance.RegisterBulletReturned();
                };
            }

            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = shootDirection * bulletSpeed;
            }

            if (AudioManager.Instance != null) AudioManager.Instance.PlayCannonShot();

            if (muzzleVFXPrefab != null)
            {
                GameObject flash = Instantiate(muzzleVFXPrefab, firePoint.position, firePoint.rotation);
                Destroy(flash, 0.5f);
            }
        }

  
        if (!isInfiniteAmmoActive && !wasBigBullet)
        {
            currentBullets--;
        }
        OnAmmoChanged?.Invoke(currentBullets);

        if (GameRuleController.Instance != null)
        {
            GameRuleController.Instance.RegisterBulletFired();
        }
    }

    public void AddBullets(int amount)
    {
        currentBullets += amount;
        OnAmmoChanged?.Invoke(currentBullets);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    return true;
            }
        }

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}