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
    [SerializeField] private float raycastDistance = 50f;

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
    private bool isAiming = false; // Theo dõi xem người chơi có đang giữ ngón tay không

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

    // =====================================================
    // UPDATE - BẮT SỰ KIỆN CHẠM/KÉO/NHẢ
    // =====================================================
    // =====================================================
    // UPDATE - ĐÃ SỬA LẠI ĐỂ TƯƠNG THÍCH HOÀN HẢO VỚI SIMULATOR/MOBILE
    // =====================================================
    // =====================================================
    // UPDATE - BẮT SỰ KIỆN CHẠM/KÉO/NHẢ
    // =====================================================
    private void Update()
    {
        bool isPointerDown = false;
        bool isPointerHeld = false;
        bool isPointerUp = false;
        Vector2 screenPosition = Vector2.zero;

        // Bắt sự kiện Input
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

        // 1. KHI BẮT ĐẦU CHẠM VÀO MÀN HÌNH
        if (isPointerDown)
        {
            // 🔥 GỌI HÀM BẢO VỆ Ở ĐÂY: Nếu chạm trúng Nút bấm -> Chặn luôn!
            if (IsPointerOverUI())
            {
                isAiming = false; // Tắt cờ ngắm bắn
                Debug.Log("Chạm vào UI -> Đã khóa nòng pháo!");
                return;
            }

            // Nếu không chạm UI và có đạn thì cho phép ngắm
            if (currentBullets > 0 || isInfiniteAmmoActive || isBigBulletActive)
            {
                isAiming = true;
            }
        }

        // 2. KHI ĐANG KÉO TAY (NGẮM)
        if (isAiming && isPointerHeld)
        {
            Aim(screenPosition);
        }

        // 3. KHI NHẢ TAY (BẮN)
        if (isAiming && isPointerUp)
        {
            isAiming = false; // Tắt ngắm
            ExecuteShoot();   // Bóp cò
        }
    }

    // =====================================================
    // HÀM NGẮM: CHỈ XOAY PHÁO, KHÔNG BẮN ĐẠN
    // =====================================================
    // =====================================================
    // HÀM NGẮM: CHỈ XOAY PHÁO, KHÔNG BẮN ĐẠN
    // =====================================================
    private void Aim(Vector2 screenPos)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null || firePoint == null) return;

        // Bắn Raycast để tìm điểm ngắm
        Ray ray = mainCam.ScreenPointToRay(screenPos);

        // 🔥 THÊM LẠI LỆNH VẼ TIA LASER Ở ĐÂY (Vẽ trong 1 frame vì ngắm diễn ra liên tục)
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance)
            ? hitInfo.point
            : ray.GetPoint(raycastDistance);

        // Tính hướng và xoay nòng pháo
        Vector3 cannonLookDirection = (targetPoint - transform.position).normalized;

        if (cannonLookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(cannonLookDirection);
        }
    }

    // =====================================================
    // HÀM BẮN: TẠO ĐẠN BAY THEO HƯỚNG HIỆN TẠI CỦA NÒNG PHÁO
    // =====================================================
    private void ExecuteShoot()
    {
        // Chốt lại 1 lần nữa xem có đạn không trước khi bắn
        if (!(currentBullets > 0 || isInfiniteAmmoActive || isBigBulletActive)) return;

        bool wasBigBullet = isBigBulletActive;

        if (SimpleBulletPool.Instance == null) return;
        GameObject bullet = SimpleBulletPool.Instance.GetBullet();

        if (bullet != null)
        {
            // Lấy chính hướng của nòng pháo hiện tại (firePoint.forward) làm hướng bắn
            Vector3 shootDirection = firePoint.forward;

            bullet.transform.SetPositionAndRotation(firePoint.position, Quaternion.LookRotation(shootDirection));

            // -- Xử lý Scale đạn --
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

            // -- Xử lý Hủy đạn --
            if (bullet.TryGetComponent<Bullet>(out Bullet bulletScript))
            {
                bulletScript.OnRelease = (go) =>
                {
                    go.transform.localScale = originalBulletScale;
                    SimpleBulletPool.Instance.ReturnBullet(go);
                    if (GameRuleController.Instance != null) GameRuleController.Instance.RegisterBulletReturned();
                };
            }

            // -- Vật lý bay --
            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = shootDirection * bulletSpeed;
            }

            // -- Âm thanh & VFX --
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCannonShot();

            if (muzzleVFXPrefab != null)
            {
                GameObject flash = Instantiate(muzzleVFXPrefab, firePoint.position, firePoint.rotation);
                Destroy(flash, 0.5f);
            }
        }

        // -- Trừ đạn sau khi bắn --
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

    // =====================================================
    // KIỂM TRA CHẠM UI (BẢO VỆ KÉP CHO CẢ PC LẪN MOBILE)
    // =====================================================
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // 1. Kiểm tra cảm ứng (Dành cho điện thoại thật)
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    return true;
            }
        }

        // 2. Kiểm tra chuột (Dành cho PC và Simulator)
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