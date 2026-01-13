using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeightScore : MonoBehaviour
{
    public static HeightScore Instance;

    [Header("UI References")]
    // High score label intentionally unused in 3D flow; HUD handles high score separately.
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI[] currentScoreText;
    [SerializeField] private TextMeshProUGUI gameOverCurrentScoreText;
    [SerializeField] private Image newImgPopup;   
    [SerializeField] private Image newImgPopupIf;   
    [SerializeField] private Image newImgPopupElse;  

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem popupParticle;     
    [SerializeField] private ParticleSystem popupIfParticle;   
    [SerializeField] private ParticleSystem popupElseParticle; 

    public static int highScore;
    private int currentScore;

    [Header("Popup Settings")]
    [SerializeField] private float popupDuration = 2f;
    [SerializeField] private float fadeSpeed = 2f;
    private bool hasShownPopup = false;
    private int lastMilestoneGiven = 0;
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;
    private bool isFirstGame = false;

    private const string HighScoreKey = "heightScoreGet";
    private const string CurrentScoreKey = "currentHeightScore";
    private const string PopupShownKey = "popupShown";
    private const string FirstPlayKey = "firstPlayDone";
    private const string FirstGameOverKey = "firstGameOverDone";
    [SerializeField] private bool enablePowerupPopups = false;


    // Ensure there is always a HeightScore instance available at runtime,
    // even if the component is only placed on game-over panels in the scene.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstance()
    {
        if (Instance != null) return;

        var go = new GameObject("HeightScoreManager");
        DontDestroyOnLoad(go);
        go.AddComponent<HeightScore>();
    }

    private void Awake()
    {
        // Robust singleton: prefer the instance that actually has UI wired up.
        if (Instance != null && Instance != this)
        {
            bool thisHasUi = highScoreText != null ||
                             (currentScoreText != null && currentScoreText.Length > 0);
            bool existingHasUi = Instance.highScoreText != null ||
                                 (Instance.currentScoreText != null && Instance.currentScoreText.Length > 0);

            if (thisHasUi && !existingHasUi)
            {
                // Replace headless instance with this scene's UI instance.
                Destroy(Instance.gameObject);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            Instance = this;
        }

        CacheUiReferencesIfMissing();
    }

    private void OnEnable()
    {
        CacheUiReferencesIfMissing();
        UpdateUI();
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt(FirstPlayKey, 0) == 0)
        {
            isFirstGame = true;
            PlayerPrefs.SetInt(FirstPlayKey, 1);
            PlayerPrefs.Save();
        }

        LoadProgress();
        
        // Resume current score if we're resuming a saved 3D game
        if (Assets.Scripts.GameManager.GM.GetInstance().isSavedGame() && 
            Assets.Scripts.GameManager.GM.GetInstance().GetSavedGameID() == 3)
        {
            int savedScore = PlayerPrefs.GetInt(CurrentScoreKey, 0);
            if (savedScore > 0)
            {
                currentScore = savedScore;
                Debug.Log($"[HeightScore] Resuming 3D game with score: {currentScore}");
            }
        }
        
        UpdateUI();

        if (newImgPopup != null)
        {
            newImgPopup.gameObject.SetActive(false);
        }
        if (newImgPopupIf != null)
        {
            newImgPopupIf.gameObject.SetActive(false);
        }
        if (newImgPopupElse != null)
        {
            newImgPopupElse.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Protects against missing inspector wiring in the 3D scene by auto-finding the score UI texts.
    /// </summary>
    private void CacheUiReferencesIfMissing()
    {
        // High score text is not auto-bound here; HUD owns high score display in 3D.
        highScoreText = null;

        // Optional: find game-over score label by name.
        if (gameOverCurrentScoreText == null)
        {
            gameOverCurrentScoreText = FindObjectsOfType<TextMeshProUGUI>(true)
                .FirstOrDefault(t =>
                    t.name.ToLower().Contains("gameover") &&
                    t.name.ToLower().Contains("score"));
        }

        // Current score labels (could be multiple HUD instances)
        if (currentScoreText == null || currentScoreText.Length == 0)
        {
            currentScoreText = FindObjectsOfType<TextMeshProUGUI>(true)
                .Where(t =>
                    (t.name.ToLower().Contains("currentscore") ||
                     t.name.ToLower().Contains("scoretext") ||
                     t.name.ToLower().Equals("currentscore")) &&
                    (highScoreText == null || t != highScoreText)) // avoid capturing high score text
                .ToArray();
        }
    }

    private void LoadProgress()
    {
        // Load 3D high score specifically
        highScore = Assets.Scripts.GameManager.GM.GetInstance().GetHighScore3D();
        currentScore = PlayerPrefs.GetInt(CurrentScoreKey, 0);
        hasShownPopup = PlayerPrefs.GetInt(PopupShownKey, 0) == 1;
        lastMilestoneGiven = currentScore / 1000;

     
        if (PlayerPrefs.GetInt(FirstGameOverKey, 0) == 0)
        {
            highScore = 0;
        }
    }

    public void AddScore(int value)
    {
        if (isGameOver) return;

        currentScore += value;
        UpdateUI();

        int newMilestone = currentScore / 1000;
        if (enablePowerupPopups && newMilestone > lastMilestoneGiven)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                for (int m = lastMilestoneGiven + 1; m <= newMilestone; m++)
                {
                    if (m % 2 == 1)
                    {
                        player.AddFirstModelUses(1);
                        ShowPopupWithFade(newImgPopupIf, popupIfParticle);
                    }
                    else
                    {
                        player.AddSecondModelUses(1);
                        ShowPopupWithFade(newImgPopupElse, popupElseParticle);
                    }
                }
            }

            lastMilestoneGiven = newMilestone;
        }

        // Always update high score as soon as it is exceeded (match 2D behaviour)
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(FirstGameOverKey, 1); // allow HUD to display high score immediately

            // REAL-TIME SYNC: Trigger Firebase save immediately when we beat the high score.
            // This ensures the leaderboard is updated while playing, not just after Game Over.
            Assets.Scripts.GameManager.GM.GetInstance().SaveHighScore3D(highScore);

            if (enablePowerupPopups && !hasShownPopup)
            {
                ShowPopupWithFade(newImgPopup, popupParticle);
                hasShownPopup = true;
                PlayerPrefs.SetInt(PopupShownKey, 1);
            }
        }

        SaveProgress();
    }

    private void UpdateUI()
    {
      
        // High score UI is handled by HUD (HeightScoreHUD) in 3D; skip here.

        if (currentScoreText != null && currentScoreText.Length > 0)
        {
            foreach (var text in currentScoreText)
            {
                if (text == null) continue;

                // Force these labels to be active while updating, so score is always visible in HUD.
                text.gameObject.SetActive(true);
                text.text = currentScore.ToString();
            }
        }

        // Game-over panel current score (if assigned)
        if (gameOverCurrentScoreText != null)
        {
            gameOverCurrentScoreText.gameObject.SetActive(true);
            gameOverCurrentScoreText.text = currentScore.ToString();
        }
    }

 
    private void ShowPopupWithFade(Image popupImage, ParticleSystem particle)
    {
        if (popupImage == null) return;

        popupImage.gameObject.SetActive(true);
        popupImage.canvasRenderer.SetAlpha(0f);

        popupImage.CrossFadeAlpha(1f, 1f / fadeSpeed, false);

        if (particle != null)
        {
            particle.transform.position = popupImage.transform.position;
            particle.Play();
        }

        StartCoroutine(HidePopupAfterDelay(popupImage, popupDuration));
    }

    private IEnumerator HidePopupAfterDelay(Image popupImage, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (popupImage != null)
        {
            popupImage.CrossFadeAlpha(0f, 1f / fadeSpeed, false);
            yield return new WaitForSeconds(1f / fadeSpeed);
            popupImage.gameObject.SetActive(false);
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.SetInt(CurrentScoreKey, currentScore);
        PlayerPrefs.Save();
    }

    public void GameOver()
    {
        isGameOver = true;

 
        PlayerPrefs.SetInt(FirstGameOverKey, 1);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            hasShownPopup = false;
            PlayerPrefs.SetInt(PopupShownKey, 0);
            
            // Save 3D high score separately - call GM to handle sync
            Assets.Scripts.GameManager.GM.GetInstance().SaveHighScore3D(currentScore);
        }
        else
        {
            // Even if not a *new* high score locally, we should try to ensure the cloud is up to date
            // or at least give the GM a chance to check.
            Assets.Scripts.GameManager.GM.GetInstance().SaveHighScore3D(highScore);
        }

        SaveProgress();
        UpdateUI();

        isFirstGame = false;
    }

    public void ResetCurrentScore()
    {
        currentScore = 0;
        hasShownPopup = false;
        lastMilestoneGiven = 0;
        isGameOver = false;
        PlayerPrefs.SetInt(PopupShownKey, 0);

        UpdateUI();
        SaveProgress();
    }

    public void ResetAllScores()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.DeleteKey(CurrentScoreKey);
        PlayerPrefs.DeleteKey(PopupShownKey);
        PlayerPrefs.DeleteKey(FirstPlayKey);
        PlayerPrefs.DeleteKey(FirstGameOverKey);
        PlayerPrefs.Save();

        highScore = 0;
        currentScore = 0;
        hasShownPopup = false;
        lastMilestoneGiven = 0;
        isGameOver = false;
        isFirstGame = true;

        UpdateUI();
    }

    public void StartNewGame()
    {
        isGameOver = false;
        ResetCurrentScore();
    }

    private void OnApplicationQuit()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(PopupShownKey, 0);
            
            // Save 3D high score separately
            Assets.Scripts.GameManager.GM.GetInstance().SaveHighScore3D(currentScore);
        }
        else
        {
             Assets.Scripts.GameManager.GM.GetInstance().SaveHighScore3D(highScore);
        }

        SaveProgress();
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}
