using System.Collections;
using UnityEngine;

public class GameRuleController : MonoBehaviour
{
    public static GameRuleController Instance;

    [Header("Liên kết Hệ thống")]
    public SimpleCannon playerCannon;

    [Header("Giao diện UI")]
    public EndGameView endGameView;
    public BulletCountView bulletCountView;

    [Header("Cài đặt Popup")]
    public GameObject panelMoreLives;
    public GameObject panelShop;       
    public int continuePrice = 900;    

    private int activeBlocks = 0;
    private int activeBulletsFlying = 0;
    private bool isGameOver = false;
    private bool isWaitingForContinue = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (playerCannon != null)
        {
            playerCannon.OnAmmoChanged += UpdateBulletUI;
            UpdateBulletUI(playerCannon.GetCurrentBullets());
        }

        if (endGameView != null)
        {
            endGameView.OnTryAgainClicked += HandleTryAgain;
            endGameView.OnHomeClicked += HandleReturnToHome;
            endGameView.OnPlayOnClicked += HandlePlayOn;
            endGameView.OnContinueCloseClicked += HandleContinueClose;
        }
    }

    private void OnDestroy()
    {
        if (playerCannon != null) playerCannon.OnAmmoChanged -= UpdateBulletUI;
        if (endGameView != null)
        {
            endGameView.OnTryAgainClicked -= HandleTryAgain;
            endGameView.OnHomeClicked -= HandleReturnToHome;
            endGameView.OnPlayOnClicked -= HandlePlayOn;
            endGameView.OnContinueCloseClicked -= HandleContinueClose;
        }
    }

    private void HandlePlayOn()
    {
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.TrySpendCoins(continuePrice))
        {
            isWaitingForContinue = false;
            if (endGameView != null) endGameView.HideAll();
            if (playerCannon != null) playerCannon.AddBullets(5);
            Debug.Log(" Được cộng 5 viên đạn");
        }
        else
        {
            Debug.LogWarning("Không đủ Vàng!");
            if (panelShop != null) panelShop.SetActive(true);
        }
    }

    private void HandleContinueClose()
    {
        isWaitingForContinue = false;
        isGameOver = true;
        if (LivesManager.Instance != null) LivesManager.Instance.LoseLife();
        if (endGameView != null) endGameView.ShowLose();
    }

    private void HandleTryAgain()
    {
        if (LivesManager.Instance != null && LivesManager.Instance.GetCurrentLives() <= 0)
        {
            if (panelMoreLives != null) panelMoreLives.SetActive(true);
            return;
        }

        if (endGameView != null) endGameView.HideAll();
        if (SceneFlowManager.Instance != null) SceneFlowManager.Instance.ReloadScene(true);
    }

    private void HandleReturnToHome()
    {
        if (endGameView != null) endGameView.HideAll();
        if (SceneFlowManager.Instance != null) SceneFlowManager.Instance.ReloadScene(false);
    }

    public void QuitGameAndLoseLife()
    {
        isGameOver = true;
        isWaitingForContinue = false;

        if (LivesManager.Instance != null) LivesManager.Instance.LoseLife();
        if (endGameView != null) endGameView.HideAll();


        if (SceneFlowManager.Instance != null) SceneFlowManager.Instance.ReloadScene(false);
    }

    //  XỬ LÝ LOGIC LUẬT CHƠI 
    private void UpdateBulletUI(int currentAmmo)
    {
        if (bulletCountView != null) bulletCountView.UpdateAmmoText(currentAmmo);
    }

    public void ResetRules()
    {
        activeBlocks = 0;
        activeBulletsFlying = 0;
        isGameOver = false;
        isWaitingForContinue = false;
        if (endGameView != null) endGameView.HideAll();
    }

    public void RegisterBlock(Block block)
    {
        activeBlocks++;
        block.OnBlockDestroyed += HandleBlockDestroyed;
    }

    private void HandleBlockDestroyed(Block block)
    {
        activeBlocks--;
        if (block != null) block.OnBlockDestroyed -= HandleBlockDestroyed;
        StartCoroutine(CheckWinLoseRoutine());
    }

    public void RegisterBulletFired()
    {
        activeBulletsFlying++;
    }

    public void RegisterBulletReturned()
    {
        activeBulletsFlying--;
        StartCoroutine(CheckWinLoseRoutine());
    }

    private IEnumerator CheckWinLoseRoutine()
    {
        if (isGameOver || isWaitingForContinue) yield break;
        yield return new WaitForSeconds(0.05f);
        if (isGameOver || isWaitingForContinue) yield break;

        if (activeBlocks <= 0)
        {
            DeclareWin();
            yield break;
        }

        if (playerCannon != null && playerCannon.GetCurrentBullets() <= 0 && activeBulletsFlying <= 0)
        {
            float waitTimer = 0f;
            while (waitTimer < 0.1f)
            {
                if (activeBlocks <= 0)
                {
                    DeclareWin();
                    yield break;
                }
                waitTimer += Time.deltaTime;
                yield return null;
            }

            if (activeBlocks > 0 && !isGameOver)
            {
                isWaitingForContinue = true;
                if (endGameView != null) endGameView.ShowContinue();
            }
        }
    }

    private void DeclareWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (endGameView != null) endGameView.ShowWin();
        StartCoroutine(AutoGoToNextLevelRoutine(1.5f));
    }

    private IEnumerator AutoGoToNextLevelRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        int currentLevel = PlayerPrefs.GetInt("CURRENT_LEVEL_INDEX", 1);
        PlayerPrefs.SetInt("CURRENT_LEVEL_INDEX", currentLevel + 1);
        PlayerPrefs.Save();

        if (SceneFlowManager.Instance != null) SceneFlowManager.Instance.ReloadScene(true);
    }
}