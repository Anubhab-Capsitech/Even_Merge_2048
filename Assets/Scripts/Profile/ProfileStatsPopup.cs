using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Assets.Scripts.GameManager;

/// <summary>
/// Displays user profile statistics fetched from Firebase.
/// Shows name, avatar, score, level, and other stored data.
/// </summary>
public class ProfileStatsPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private Image avatarImage;
    // [SerializeField] private TextMeshProUGUI scoreText; // Kept for backward compatibility (shows max of 2D/3D)
    [SerializeField] private TextMeshProUGUI highScore2DText; // High score for 2D game
    [SerializeField] private TextMeshProUGUI highScore3DText; // High score for 3D game
    [SerializeField] private TextMeshProUGUI xpText; // New: XP Display
    [SerializeField] private TextMeshProUGUI tierText; // New: Tier Display ("A++")
    //[SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI creationDateText;



    [SerializeField] private Button closeButton;
    [SerializeField] private Button changeAvatarButton; // New: Button to open avatar change popup
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Panel Prefabs")]
    [SerializeField] private GameObject avatarChangePopupPrefab; // New: Reference to the avatar change popup prefab

    [Header("Avatar Sprites")]
    [Tooltip("Array of avatar sprites. Index should match avatarId (0-5)")]
    [SerializeField] private Sprite[] avatarSprites = new Sprite[6];

    [Header("Default Values")]
    [SerializeField] private Sprite defaultAvatarSprite;
    [SerializeField] private string defaultUsername = "Guest";
    [SerializeField] private string loadingText = "Loading...";
    [SerializeField] private string errorMessage = "Failed to load profile data.";

    private UserProfile currentProfile;

    private void Start()
    {
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        // Setup change avatar button
        if (changeAvatarButton != null)
        {
            changeAvatarButton.onClick.AddListener(OnChangeAvatarClicked);
        }

        // Initially show loading
        ShowLoading(true);
        ShowError(false);

        // Fetch profile data
        StartCoroutine(LoadProfileData());
    }

    private IEnumerator LoadProfileData()
    {
        // Wait for Firebase to be ready
        float timeout = 10f;
        float elapsed = 0f;

        while (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady())
        {
            elapsed += 0.5f;
            if (elapsed >= timeout)
            {
                ShowError(true);
                ShowLoading(false);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }

        // Fetch profile from Firebase
        bool fetchComplete = false;
        UserProfile profile = null;
        bool fetchSuccess = false;

        FirebaseManager.Instance.GetUserProfile((fetchedProfile) =>
        {
            profile = fetchedProfile;
            fetchComplete = true;
            fetchSuccess = (fetchedProfile != null);
        });

        // Wait for fetch to complete
        float fetchTimeout = 5f;
        float fetchElapsed = 0f;
        while (!fetchComplete && fetchElapsed < fetchTimeout)
        {
            fetchElapsed += Time.deltaTime;
            yield return null;
        }

        if (!fetchComplete)
        {
            Debug.LogError("ProfileStatsPopup: Timeout fetching profile data.");
            ShowError(true);
            ShowLoading(false);
            yield break;
        }

        if (fetchSuccess && profile != null)
        {
            currentProfile = profile;
            DisplayProfile(profile);
        }
        else
        {
            Debug.LogWarning("ProfileStatsPopup: No profile found or fetch failed.");
            ShowError(true);
            ShowLoading(false);
        }
    }

    private void DisplayProfile(UserProfile profile)
    {
        ShowLoading(false);
        ShowError(false);

        // Display username
        if (usernameText != null)
        {
            usernameText.text = !string.IsNullOrEmpty(profile.username) ? profile.username : defaultUsername;
        }

        // Display avatar
        if (avatarImage != null)
        {
            if (profile.avatarId >= 0 && profile.avatarId < avatarSprites.Length && avatarSprites[profile.avatarId] != null)
            {
                avatarImage.sprite = avatarSprites[profile.avatarId];
            }
            else if (defaultAvatarSprite != null)
            {
                avatarImage.sprite = defaultAvatarSprite;
            }
        }

        // Display scores - show both 2D and 3D separately
        if (highScore2DText != null)
        {
            highScore2DText.text = profile.highScore2D.ToString("N0"); // Format with commas
        }

        if (highScore3DText != null)
        {
            highScore3DText.text = profile.highScore3D.ToString("N0"); // Format with commas
        }

        // Display XP
        if (xpText != null)
        {
            xpText.text = $"XP ({profile.xp:F2})";
        }

        // Display Tier (Formerly Level)
        if (tierText != null)
        {
            tierText.text = XPUtils.GetTierFromXP(profile.xp);
        }

        // // Display total score (backward compatibility - shows max of 2D/3D)
        // if (scoreText != null)
        // {
        //     int maxScore = Mathf.Max(profile.highScore2D, profile.highScore3D);
        //     scoreText.text = maxScore.ToString("N0"); // Format with commas
        // }

        // Display Tier (Formerly Level)
        //if (levelText != null)
        //{
        //    levelText.text = !string.IsNullOrEmpty(profile.tier) ? profile.tier : GM.GetInstance().GetTierName(profile.xp);
        //}

        // Display creation date
        if (creationDateText != null)
        {
            if (!string.IsNullOrEmpty(profile.creationDate))
            {
                // Try to parse and format the date
                if (System.DateTime.TryParse(profile.creationDate, out System.DateTime date))
                {
                    creationDateText.text = date.ToString("MMM dd, yyyy");
                }
                else
                {
                    creationDateText.text = profile.creationDate;
                }
            }
            else
            {
                creationDateText.text = "N/A";
            }
        }

        Debug.Log($"ProfileStatsPopup: Displayed profile for {profile.username}");
    }

    private void ShowLoading(bool show)
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(show);
        }
    }

    private void ShowError(bool show)
    {
        if (errorText != null)
        {
            errorText.gameObject.SetActive(show);
            if (show)
            {
                errorText.text = errorMessage;
            }
        }
    }

    private void OnCloseClicked()
    {
        if (PersistentAudioManager.Instance != null)
        {
            PersistentAudioManager.Instance.PlayClickSound();
        }

        DialogManager.GetInstance().Close(null);
        
        // Resume the game when profile dialog is closed (for 2D game)
        // This fixes the camera/viewport issue where game appears distant after closing popup
        Time.timeScale = 1f;
        if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
        {
            G2BoardGenerator.GetInstance().IsPuase = false;
        }
    }

    /// <summary>
    /// Refreshes the profile data from Firebase.
    /// Can be called by a refresh button if needed.
    /// </summary>
    public void RefreshProfile()
    {
        ShowLoading(true);
        ShowError(false);
        StartCoroutine(LoadProfileData());
    }

    private void OnChangeAvatarClicked()
    {
        if (avatarChangePopupPrefab == null)
        {
            Debug.LogError("ProfileStatsPopup: Avatar change popup prefab not assigned!");
            return;
        }

        if (PersistentAudioManager.Instance != null)
        {
            PersistentAudioManager.Instance.PlayClickSound();
        }

        // Open the avatar change popup
        GameObject popupObj = Instantiate(avatarChangePopupPrefab);
        DialogManager.GetInstance().show(popupObj, false);

        if (popupObj != null)
        {
            AvatarChangePopup changePopup = popupObj.GetComponent<AvatarChangePopup>();
            if (changePopup != null)
            {
                int currentId = currentProfile != null ? currentProfile.avatarId : 0;
                changePopup.Initialize(currentId, (newId) =>
                {
                    // Update UI immediately when avatar is changed
                    if (currentProfile != null)
                    {
                        currentProfile.avatarId = newId;
                        RefreshAvatarUI(newId);
                    }
                });
            }
        }
    }

    private void RefreshAvatarUI(int avatarId)
    {
        if (avatarImage != null)
        {
            if (avatarId >= 0 && avatarId < avatarSprites.Length && avatarSprites[avatarId] != null)
            {
                avatarImage.sprite = avatarSprites[avatarId];
            }
            else if (defaultAvatarSprite != null)
            {
                avatarImage.sprite = defaultAvatarSprite;
            }
        }
    }
}

