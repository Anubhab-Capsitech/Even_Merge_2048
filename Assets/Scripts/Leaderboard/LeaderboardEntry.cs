using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a single entry in the leaderboard.
/// Displays rank, avatar, username, and score.
/// </summary>
public class LeaderboardEntry : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image badgeImage; // New: For Medals (Gold/Silver/Bronze)
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI tierText; // New: For "A++" etc
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private Image avatarImage;

    [SerializeField] private LeaderboardAssets assets; // Direct reference

    public void Setup(UserProfile profile, int rank, float xp, LeaderboardAssets passedAssets, bool isCurrentUser)
    {
        // Use passed assets if available, otherwise fallback to local assets
        LeaderboardAssets assetsToUse = passedAssets != null ? passedAssets : this.assets;

        // 1. Set Basic Text
        if (usernameText != null) usernameText.text = !string.IsNullOrEmpty(profile.username) ? profile.username : "Guest";
        if (xpText != null) xpText.text = $"XP({xp:F2})";
        
        // 2. Set Rank Visuals
        if (rankText != null) rankText.text = rank.ToString();

        // 3. Set Assets (Backgrounds, Medals, Avatars)
        if (assetsToUse != null)
        {
            // Background
            if (backgroundImage != null)
            {
                if (rank == 1) backgroundImage.sprite = assetsToUse.rank1Bg;
                else if (rank == 2) backgroundImage.sprite = assetsToUse.rank2Bg;
                else backgroundImage.sprite = assetsToUse.defaultBg;
            }

            // Badge (Medal)
            if (badgeImage != null)
            {
                if (rank == 1) badgeImage.sprite = assetsToUse.medalGold;
                else if (rank == 2) badgeImage.sprite = assetsToUse.medalSilver;
                else if (rank == 3) badgeImage.sprite = assetsToUse.medalBronze;
                else badgeImage.sprite = assetsToUse.medalGeneric;
            }
            else
            {
                // Warn only once to avoid spam (checking rank 1)
                if (rank == 1) Debug.LogError("LeaderboardEntry: 'Badge Image' is not assigned in the Inspector!");
            }

            // Avatar (Handle Crown for Rank 1)
            if (avatarImage != null)
            {
                avatarImage.sprite = assetsToUse.GetAvatarSprite(profile.avatarId, rank);
            }
        }
        else
        {
            Debug.LogError($"LeaderboardEntry: 'Assets' is NULL! Assign LeaderboardAssetsConfig to the LeaderboardEntry prefab OR the LeaderboardPopup script.");
        }

        // 4. Set Tier Stamp
        if (tierText != null)
        {
            tierText.text = XPUtils.GetTierFromXP(xp);
        }
    }

    // Deprecated methods kept to avoid strict compiler errors if referenced elsewhere (unlikely but safe)
    public void SetAvatarSprites(Sprite[] sprites) { }
    public void SetDefaultAvatarSprite(Sprite sprite) { }
}
