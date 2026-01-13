using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Assets.Scripts.GameManager;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ProfileSetupPopup : MonoBehaviour, IAvatarSelectedHandler
{
    [Header("UI References")]
    [SerializeField] private GameObject popupContent;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button saveButton;
    [SerializeField] private Transform avatarContainer; // Parent of avatar toggle buttons
    [SerializeField] private LoadingScreen loadingScreenPrefab; // Reference if needed for instantiation
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject statusPanel; // Parent of statusText (Image + Text)
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Avatar Settings")]
    [SerializeField] private List<int> availableavatarIds = new List<int> { 0, 1, 2, 3, 4, 5 };
    
    [SerializeField] private float mobileShiftY = 400f; // Height to shift up on mobile
    
    private int selectedAvatarId = 0;
    private List<AvatarSelectorButton> avatarButtons = new List<AvatarSelectorButton>();
    private LoadingScreen loadingScreen;
    private Vector2 originalContentPosition;
    private bool hasStoredPosition = false;

    private void Start()
    {
        // Start the check coroutine BEFORE manipulating UI visibility
        // This ensures the coroutine starts even if we disable some non-root children
        StartCoroutine(CheckFirebaseAndProfile());

        // Initially hide popup content
        SetContentActive(false);

        if(loadingIndicator != null) loadingIndicator.SetActive(true);
        saveButton.interactable = false;

        saveButton.onClick.AddListener(OnSaveClicked);
        usernameInput.onValueChanged.AddListener(OnUsernameChanged);
        usernameInput.onSelect.AddListener(OnUsernameFocused);
        usernameInput.onDeselect.AddListener(OnUsernameUnfocused);
        usernameInput.characterLimit = 15;
        
        // Ensure status panel starts inactive if not needed
        if (statusPanel != null) statusPanel.SetActive(false);
    }

    private void SetContentActive(bool active)
    {
        if (popupContent == null) return;

        // CRITICAL FIX: If the user assigned THIS GameObject to popupContent, DO NOT deactivate it.
        // Doing so would stop all Coroutines on this script.
        if (popupContent == this.gameObject)
        {
            // Toggle the background Image if present
            Image bgImage = GetComponent<Image>();
            if (bgImage != null) bgImage.enabled = active;

            // Toggle input elements
            if(usernameInput != null) usernameInput.gameObject.SetActive(active);
            if(saveButton != null) saveButton.gameObject.SetActive(active);
            if(avatarContainer != null) avatarContainer.gameObject.SetActive(active);
            
            // Note: We intentionally don't toggle loadingIndicator here as it's controlled separately
            // But we should ensure StatusText follows visibility if desired, or let it stay?
            // Usually Setup Popup hides inputs -> shows loader -> finishes
        }
        else
        {
            popupContent.SetActive(active);
        }
    }

    private void OnUsernameFocused(string value)
    {
        UpdateUsernameStatus(value);
        
        // Mobile UI shift: Move the panel up so keyboard doesn't cover it
        if (popupContent != null)
        {
            RectTransform rt = popupContent.GetComponent<RectTransform>();
            if (rt != null)
            {
                if (!hasStoredPosition)
                {
                    originalContentPosition = rt.anchoredPosition;
                    hasStoredPosition = true;
                }
                
                // Kill any existing tweens to prevent conflicts
                rt.DOKill();
                rt.DOAnchorPos(new Vector2(originalContentPosition.x, originalContentPosition.y + mobileShiftY), 0.35f).SetEase(Ease.OutQuad);
            }
        }
    }

    private void OnUsernameUnfocused(string value)
    {
        // Hide status panel when focus is lost to keep UI clean
        if (statusPanel != null) statusPanel.SetActive(false);
        
        // Return UI to original position
        if (popupContent != null && hasStoredPosition)
        {
            RectTransform rt = popupContent.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.DOKill();
                rt.DOAnchorPos(originalContentPosition, 0.3f).SetEase(Ease.OutQuad);
            }
        }
    }

    private void OnUsernameChanged(string value)
    {
        UpdateUsernameStatus(value);
    }

    private void UpdateUsernameStatus(string value)
    {
        string trimmed = value.Trim();
        bool hasSpace = value.Contains(" ");
        
        // Regex: Only allow alphanumeric, underscore, and hyphen
        bool hasInvalidChars = !Regex.IsMatch(trimmed, @"^[a-zA-Z0-9_-]+$") && !string.IsNullOrEmpty(trimmed);
        
        // Regex: Check if purely numeric
        bool isNumericOnly = Regex.IsMatch(trimmed, @"^\d+$");

        bool isValid = !string.IsNullOrWhiteSpace(trimmed) && 
                       trimmed.Length >= 3 && 
                       !hasSpace && 
                       !hasInvalidChars && 
                       !isNumericOnly;
        
        saveButton.interactable = isValid;

        if (statusPanel != null && statusText != null)
        {
            LanguageComponent langComp = statusText.GetComponent<LanguageComponent>();
            
            if (hasSpace)
            {
                if (langComp != null) langComp.SetText("TXT_ERR_USER_SPACES");
                else statusText.text = "Spaces are not allowed.";
                
                statusPanel.SetActive(true);
            }
            else if (hasInvalidChars)
            {
                // New Error: Invalid characters
                if (langComp != null) langComp.SetText("TXT_ERR_USER_CHARS"); // You might need to add this key
                else statusText.text = "Only letters, numbers, '-' and '_' allowed.";
                
                statusPanel.SetActive(true);
            }
            else if (isNumericOnly)
            {
                // New Error: Numeric only
                if (langComp != null) langComp.SetText("TXT_ERR_USER_NUMERIC"); // You might need to add this key
                else statusText.text = "Username cannot be numbers only.";
                
                statusPanel.SetActive(true);
            }
            else if (string.IsNullOrWhiteSpace(value))
            {
                if (langComp != null) langComp.SetText("TXT_INFO_USER_START");
                else statusText.text = "Please enter a username (at least 3 characters).";
                
                statusPanel.SetActive(true);
            }
            else if (trimmed.Length < 3)
            {
                if (langComp != null) langComp.SetText("TXT_ERR_USER_TOO_SHORT");
                else statusText.text = "Username must be at least 3 characters.";
                
                statusPanel.SetActive(true);
            }
            else
            {
                // If valid, hide the panel as requested
                statusPanel.SetActive(false);
            }
        }
    }

    private IEnumerator CheckFirebaseAndProfile()
    {
        // Wait for Firebase to be initialized
        float timeout = 10f; // 10 second timeout
        float elapsed = 0f;
        
        while (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady())
        {
            elapsed += 0.5f;
            if (elapsed >= timeout)
            {
                Debug.LogError("Timeout waiting for Firebase to be ready. Showing popup anyway.");
                ShowPopup();
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }

        // Additional wait to ensure user is authenticated
        yield return new WaitForSeconds(0.5f);

        // Retry logic: Try up to 3 times to get fresh data from Firebase
        int maxRetries = 3;
        int retryCount = 0;
        bool profileExists = false;
        UserProfile foundProfile = null;

        while (retryCount < maxRetries && !profileExists)
        {
            retryCount++;
            Debug.Log($"Checking Firebase for user profile (Attempt {retryCount}/{maxRetries})...");
            
            bool checkComplete = false;

            FirebaseManager.Instance.GetUserProfile((profile) =>
            {
                foundProfile = profile;
                checkComplete = true;
            });

            // Wait for check to complete with timeout
            float checkTimeout = 5f;
            float checkElapsed = 0f;
            float checkInterval = 0.1f;
            while (!checkComplete && checkElapsed < checkTimeout)
            {
                checkElapsed += checkInterval;
                yield return new WaitForSeconds(checkInterval);
            }

            if (checkComplete)
            {
                profileExists = (foundProfile != null);
                if (profileExists)
                {
                    Debug.Log($"✅ Profile found in Firebase: {foundProfile.username}");
                    break; // Exit retry loop
                }
                else
                {
                    Debug.Log($"ℹ️ No profile found in Firebase (Attempt {retryCount}).");
                    // Wait a bit before retry to ensure cache is cleared
                    if (retryCount < maxRetries)
                    {
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ Firebase check timed out (Attempt {retryCount}). Retrying...");
                yield return new WaitForSeconds(1f);
            }
        }

        // Handle result
        if (profileExists)
        {
            Debug.Log($"✅ Profile exists in Firebase: {foundProfile.username}. Syncing to local data.");
            
            // Sync Firebase data to local PlayerPrefs/GM
            ApplyProfileToLocalData(foundProfile);
            
            PlayerPrefs.SetInt("ProfileCreated", 1);
            if(loadingIndicator != null) loadingIndicator.SetActive(false);
            
            // Hide loading screen before destroying popup
            if (loadingScreen != null)
            {
                loadingScreen.Hide();
            }
            
            Destroy(gameObject); // Clean up immediately if done
        }
        else
        {
            Debug.Log("ℹ️ No profile in Firebase after all retries. Showing setup popup.");
            ShowPopup();
        }
    }

    private void ShowPopup()
    {
        // Clear the local flag if it exists (in case user deleted profile from Firebase)
        PlayerPrefs.DeleteKey("ProfileCreated");
        if(loadingIndicator != null) loadingIndicator.SetActive(false);
        
        // Hide loading screen when showing popup
        if (loadingScreen != null)
        {
            loadingScreen.Hide();
        }
        
        SetContentActive(true);
        InitializeAvatarSelection();
    }

    /// <summary>
    /// Sets the loading screen reference. Called by MainMenuProfileChecker.
    /// </summary>
    public void SetLoadingScreen(LoadingScreen screen)
    {
        loadingScreen = screen;
    }

    public void OnAvatarSelected(int avatarId)
    {
        selectedAvatarId = avatarId;
        Debug.Log($"Avatar {avatarId} selected.");
        
        // Update visual state of all avatar buttons
        foreach (var btn in avatarButtons)
        {
            if (btn != null)
            {
                btn.SetSelected(btn.GetAvatarId() == avatarId);
            }
        }
    }

    /// <summary>
    /// Initializes avatar selection buttons when popup is shown.
    /// Finds all AvatarSelectorButton components in the avatar container.
    /// </summary>
    private void InitializeAvatarSelection()
    {
        if (avatarContainer == null)
        {
            Debug.LogWarning("ProfileSetupPopup: Avatar container not assigned!");
            return;
        }

        // Find all avatar selector buttons
        avatarButtons.Clear();
        avatarButtons.AddRange(avatarContainer.GetComponentsInChildren<AvatarSelectorButton>());

        if (avatarButtons.Count == 0)
        {
            Debug.LogWarning("ProfileSetupPopup: No AvatarSelectorButton components found in avatar container!");
        }
        else
        {
            // Set initial selection (first avatar)
            if (selectedAvatarId >= 0 && selectedAvatarId < availableavatarIds.Count)
            {
                selectedAvatarId = availableavatarIds[0];
            }
            OnAvatarSelected(selectedAvatarId);
            Debug.Log($"ProfileSetupPopup: Initialized {avatarButtons.Count} avatar buttons.");
        }
    }

    private void OnSaveClicked()
    {
        string username = usernameInput.text.Trim();
        bool hasSpace = usernameInput.text.Contains(" ");
        
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || hasSpace)
        {
            if (statusPanel != null && statusText != null)
            {
                LanguageComponent langComp = statusText.GetComponent<LanguageComponent>();
                if (hasSpace)
                {
                    if (langComp != null) langComp.SetText("TXT_ERR_USER_SPACES");
                    else statusText.text = "Spaces are not allowed.";
                }
                else
                {
                    if (langComp != null) langComp.SetText("TXT_ERR_USER_INVALID");
                    else statusText.text = "Invalid username. Please try again.";
                }
                statusPanel.SetActive(true);
            }
            return;
        }

        if (loadingIndicator != null) loadingIndicator.SetActive(true);
        saveButton.interactable = false;

        UserProfile newProfile = new UserProfile(
            FirebaseManager.Instance.CurrentUserId,
            username,
            selectedAvatarId
        );

        newProfile.highScore2D = GM.GetInstance().GetHighScore2D();
        newProfile.highScore3D = GM.GetInstance().GetHighScore3D();
        newProfile.xp = GM.GetInstance().Exp;
        newProfile.tier = GM.GetInstance().GetTierName(newProfile.xp);

        FirebaseManager.Instance.SaveUserProfile(newProfile, (success) =>
        {
            Debug.Log($"SaveUserProfile callback received. Success: {success}");

            if (loadingIndicator != null) loadingIndicator.SetActive(false);
            saveButton.interactable = true;

            if (success)
            {
                Debug.Log("OnSaveClicked: Success path entered.");
                PlayerPrefs.SetInt("ProfileCreated", 1);
                
                // Notify UI to update avatar on the main button
                ProfileButtonAvatarUpdater.NotifyAvatarChanged(selectedAvatarId);
                
                if (statusPanel != null) statusPanel.SetActive(false);

                // Hide loading screen if it was active
                if (loadingScreen != null)
                {
                    loadingScreen.Hide();
                }

                // Destroy the popup completely as we are done
                Destroy(gameObject);
                Debug.Log("ProfileSetupPopup Destroyed.");
            }
            else
            {
                Debug.Log("OnSaveClicked: Failure path entered.");
                if (statusText != null)
                {
                    statusText.text = "Failed to save. Check logs.";
                    statusText.gameObject.SetActive(true);
                }
            }
        });
    }

    //private void OnSaveClicked()
    //{
    //    string username = usernameInput.text;
    //    if (string.IsNullOrEmpty(username)) return;

    //    if(loadingIndicator != null) loadingIndicator.SetActive(true);
    //    saveButton.interactable = false;

    //    UserProfile newProfile = new UserProfile(
    //        FirebaseManager.Instance.CurrentUserId,
    //        username,
    //        selectedAvatarId
    //    );

    //    // Initialize scores from current game state
    //    newProfile.highScore2D = GM.GetInstance().GetHighScore2D();
    //    newProfile.highScore3D = GM.GetInstance().GetHighScore3D();
    //    // newProfile.totalScore = Mathf.Max(newProfile.highScore2D, newProfile.highScore3D);

    //    FirebaseManager.Instance.SaveUserProfile(newProfile, (success) =>
    //    {
    //        Debug.Log($"SaveUserProfile callback received. Success: {success}");

    //        if(loadingIndicator != null) loadingIndicator.SetActive(false);
    //        saveButton.interactable = true;

    //        if (success)
    //        {
    //            Debug.Log("OnSaveClicked: Success path entered.");
    //            PlayerPrefs.SetInt("ProfileCreated", 1);

    //            // Destroy the popup completely as we are done
    //            Destroy(gameObject);
    //            Debug.Log("ProfileSetupPopup Destroyed.");
    //        }
    //        else
    //        {
    //            Debug.Log("OnSaveClicked: Failure path entered.");
    //            if(statusText != null) 
    //            {
    //                statusText.text = "Failed to save. Check logs.";
    //                // Ensure status text is visible if we are in this state
    //                statusText.gameObject.SetActive(true);
    //            }
    //        }
    //    });
    /// <summary>
    /// Applies the retrieved Firebase profile data to the local game state (PlayerPrefs/GM).
    /// </summary>
    private void ApplyProfileToLocalData(UserProfile profile)
    {
        if (profile == null) return;

        // Sync XP
        if (profile.xp > 0)
        {
            GM.GetInstance().Exp = profile.xp;
            PlayerPrefs.SetFloat("LocalData_Exp", profile.xp);
        }

        // Sync High Scores
        if (profile.highScore2D > 0)
        {
            // Set directly to PlayerPrefs to avoid redundant Firebase sync from SaveHighScore2D
            string[] scores = PlayerPrefs.GetString("LocalData_Record_Score", "0,0,0").Split(',');
            if (scores.Length >= 2)
            {
                scores[1] = profile.highScore2D.ToString();
                PlayerPrefs.SetString("LocalData_Record_Score", string.Join(",", scores));
            }
        }

        if (profile.highScore3D > 0)
        {
            // Set directly to PlayerPrefs to avoid redundant Firebase sync from SaveHighScore3D
            PlayerPrefs.SetInt("heightScoreGet", profile.highScore3D);
        }

        // Sync Profile flag
        PlayerPrefs.SetInt("ProfileCreated", 1);
        
        // Sync Avatar ID and notify UI
        PlayerPrefs.SetInt("UserAvatarId", profile.avatarId);
        ProfileButtonAvatarUpdater.NotifyAvatarChanged(profile.avatarId);
        
        PlayerPrefs.Save();
        
        Debug.Log($"[ProfileSetupPopup] Synced profile for {profile.username}: XP {profile.xp}");
    }
}
