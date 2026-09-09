//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.EventSystems;
//using System;
//using System.Collections;

//public class SimpleCannon : MonoBehaviour
//{
//    public static SimpleCannon Instance;

//    [Header("Cannon Settings")]
//    [SerializeField] private Transform firePoint;
//    [SerializeField] private Transform cannonBasePoint; // 🔥 MỚI: Thêm một điểm để xác định vị trí "chân pháo"
//    [SerializeField] private float bulletSpeed = 50f;
//    [SerializeField] private float raycastDistance = 50f;

//    [Header("Ammo Settings")]
//    public int maxBullets = 30;
//    private int currentBullets;

//    [Header("Bullet Scale Settings")]
//    [SerializeField] private float normalBulletScaleMultiplier = 1f; 
//    private float bigBulletScaleMultiplier = 2.5f;

//    [Header("Muzzle VFX")]
//    [SerializeField] private GameObject muzzleVFXPrefab;

//    [SerializeField] private GameObject bigBulletChargeVFXPrefab;
//    private GameObject currentChargeVFX;

//    [SerializeField] private GameObject infiniteAmmoVFXPrefab;
//    private GameObject currentInfiniteAmmoVFX;

//    private bool isBigBulletActive = false;
//    private bool isInfiniteAmmoActive = false;
//    private Coroutine infiniteAmmoCoroutine;

//    private Vector3 originalBulletScale = Vector3.one;
//    private bool isScaleSaved = false;

//    public event Action<int> OnAmmoChanged;

//    private void Awake()
//    {
//        if (Instance == null) Instance = this;
//        else if (Instance != this) { Destroy(gameObject); return; }

//        ResetAmmo();
//    }

//    public void SetMaxBullets(int amount)
//    {
//        maxBullets = amount;
//        ResetAmmo();
//    }

//    public void ResetAmmo()
//    {
//        currentBullets = maxBullets;
//        isBigBulletActive = false;
//        isInfiniteAmmoActive = false;
//        if (infiniteAmmoCoroutine != null) StopCoroutine(infiniteAmmoCoroutine);

//        if (currentChargeVFX != null)
//        {
//            Destroy(currentChargeVFX);
//            currentChargeVFX = null;
//        }

//        if (currentInfiniteAmmoVFX != null)
//        {
//            Destroy(currentInfiniteAmmoVFX);
//            currentInfiniteAmmoVFX = null;
//        }

//        OnAmmoChanged?.Invoke(currentBullets);
//    }

//    public int GetCurrentBullets() => currentBullets;

//    public bool ActivateBigBullet(float scale)
//    {
//        if (isBigBulletActive) return false;
//        isBigBulletActive = true;
//        bigBulletScaleMultiplier = scale;

//        if (bigBulletChargeVFXPrefab != null && firePoint != null)
//        {
//            if (currentChargeVFX != null) Destroy(currentChargeVFX);
//            currentChargeVFX = Instantiate(bigBulletChargeVFXPrefab, firePoint.position, firePoint.rotation, firePoint);
//        }

//        return true;
//    }

//    public bool ActivateInfiniteAmmo(float duration)
//    {
//        if (isInfiniteAmmoActive) return false;
//        if (infiniteAmmoCoroutine != null) StopCoroutine(infiniteAmmoCoroutine);
//        infiniteAmmoCoroutine = StartCoroutine(InfiniteAmmoRoutine(duration));
//        return true;
//    }

//    private IEnumerator InfiniteAmmoRoutine(float duration)
//    {
//        isInfiniteAmmoActive = true;

//        if (infiniteAmmoVFXPrefab != null)
//        {
//            if (currentInfiniteAmmoVFX != null) Destroy(currentInfiniteAmmoVFX);

//            Transform basePos = cannonBasePoint != null ? cannonBasePoint : transform;

//            // 🔥 SỬA LỖI TẠI ĐÂY:
//            // 1. Dùng infiniteAmmoVFXPrefab.transform.rotation để giữ lại góc xoay gốc (-90 độ) của Prefab
//            // 2. Xóa chữ 'basePos' ở cuối (không nhận pháo làm cha nữa) để khi nòng pháo quay, vòng sáng vẫn nằm im phẳng lì
//            currentInfiniteAmmoVFX = Instantiate(infiniteAmmoVFXPrefab, basePos.position, infiniteAmmoVFXPrefab.transform.rotation);
//        }

//        yield return new WaitForSeconds(duration);

//        isInfiniteAmmoActive = false;

//        if (currentInfiniteAmmoVFX != null)
//        {
//            Destroy(currentInfiniteAmmoVFX);
//            currentInfiniteAmmoVFX = null;
//        }
//    }

//    private void Update()
//    {
//        bool isPressed = false;
//        Vector2 screenPosition = Vector2.zero;

//        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
//        {
//            isPressed = true;
//            screenPosition = Mouse.current.position.ReadValue();
//        }
//        else if (Input.GetMouseButtonDown(0))
//        {
//            isPressed = true;
//            screenPosition = Input.mousePosition;
//        }

//        if (isPressed)
//        {
//            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

//            if (currentBullets > 0 || isInfiniteAmmoActive || isBigBulletActive)
//            {
//                bool wasBigBullet = isBigBulletActive;

//                Shoot(screenPosition);

//                if (!isInfiniteAmmoActive && !wasBigBullet)
//                {
//                    currentBullets--;
//                }

//                OnAmmoChanged?.Invoke(currentBullets);

//                if (GameRuleController.Instance != null)
//                {
//                    GameRuleController.Instance.RegisterBulletFired();
//                }
//            }
//        }
//    }

//    private void Shoot(Vector2 clickPos)
//    {
//        Camera mainCam = Camera.main;
//        if (mainCam == null || firePoint == null) return;

//        Ray ray = mainCam.ScreenPointToRay(clickPos);
//        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 2.0f);
//        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance) 
//            ? hitInfo.point 
//            : ray.GetPoint(raycastDistance);

//        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;
//        Vector3 lookTarget = targetPoint;
//        //lookTarget.y = transform.position.y; KHONG XOA DONG COMMENT NAY (TUYET DOI KHONG)
//        Vector3 cannonLookDirection = (lookTarget - transform.position).normalized;

//        if (cannonLookDirection != Vector3.zero)
//        {
//            transform.rotation = Quaternion.LookRotation(cannonLookDirection);
//        }

//        if (SimpleBulletPool.Instance == null) return;
//        GameObject bullet = SimpleBulletPool.Instance.GetBullet();

//        if (bullet != null)
//        {
//            bullet.transform.SetPositionAndRotation(firePoint.position, Quaternion.LookRotation(shootDirection));

//            if (!isScaleSaved)
//            {
//                originalBulletScale = bullet.transform.localScale;
//                isScaleSaved = true;
//            }

//            Vector3 baseNormalScale = originalBulletScale * normalBulletScaleMultiplier;

//            if (isBigBulletActive)
//            {
//                bullet.transform.localScale = baseNormalScale * bigBulletScaleMultiplier;
//                isBigBulletActive = false;

//                if (currentChargeVFX != null)
//                {
//                    Destroy(currentChargeVFX);
//                    currentChargeVFX = null;
//                }
//            }
//            else
//            {
//                bullet.transform.localScale = baseNormalScale;
//            }

//            if (bullet.TryGetComponent<Bullet>(out Bullet bulletScript))
//            {
//                bulletScript.OnRelease = (go) =>
//                {
//                    go.transform.localScale = originalBulletScale;
//                    SimpleBulletPool.Instance.ReturnBullet(go);

//                    if (GameRuleController.Instance != null)
//                    {
//                        GameRuleController.Instance.RegisterBulletReturned();
//                    }
//                };
//            }

//            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
//            {
//                rb.linearVelocity = Vector3.zero;
//                rb.angularVelocity = Vector3.zero;
//                rb.linearVelocity = shootDirection * bulletSpeed;
//            }

//            if (muzzleVFXPrefab != null)
//            {
//                GameObject flash = Instantiate(muzzleVFXPrefab, firePoint.position, firePoint.rotation);
//                Destroy(flash, 0.5f); 
//            }
//        }
//    }

//    public void AddBullets(int amount)
//    {
//        currentBullets += amount;
//        OnAmmoChanged?.Invoke(currentBullets); 
//    }

//    private void OnDestroy()
//    {
//        if (Instance == this) Instance = null;
//    }
//}
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class SimpleCannon : MonoBehaviour
{
    // =====================================================
    // SINGLETON
    // =====================================================

    // Cho phép các script khác gọi SimpleCannon.Instance
    public static SimpleCannon Instance;


    // =====================================================
    // CANNON SETTINGS
    // =====================================================

    [Header("Cannon Settings")]

    // Điểm đầu nòng pháo
    // Đạn sẽ xuất hiện tại vị trí này
    [SerializeField] private Transform firePoint;

    // Điểm chân pháo
    // Dùng để đặt VFX Infinite Ammo ở dưới chân pháo
    [SerializeField] private Transform cannonBasePoint; // 🔥 MỚI: Thêm một điểm để xác định vị trí "chân pháo"

    // Tốc độ bay của viên đạn
    [SerializeField] private float bulletSpeed = 50f;

    // Khoảng cách tối đa của Raycast
    // Dùng để xác định điểm người chơi đang click
    [SerializeField] private float raycastDistance = 50f;


    // =====================================================
    // AMMO SETTINGS
    // =====================================================

    [Header("Ammo Settings")]

    // Số đạn tối đa của pháo
    public int maxBullets = 30;

    // Số đạn hiện tại
    private int currentBullets;


    // =====================================================
    // BULLET SCALE SETTINGS
    // =====================================================

    [Header("Bullet Scale Settings")]

    // Scale mặc định của viên đạn thường
    [SerializeField] private float normalBulletScaleMultiplier = 1f;

    // Scale của Big Bullet
    private float bigBulletScaleMultiplier = 2.5f;


    // =====================================================
    // MUZZLE VFX
    // =====================================================

    [Header("Muzzle VFX")]

    // Hiệu ứng lóe sáng ở đầu nòng pháo khi bắn
    [SerializeField] private GameObject muzzleVFXPrefab;


    // =====================================================
    // BIG BULLET VFX
    // =====================================================

    // VFX xuất hiện khi đang có Big Bullet
    [SerializeField] private GameObject bigBulletChargeVFXPrefab;

    // VFX Big Bullet hiện tại đang tồn tại
    private GameObject currentChargeVFX;


    // =====================================================
    // INFINITE AMMO VFX
    // =====================================================

    // Prefab hiệu ứng Infinite Ammo
    [SerializeField] private GameObject infiniteAmmoVFXPrefab;

    // VFX Infinite Ammo hiện tại
    private GameObject currentInfiniteAmmoVFX;


    // =====================================================
    // POWER-UP STATE
    // =====================================================

    // true = phát bắn tiếp theo là Big Bullet
    private bool isBigBulletActive = false;

    // true = đang trong trạng thái đạn vô hạn
    private bool isInfiniteAmmoActive = false;

    // Coroutine quản lý thời gian Infinite Ammo
    private Coroutine infiniteAmmoCoroutine;


    // =====================================================
    // BULLET SCALE CACHE
    // =====================================================

    // Lưu scale gốc của viên đạn
    // để sau khi dùng Big Bullet có thể trả về scale ban đầu
    private Vector3 originalBulletScale = Vector3.one;

    // Kiểm tra scale gốc đã được lưu chưa
    private bool isScaleSaved = false;


    // =====================================================
    // EVENT
    // =====================================================

    // Event thông báo khi số đạn thay đổi
    // UI có thể đăng ký event này để cập nhật số đạn
    public event Action<int> OnAmmoChanged;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        // Tạo Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            // Nếu đã có một SimpleCannon khác
            // thì xóa object trùng
            Destroy(gameObject);
            return;
        }

        // Reset số đạn khi pháo được tạo
        ResetAmmo();
    }


    // =====================================================
    // SET MAX BULLETS
    // =====================================================

    public void SetMaxBullets(int amount)
    {
        // Thay đổi số đạn tối đa
        maxBullets = amount;

        // Sau đó reset số đạn hiện tại về maxBullets
        ResetAmmo();
    }


    // =====================================================
    // RESET AMMO
    // =====================================================

    public void ResetAmmo()
    {
        // Đưa số đạn về tối đa
        currentBullets = maxBullets;

        // Tắt Big Bullet
        isBigBulletActive = false;

        // Tắt Infinite Ammo
        isInfiniteAmmoActive = false;


        // Nếu coroutine Infinite Ammo đang chạy
        // thì dừng lại
        if (infiniteAmmoCoroutine != null)
        {
            StopCoroutine(infiniteAmmoCoroutine);
        }


        // Xóa VFX Big Bullet nếu đang tồn tại
        if (currentChargeVFX != null)
        {
            Destroy(currentChargeVFX);
            currentChargeVFX = null;
        }


        // Xóa VFX Infinite Ammo nếu đang tồn tại
        if (currentInfiniteAmmoVFX != null)
        {
            Destroy(currentInfiniteAmmoVFX);
            currentInfiniteAmmoVFX = null;
        }


        // Thông báo số đạn mới cho UI
        OnAmmoChanged?.Invoke(currentBullets);
    }


    // =====================================================
    // GET CURRENT BULLETS
    // =====================================================

    // Cho script khác lấy số đạn hiện tại
    public int GetCurrentBullets() => currentBullets;


    // =====================================================
    // ACTIVATE BIG BULLET
    // =====================================================

    public bool ActivateBigBullet(float scale)
    {
        // Nếu Big Bullet đang active rồi
        // thì không kích hoạt thêm lần nữa
        if (isBigBulletActive)
        {
            return false;
        }


        // Bật Big Bullet
        isBigBulletActive = true;

        // Lưu scale Big Bullet được truyền vào
        bigBulletScaleMultiplier = scale;


        // Tạo VFX Big Bullet ở đầu nòng pháo
        if (bigBulletChargeVFXPrefab != null && firePoint != null)
        {
            // Nếu trước đó đã có VFX thì xóa
            if (currentChargeVFX != null)
            {
                Destroy(currentChargeVFX);
            }


            // Tạo VFX mới
            // và cho firePoint làm cha
            // để VFX đi theo nòng pháo
            currentChargeVFX = Instantiate(
                bigBulletChargeVFXPrefab,
                firePoint.position,
                firePoint.rotation,
                firePoint
            );
        }


        return true;
    }


    // =====================================================
    // ACTIVATE INFINITE AMMO
    // =====================================================

    public bool ActivateInfiniteAmmo(float duration)
    {
        // Nếu đang Infinite Ammo thì không kích hoạt lại
        if (isInfiniteAmmoActive)
        {
            return false;
        }


        // Nếu coroutine cũ còn tồn tại thì dừng
        if (infiniteAmmoCoroutine != null)
        {
            StopCoroutine(infiniteAmmoCoroutine);
        }


        // Chạy Infinite Ammo trong khoảng thời gian duration
        infiniteAmmoCoroutine =
            StartCoroutine(InfiniteAmmoRoutine(duration));

        return true;
    }


    // =====================================================
    // INFINITE AMMO ROUTINE
    // =====================================================

    private IEnumerator InfiniteAmmoRoutine(float duration)
    {
        // Bật trạng thái Infinite Ammo
        isInfiniteAmmoActive = true;


        // Tạo VFX Infinite Ammo
        if (infiniteAmmoVFXPrefab != null)
        {
            // Xóa VFX cũ nếu có
            if (currentInfiniteAmmoVFX != null)
            {
                Destroy(currentInfiniteAmmoVFX);
            }


            // Nếu có CannonBasePoint thì dùng vị trí đó
            // nếu không thì dùng vị trí của pháo
            Transform basePos =
                cannonBasePoint != null
                    ? cannonBasePoint
                    : transform;


            // 🔥 SỬA LỖI TẠI ĐÂY:
            // 1. Dùng infiniteAmmoVFXPrefab.transform.rotation để giữ lại góc xoay gốc (-90 độ) của Prefab
            // 2. Xóa chữ 'basePos' ở cuối (không nhận pháo làm cha nữa) để khi nòng pháo quay, vòng sáng vẫn nằm im phẳng lì
            currentInfiniteAmmoVFX = Instantiate(
                infiniteAmmoVFXPrefab,
                basePos.position,
                infiniteAmmoVFXPrefab.transform.rotation
            );
        }


        // Chờ hết thời gian Infinite Ammo
        yield return new WaitForSeconds(duration);


        // Tắt Infinite Ammo
        isInfiniteAmmoActive = false;


        // Xóa VFX Infinite Ammo
        if (currentInfiniteAmmoVFX != null)
        {
            Destroy(currentInfiniteAmmoVFX);
            currentInfiniteAmmoVFX = null;
        }
    }


    // =====================================================
    // UPDATE - NHẬN INPUT BẮN
    // =====================================================

    private void Update()
    {
        // Kiểm tra trong frame này người chơi có bấm hay không
        bool isPressed = false;

        // Vị trí click trên màn hình
        Vector2 screenPosition = Vector2.zero;


        // =================================================
        // NEW INPUT SYSTEM
        // =================================================

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            isPressed = true;

            // Lấy tọa độ chuột trên màn hình
            screenPosition = Mouse.current.position.ReadValue();
        }


        // =================================================
        // OLD INPUT SYSTEM
        // =================================================

        else if (Input.GetMouseButtonDown(0))
        {
            isPressed = true;

            // Lấy tọa độ chuột
            screenPosition = Input.mousePosition;
        }


        // Nếu người chơi vừa bấm
        if (isPressed)
        {
            // Nếu đang click vào UI
            // thì không bắn pháo
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }


            // Chỉ được bắn khi:
            //
            // còn đạn
            // HOẶC đang Infinite Ammo
            // HOẶC có Big Bullet
            if (currentBullets > 0 ||
                isInfiniteAmmoActive ||
                isBigBulletActive)
            {
                // Lưu lại trạng thái Big Bullet trước khi Shoot()
                //
                // Vì bên trong Shoot() sẽ đặt
                // isBigBulletActive = false
                bool wasBigBullet = isBigBulletActive;


                // Bắn
                Shoot(screenPosition);


                // Nếu không Infinite Ammo
                // và phát vừa rồi không phải Big Bullet
                // thì trừ 1 viên đạn
                if (!isInfiniteAmmoActive && !wasBigBullet)
                {
                    currentBullets--;
                }


                // Thông báo UI số đạn đã thay đổi
                OnAmmoChanged?.Invoke(currentBullets);


                // Thông báo cho GameRuleController
                // rằng người chơi vừa bắn một viên
                if (GameRuleController.Instance != null)
                {
                    GameRuleController.Instance.RegisterBulletFired();
                }
            }
        }
    }


    // =====================================================
    // SHOOT
    // =====================================================

    private void Shoot(Vector2 clickPos)
    {
        // Lấy camera chính
        Camera mainCam = Camera.main;


        // Không có camera hoặc firePoint
        // thì không thể bắn
        if (mainCam == null || firePoint == null)
        {
            return;
        }


        // =================================================
        // TẠO RAY TỪ CAMERA ĐẾN ĐIỂM CLICK
        // =================================================

        Ray ray = mainCam.ScreenPointToRay(clickPos);


        // Vẽ Ray màu đỏ trong Scene để debug
        Debug.DrawRay(
            ray.origin,
            ray.direction * raycastDistance,
            Color.red,
            2.0f
        );


        // Nếu Raycast trúng object
        // targetPoint = vị trí va chạm
        //
        // Nếu không trúng
        // lấy điểm nằm cách camera raycastDistance
        Vector3 targetPoint =
            Physics.Raycast(
                ray,
                out RaycastHit hitInfo,
                raycastDistance
            )
            ? hitInfo.point
            : ray.GetPoint(raycastDistance);


        // =================================================
        // TÍNH HƯỚNG BẮN
        // =================================================

        // Hướng từ đầu nòng pháo đến điểm click
        Vector3 shootDirection =
            (targetPoint - firePoint.position).normalized;


        // Điểm pháo sẽ quay nhìn về
        Vector3 lookTarget = targetPoint;


        //lookTarget.y = transform.position.y; KHONG XOA DONG COMMENT NAY (TUYET DOI KHONG)


        // Tính hướng từ pháo đến target
        Vector3 cannonLookDirection =
            (lookTarget - transform.position).normalized;


        // Nếu hướng hợp lệ
        // quay pháo nhìn về target
        if (cannonLookDirection != Vector3.zero)
        {
            transform.rotation =
                Quaternion.LookRotation(cannonLookDirection);
        }


        // =================================================
        // LẤY BULLET TỪ OBJECT POOL
        // =================================================

        // Nếu BulletPool chưa tồn tại thì dừng
        if (SimpleBulletPool.Instance == null)
        {
            return;
        }


        // Lấy một viên đạn từ Pool
        GameObject bullet =
            SimpleBulletPool.Instance.GetBullet();


        // Chỉ xử lý khi lấy được Bullet
        if (bullet != null)
        {
            // =================================================
            // VỊ TRÍ + HƯỚNG BULLET
            // =================================================

            // Đặt viên đạn tại đầu nòng pháo
            // đồng thời xoay viên đạn theo hướng bắn
            bullet.transform.SetPositionAndRotation(
                firePoint.position,
                Quaternion.LookRotation(shootDirection)
            );


            // =================================================
            // LƯU SCALE GỐC
            // =================================================

            // Chỉ cần lưu scale của Bullet một lần
            if (!isScaleSaved)
            {
                originalBulletScale =
                    bullet.transform.localScale;

                isScaleSaved = true;
            }


            // Scale cơ bản của viên đạn thường
            Vector3 baseNormalScale =
                originalBulletScale *
                normalBulletScaleMultiplier;


            // =================================================
            // BIG BULLET
            // =================================================

            if (isBigBulletActive)
            {
                // Tăng kích thước Bullet
                bullet.transform.localScale =
                    baseNormalScale *
                    bigBulletScaleMultiplier;


                // Big Bullet chỉ dùng cho 1 phát
                isBigBulletActive = false;


                // Sau khi bắn Big Bullet
                // xóa VFX charge
                if (currentChargeVFX != null)
                {
                    Destroy(currentChargeVFX);
                    currentChargeVFX = null;
                }
            }
            else
            {
                // Bullet thường
                bullet.transform.localScale =
                    baseNormalScale;
            }


            // =================================================
            // BULLET RELEASE
            // =================================================

            // Tìm script Bullet trên viên đạn
            if (bullet.TryGetComponent<Bullet>(
                out Bullet bulletScript))
            {
                // Khi Bullet yêu cầu được release
                bulletScript.OnRelease = (go) =>
                {
                    // Trả scale về mặc định
                    go.transform.localScale =
                        originalBulletScale;


                    // Trả Bullet về Object Pool
                    SimpleBulletPool.Instance.ReturnBullet(go);


                    // Thông báo GameRuleController
                    // viên đạn đã được trả về Pool
                    if (GameRuleController.Instance != null)
                    {
                        GameRuleController.Instance
                            .RegisterBulletReturned();
                    }
                };
            }


            // =================================================
            // BULLET PHYSICS
            // =================================================

            // Nếu Bullet có Rigidbody
            if (bullet.TryGetComponent<Rigidbody>(
                out Rigidbody rb))
            {
                // Reset vận tốc cũ
                // rất quan trọng khi dùng Object Pool
                rb.linearVelocity = Vector3.zero;

                rb.angularVelocity = Vector3.zero;


                // Cho viên đạn bay theo hướng bắn
                rb.linearVelocity =
                    shootDirection * bulletSpeed;
            }


            // =================================================
            // CANNON SOUND
            // =================================================

            // Chỉ chạy khi thực sự lấy được Bullet
            // và viên đạn được bắn
            //
            // Bắn 1 viên -> sound phát 1 lần
            // Bắn tiếp -> sound phát tiếp
            // Bắn nhanh liên tục -> PlayOneShot cho phép
            // các tiếng pháo được phát liên tục / chồng lên nhau
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCannonShot();
            }


            // =================================================
            // MUZZLE VFX
            // =================================================

            // Tạo hiệu ứng đầu nòng khi bắn
            if (muzzleVFXPrefab != null)
            {
                GameObject flash = Instantiate(
                    muzzleVFXPrefab,
                    firePoint.position,
                    firePoint.rotation
                );


                // Xóa VFX sau 0.5 giây
                Destroy(flash, 0.5f);
            }
        }
    }


    // =====================================================
    // ADD BULLETS
    // =====================================================

    public void AddBullets(int amount)
    {
        // Thêm đạn
        currentBullets += amount;


        // Cập nhật UI số đạn
        OnAmmoChanged?.Invoke(currentBullets);
    }


    // =====================================================
    // ON DESTROY
    // =====================================================

    private void OnDestroy()
    {
        // Nếu object bị Destroy
        // thì xóa Singleton reference
        if (Instance == this)
        {
            Instance = null;
        }
    }
}