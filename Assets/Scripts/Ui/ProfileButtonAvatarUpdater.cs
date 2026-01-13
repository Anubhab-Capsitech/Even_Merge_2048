using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.GameManager;
using System;

/// <summary>
/// Updates the profile button's image to display the user's selected avatar.
/// Fetches avatar ID from Firebase and updates the button sprite accordingly.
/// Attach this to your Profile Button GameObject.
/// </summary>
public class ProfileButtonAvatarUpdater : MonoBehaviour
{
    [Header("Avatar Sprites")]
    [Tooltip("Array of avatar sprites. Index should match avatarId (0-5)")]
    [SerializeField] private Sprite[] avatarSprites = new Sprite[6];

    [Header("Default Settings")]
    [SerializeField] private Sprite defaultAvatarSprite;
    [Tooltip("Time to wait before retrying if Firebase isn't ready (seconds)")]
    [SerializeField] private float retryDelay = 1f;
    [Tooltip("Maximum number of retry attempts")]
    [SerializeField] private int maxRetries = 5;

    private Image buttonImage;
    private int currentAvatarId = -1;

    // Static event that ProfileSetupPopup can trigger
    public static event Action<int> OnProfileAvatarChanged;

    private void Awake()
    {
        // Get the Image component from this button
        buttonImage = GetComponent<Image>();

        if (buttonImage == null)
        {
            Debug.LogError("ProfileButtonAvatarUpdater: No Image component found on this GameObject!");
            return;
        }

        // Set default avatar initially
        if (defaultAvatarSprite != null)
        {
            buttonImage.sprite = defaultAvatarSprite;
        }
    }

    private void OnEnable()
    {
        // Subscribe to profile update events
        OnProfileAvatarChanged += HandleAvatarChanged;

        // Load avatar when button becomes active
        if (buttonImage != null)
        {
            RefreshAvatar();
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        OnProfileAvatarChanged -= HandleAvatarChanged;
    }

    private void Start()
    {
        StartCoroutine(LoadAndUpdateAvatar());
    }

    /// <summary>
    /// Call this static method when profile is saved to notify all listeners.
    /// </summary>
    public static void NotifyAvatarChanged(int avatarId)
    {
        Debug.Log($"ProfileButtonAvatarUpdater: Broadcasting avatar change to {avatarId}");
        OnProfileAvatarChanged?.Invoke(avatarId);
    }

    /// <summary>
    /// Handles the avatar changed event - updates immediately without Firebase fetch.
    /// </summary>
    private void HandleAvatarChanged(int avatarId)
    {
        Debug.Log($"ProfileButtonAvatarUpdater: Received avatar change event for ID {avatarId}");
        UpdateAvatarImage(avatarId);

        // Also save to PlayerPrefs for quick access
        PlayerPrefs.SetInt("UserAvatarId", avatarId);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Call this method to manually refresh the avatar image from Firebase.
    /// </summary>
    public void RefreshAvatar()
    {
        // First try to load from PlayerPrefs for instant display
        if (PlayerPrefs.HasKey("UserAvatarId"))
        {
            int cachedAvatarId = PlayerPrefs.GetInt("UserAvatarId");
            UpdateAvatarImage(cachedAvatarId);
            Debug.Log($"ProfileButtonAvatarUpdater: Loaded avatar {cachedAvatarId} from PlayerPrefs cache");
        }

        // Then load from Firebase in background to ensure it's up to date
        StartCoroutine(LoadAndUpdateAvatar());
    }

    private IEnumerator LoadAndUpdateAvatar()
    {
        int retryCount = 0;

        // Wait for Firebase to be ready
        while (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady())
        {
            retryCount++;
            if (retryCount >= maxRetries)
            {
                Debug.LogWarning("ProfileButtonAvatarUpdater: Firebase not ready after max retries. Using cached/default avatar.");
                yield break;
            }
            yield return new WaitForSeconds(retryDelay);
        }

        // Fetch user profile from Firebase
        bool fetchComplete = false;
        UserProfile profile = null;

        FirebaseManager.Instance.GetUserProfile((fetchedProfile) =>
        {
            profile = fetchedProfile;
            fetchComplete = true;
        });

        // Wait for fetch to complete
        float timeout = 5f;
        float elapsed = 0f;
        while (!fetchComplete && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!fetchComplete)
        {
            Debug.LogWarning("ProfileButtonAvatarUpdater: Timeout fetching profile data.");
            yield break;
        }

        if (profile != null)
        {
            UpdateAvatarImage(profile.avatarId);
            // Update PlayerPrefs cache
            PlayerPrefs.SetInt("UserAvatarId", profile.avatarId);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("ProfileButtonAvatarUpdater: No profile found. Using cached/default avatar.");
        }
    }

    /// <summary>
    /// Updates the button's image to display the specified avatar.
    /// </summary>
    /// <param name="avatarId">The avatar ID (0-5)</param>
    private void UpdateAvatarImage(int avatarId)
    {
        if (buttonImage == null) return;

        // Validate avatar ID
        if (avatarId >= 0 && avatarId < avatarSprites.Length && avatarSprites[avatarId] != null)
        {
            buttonImage.sprite = avatarSprites[avatarId];
            currentAvatarId = avatarId;
            Debug.Log($"ProfileButtonAvatarUpdater: Updated button to avatar {avatarId}");
        }
        else if (defaultAvatarSprite != null)
        {
            buttonImage.sprite = defaultAvatarSprite;
            Debug.LogWarning($"ProfileButtonAvatarUpdater: Invalid avatar ID {avatarId}. Using default.");
        }
        else
        {
            Debug.LogError($"ProfileButtonAvatarUpdater: Invalid avatar ID {avatarId} and no default sprite set!");
        }
    }

    /// <summary>
    /// Gets the currently displayed avatar ID.
    /// </summary>
    public int GetCurrentAvatarId()
    {
        return currentAvatarId;
    }
}