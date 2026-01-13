using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("UI Buttons")]
    public Button startButton;
    public Button[] restartButtons;
    public Button mainMenuButton;

    [Header("UI Panels")]
    public GameObject uiCanvas;
    public GameObject startPanel;
    public GameObject gameOverSadPanel;
    public GameObject gameOverHappyPanel;
    public GameObject backgroundImg;
    public GameObject settingPanel;

    [Header("Settings Panel Buttons")]
    public Button continueButton;
    public Button restartButton;
    public Button menuButton;

    [Header("Settings Button")]
    public Button settingsButton;

    private const string FIRST_START_KEY = "FirstGameStarted";
    private const string GAMEOVER_STATE_KEY = "LastGameOverState"; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Ensure EventSystem exists early for mobile touch input
        EnsureEventSystem();
        EnsureCanvasRaycaster();

        // Robust reset: ensure game is NOT paused when scene loads or manager is created
        Time.timeScale = 1f;
    }

    private void Start()
    {
    
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButton);

        if (restartButtons != null && restartButtons.Length > 0)
        {
            foreach (Button btn in restartButtons)
            {
                if (btn != null)
                    btn.onClick.AddListener(OnRestartButton);
            }
        }

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMenuButton);

        InitializeSettingsPanelButtons();

        InitializeGameUI();

        if (backgroundImg != null)
            backgroundImg.SetActive(true);

        if (AdMobManager.Instance != null)
            AdMobManager.Instance.LoadBannerAd();

        InitializeSettingsPanelButtons();
    }

    private void Update()
    {
        // Handle back gesture for 3D scene
        // Only process if we're in the 3D gameplay scene (not in start panel or game over panels)
        if (startPanel != null && startPanel.activeSelf)
        {
            // Don't handle back gesture on start panel
            return;
        }
        
        if (gameOverSadPanel != null && gameOverSadPanel.activeSelf)
        {
            // Don't handle back gesture on game over panels
            return;
        }
        
        if (gameOverHappyPanel != null && gameOverHappyPanel.activeSelf)
        {
            // Don't handle back gesture on game over panels
            return;
        }
        
        // Use Utils.BackListener to handle Android back button/gesture
        Utils.BackListener(gameObject, delegate
        {
            // If pause/settings panel is already open, close it
            if (settingPanel != null && settingPanel.activeSelf)
            {
                CloseSettingPanel();
                return;
            }
            
            // If SettingsPanel3D (audio settings) is open, close it
            if (SettingsPanel3D.Instance != null && SettingsPanel3D.Instance.settingsPanel != null && SettingsPanel3D.Instance.settingsPanel.activeSelf)
            {
                SettingsPanel3D.Instance.CloseSettings();
                return;
            }
            
            // Otherwise, open the PAUSE panel (not the audio settings)
            // This matches the expected behavior: back gesture should pause the game
            OpenSettingPanel();
        });
    }

    #region Start Button
    public void OnStartButton()
    {
        PersistentAudioManager.Instance.PlayClickSound();

        if (startPanel != null) startPanel.SetActive(false);
        if (uiCanvas != null) uiCanvas.SetActive(true);

        PlayerPrefs.SetInt(FIRST_START_KEY, 1);
        PlayerPrefs.Save();
    }
    #endregion

    #region Game Over Panels
    public void GameOverSadPanel()
    {
        if (gameOverSadPanel != null) gameOverSadPanel.SetActive(true);
        if (gameOverHappyPanel != null) gameOverHappyPanel.SetActive(false);
        if (uiCanvas != null) uiCanvas.SetActive(false);
        if (startPanel != null) startPanel.SetActive(false);

        PlayerPrefs.SetInt(GAMEOVER_STATE_KEY, 1);
        PlayerPrefs.Save();
        Time.timeScale = 0f;
    }

    public void GameOverHappyPanel()
    {
        if (gameOverHappyPanel != null) gameOverHappyPanel.SetActive(true);
        if (gameOverSadPanel != null) gameOverSadPanel.SetActive(false);
        if (uiCanvas != null) uiCanvas.SetActive(false);
        if (startPanel != null) startPanel.SetActive(false);

 
        PlayerPrefs.SetInt(GAMEOVER_STATE_KEY, 2);
        PlayerPrefs.Save();
        Time.timeScale = 0f;
    }
    #endregion

    #region Restart Logic
    public void OnRestartButton()
    {
        PersistentAudioManager.Instance.PlayClickSound();

        // make sure restart never opens gameover UI again
        PlayerPrefs.DeleteKey(GAMEOVER_STATE_KEY);
        PlayerPrefs.Save();

        Reset3DGameState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region Settings & Shop
    public void OpenSettingPanel()
    {
        //PersistentAudioManager.Instance.PlayClickSound();
        PersistentAudioManager.Instance.PlayEffect("sound_eff_popup");

        if (uiCanvas != null) uiCanvas.SetActive(false);
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
            // Ensure the canvas is properly scaled and active
            Canvas canvas = settingPanel.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.enabled = true;
            }
            RectTransform rectTransform = settingPanel.GetComponent<RectTransform>();
            if (rectTransform != null && rectTransform.localScale == Vector3.zero)
            {
                rectTransform.localScale = Vector3.one;
            }
        }
    }

    public void CloseSettingPanel()
    {
        PersistentAudioManager.Instance.PlayClickSound();
        if (settingPanel != null) settingPanel.SetActive(false);
        if (uiCanvas != null) uiCanvas.SetActive(true);
    }

    public void OnContinueButton()
    {
        PersistentAudioManager.Instance.PlayClickSound();
        CloseSettingPanel();
    }

    public void OnRestartButtonFromSettings()
    {
        PersistentAudioManager.Instance.PlayClickSound();
        OnRestartButton();
    }

    public void OnMenuButton()
    {
        PersistentAudioManager.Instance.PlayClickSound();
        // When leaving to the main menu from 3D Game Over / settings:
        // - Save 3D gameplay state so that the next time we enter 3D it resumes
        //   from where we left off.
        
        // FIX: Do NOT reset the game state. Save it instead.
        // Reset3DGameState();
        
        CubeSpawner.Instance?.SaveToPlayerPrefs();
        
        // Save current score so we can resume
        if (HeightScore.Instance != null)
        {
            int currentScore = HeightScore.Instance.GetCurrentScore();
            PlayerPrefs.SetInt("currentHeightScore", currentScore);
        }

        // Set GameId to 3 to mark that we have a saved 3D game
        Assets.Scripts.GameManager.GM.GetInstance().SetSavedGameID(3);

        // Only clear Game Over state if we are actually restarting or in a clean state?
        // Actually, if we are just pausing, we should probably leave it alone or it doesn't matter.
        // But if we hit Game Over, we probably want to reset that status if we resume? 
        // No, if we resume a game over state, we are still game over.
        // PlayerPrefs.DeleteKey(GAMEOVER_STATE_KEY);
        PlayerPrefs.Save();

        // Load the main menu scene by name
        SceneManager.LoadScene("MainScene 1");
    }

    private void InitializeSettingsPanelButtons()
    {
        // EventSystem is already ensured in Awake(), but double-check here
        EnsureEventSystem();
        EnsureCanvasRaycaster();

        // Try to find buttons by name if not assigned in inspector
        if (settingPanel != null)
        {
            // Find buttons by name within the settings panel (recursive search)
            // Try multiple possible names
            if (continueButton == null)
            {
                continueButton = FindButtonInChildren(settingPanel.transform, new string[] { "ContinueButton", "Continue", "BtnContinue", "ContinueBtn" });
            }

            if (restartButton == null)
            {
                restartButton = FindButtonInChildren(settingPanel.transform, new string[] { "RestartButton", "Restart", "BtnRestart", "RestartBtn" });
            }

            if (menuButton == null)
            {
                menuButton = FindButtonInChildren(settingPanel.transform, new string[] { "MenuButton", "Menu", "BtnMenu", "MenuBtn", "HomeButton", "Home", "BtnHome" });
            }

            // Wire up the buttons
            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(OnContinueButton);
                continueButton.interactable = true;
                Debug.Log("[UiManager] Continue button wired up successfully");
            }
            else
            {
                Debug.LogWarning("[UiManager] Continue button not found in settings panel");
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestartButtonFromSettings);
                restartButton.interactable = true;
                Debug.Log("[UiManager] Restart button wired up successfully");
            }
            else
            {
                Debug.LogWarning("[UiManager] Restart button not found in settings panel");
            }

            if (menuButton != null)
            {
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(OnMenuButton);
                menuButton.interactable = true;
                Debug.Log("[UiManager] Menu button wired up successfully");
            }
            else
            {
                Debug.LogWarning("[UiManager] Menu button not found in settings panel");
            }
        }

        // --- Handle Main Settings Button (the one that opens the panel) ---
        if (settingsButton == null)
        {
            // Search for settings button in the scene/canvas
            settingsButton = FindButtonInChildren(transform.root, new string[] { "SettingsBtn", "Settings", "BtnSettings", "Button_Settings" });
        }

        if (settingsButton != null)
        {
            // Remove any existing listeners to avoid duplicates
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OnSettingsButton);
            
            // Ensure button is interactable and can receive touch input
            settingsButton.interactable = true;
            
            // Ensure the button's Image/Graphic has raycastTarget enabled
            var graphic = settingsButton.GetComponent<UnityEngine.UI.Graphic>();
            if (graphic != null)
            {
                graphic.raycastTarget = true;
            }
            
            Debug.Log("[UiManager] Main Settings button wired up successfully");
        }
        else
        {
            Debug.LogWarning("[UiManager] Main Settings button not found! UI might be unresponsive.");
        }
    }

    private Button FindButtonInChildren(Transform parent, string[] possibleNames)
    {
        foreach (string name in possibleNames)
        {
            Transform found = FindChildRecursive(parent, name);
            if (found != null)
            {
                Button btn = found.GetComponent<Button>();
                if (btn != null)
                    return btn;
            }
        }
        return null;
    }

    private void EnsureEventSystem()
    {
        // Check if EventSystem exists in the scene
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            
            // StandaloneInputModule works for both mouse and touch input
            var inputModule = eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            inputModule.forceModuleActive = true; // Force it to be active
            
            Debug.Log("[UiManager] Created EventSystem for button interaction");
        }
        else
        {
            // Ensure the existing EventSystem has an input module
            var existingEventSystem = UnityEngine.EventSystems.EventSystem.current;
            var inputModule = existingEventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (inputModule == null)
            {
                inputModule = existingEventSystem.gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                inputModule.forceModuleActive = true;
                Debug.Log("[UiManager] Added StandaloneInputModule to existing EventSystem");
            }
            else
            {
                inputModule.forceModuleActive = true;
            }
        }
    }
    
    private void EnsureCanvasRaycaster()
    {
        // Find all Canvas components and ensure they have GraphicRaycaster
        Canvas[] canvases = FindObjectsOfType<Canvas>(true); // Include inactive canvases
        foreach (Canvas canvas in canvases)
        {
            if (canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                Debug.Log($"[UiManager] Added GraphicRaycaster to Canvas: {canvas.name}");
            }
        }
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            
            Transform found = FindChildRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
    #endregion

    #region Helper Methods
    private void InitializeGameUI()
    {
        bool firstStart = PlayerPrefs.GetInt(FIRST_START_KEY, 0) == 0;
        int lastGameOverState = PlayerPrefs.GetInt(GAMEOVER_STATE_KEY, 0);


        if (lastGameOverState == 1)
        {
    
            GameOverSadPanel();
            return;
        }
        else if (lastGameOverState == 2)
        {
        
            GameOverHappyPanel();
            return;
        }

        if (firstStart)
        {
            if (startPanel != null) startPanel.SetActive(true);
            if (uiCanvas != null) uiCanvas.SetActive(false);
        }
        else
        {
            if (startPanel != null) startPanel.SetActive(false);
            if (uiCanvas != null) uiCanvas.SetActive(true);
        }

        if (gameOverSadPanel != null) gameOverSadPanel.SetActive(false);
        if (gameOverHappyPanel != null) gameOverHappyPanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
    }


    public void ResetGameProgress()
    {
        PlayerPrefs.DeleteKey(FIRST_START_KEY);
        PlayerPrefs.DeleteKey(GAMEOVER_STATE_KEY);
        PlayerPrefs.Save();
        Reset3DGameState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Clears all runtime and saved state specific to the 3D gameplay so that
    /// a new run starts from a clean slate (no leftover cubes, score, or saves).
    /// </summary>
    public void Reset3DGameState()
    {
        CubeSpawner.Instance?.ClearSavedData();
        Player.Instance?.ResetModelUses();
        HeightScore.Instance?.ResetCurrentScore();
    }
    #endregion

    /// <summary>
    /// Opens the settings panel when settings button is clicked
    /// </summary>
    public void OnSettingsButton()
    {
        Debug.Log("[UiManager] OnSettingsButton() called - Settings button was clicked!");
        
        // Ensure EventSystem is still active (safety check)
        EnsureEventSystem();
        EnsureCanvasRaycaster();
        
        //PersistentAudioManager.Instance?.PlayClickSound();
        PersistentAudioManager.Instance.PlayEffect("sound_eff_popup");

        if (SettingsPanel3D.Instance != null)
        {
            Debug.Log("[UiManager] Opening settings panel via SettingsPanel3D");
            SettingsPanel3D.Instance.OpenSettings();
        }
        else
        {
            Debug.LogError("[UiManager] SettingsPanel3D instance not found! Cannot open settings panel.");
        }
    }
}