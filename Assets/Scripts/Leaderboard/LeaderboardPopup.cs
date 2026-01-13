using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GameManager;

/// <summary>
/// Leaderboard popup with a single leaderboard list.
/// Fetches data from Firebase and displays ranked list of players.
/// </summary>
public class LeaderboardPopup : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private LeaderboardAssets assets; // New: Centralized assets
    [SerializeField] private int maxLeaderboardEntries = 50;

    [Header("UI References")]
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private Transform listContainer; // Was 'content'
    
    [Header("Leaderboard Entry")]
    [SerializeField] private GameObject leaderboardEntryPrefab;

    [Header("User Footer")]
    [SerializeField] private LeaderboardEntry currentUserEntry; // The footer item

    [Header("Close Button")]
    [SerializeField] private Button closeButton;

    [Header("Loading & Error")]
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI emptyText;

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }
        
        // Ensure scroll content is set up correctly for scrolling
        EnsureScrollContentSetup(listContainer);

        RefreshLeaderboard();
    }

    private void EnsureScrollContentSetup(Transform content)
    {
        if (content == null) return;
        
        // CRITICAL: Force the Content RectTransform to anchor to the TOP of the Scroll View
        RectTransform rt = content.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.pivot = new Vector2(0.5f, 1f); // Pivot at Top-Center
            rt.anchorMin = new Vector2(0f, 1f); // Anchor Min Top-Left
            rt.anchorMax = new Vector2(1f, 1f); // Anchor Max Top-Right
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f); // Reset Y position to top
            rt.sizeDelta = new Vector2(0f, 0f); // Reset size delta to stretch horizontally
        }

        // Ensure VerticalLayoutGroup exists
        VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
        {
            vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        // Force layout settings to ensure "Top to Bottom" filling with no expanding gaps
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlHeight = false; // Let children define their height
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false; // CRITICAL: Prevent stretching to fill height
        vlg.childForceExpandWidth = true;
        vlg.spacing = 35.0f; // Increased Spacing
        vlg.padding = new RectOffset(70, 50, 50, 50); // Top, Left, and Right Padding added

        // Ensure ContentSizeFitter exists
        ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
        if (csf == null)
        {
            csf = content.gameObject.AddComponent<ContentSizeFitter>();
        }
        
        // Critical for scrolling: Vertical Fit must be Preferred Size so the container expands
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public void RefreshLeaderboard()
    {
        ShowLoading(true);
        ShowError(false);
        ShowEmpty(false);

        // Clear existing items
        if (listContainer != null)
        {
            foreach (Transform child in listContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Fetch Data - pass 0 to get the full list for rank calculation
        FirebaseManager.Instance.GetLeaderboard("3D", 0, (leaderboard) =>
        {
            if (leaderboard == null)
            {
                ShowError(true);
                ShowLoading(false);
                return;
            }

            if (leaderboard.Count == 0)
            {
                ShowEmpty(true);
                ShowLoading(false);
                // Even with empty list, show user footer
                UpdateUserFooter(new List<UserProfile>()); 
                return;
            }

            // Populate List (Limited to Top N entries for performance)
            int displayCount = Mathf.Min(leaderboard.Count, maxLeaderboardEntries);
            for (int i = 0; i < displayCount; i++)
            {
                UserProfile profile = leaderboard[i];
                int rank = i + 1;

                if (leaderboardEntryPrefab != null && listContainer != null)
                {
                    GameObject obj = Instantiate(leaderboardEntryPrefab, listContainer);
                    LeaderboardEntry item = obj.GetComponent<LeaderboardEntry>();
                    if (item != null)
                    {
                        item.Setup(profile, rank, profile.xp, assets, false);
                    }
                }
            }

            // Update Footer (Me) - uses the full list to find the correct rank
            UpdateUserFooter(leaderboard);

            ShowLoading(false);
        });
    }

    private void UpdateUserFooter(List<UserProfile> leaderboard)
    {
        if (currentUserEntry == null) return;

        string myId = FirebaseManager.Instance.CurrentUserId;
        UserProfile myProfile = null;
        int myRank = -1;

        // Find self in leaderboard
        for (int i = 0; i < leaderboard.Count; i++)
        {
            if (leaderboard[i].userId == myId)
            {
                myProfile = leaderboard[i];
                myRank = i + 1;
                break;
            }
        }

        // If not in top N, fetch self directly or use local data
        if (myProfile == null)
        {
            // Fallback: Use local data
             myProfile = new UserProfile(myId, "You", 0); 
             // Ideally we get real data, but for now:
             myProfile.highScore3D = GM.GetInstance().GetHighScore3D();
             myProfile.xp = GM.GetInstance().Exp; 
             // Or fetch profile separately if needed, but keeping it simple for now
             myRank = 999; // Unknown
        }

        // Always use the latest local XP if it's higher than what we fetched (covers immediate level-ups)
        float latestLocalXP = GM.GetInstance().Exp;
        float displayXP = Mathf.Max(myProfile.xp, latestLocalXP);
        
        currentUserEntry.Setup(myProfile, myRank, displayXP, assets, true);
    }

    private void ShowLoading(bool show)
    {
        if (loadingIndicator != null) loadingIndicator.SetActive(show);
    }

    private void ShowError(bool show)
    {
        if (errorText != null) errorText.gameObject.SetActive(show);
    }

    private void ShowEmpty(bool show)
    {
        if (emptyText != null) emptyText.gameObject.SetActive(show);
    }

    private void OnCloseClicked()
    {
        if (PersistentAudioManager.Instance != null)
        {
            PersistentAudioManager.Instance.PlayEffect("sound_eff_button");
        }

        if (DialogManager.GetInstance() != null)
        {
            DialogManager.GetInstance().Close(null);
        }
        else
        {
            Destroy(gameObject);
        }

        // Resume the game when leaderboard dialog is closed (for 2D game)
        Time.timeScale = 1f;
        if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
        {
            G2BoardGenerator.GetInstance().IsPuase = false;
        }
    }
}
