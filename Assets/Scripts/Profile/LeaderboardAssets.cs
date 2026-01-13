using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LeaderboardAssets", menuName = "ScriptableObjects/LeaderboardAssets", order = 1)]
public class LeaderboardAssets : ScriptableObject
{
    [Header("Row Backgrounds")]
    public Sprite rank1Bg;
    public Sprite rank2Bg;
    public Sprite defaultBg; // Rank 3+

    [Header("Medals")]
    public Sprite medalGold;   // Rank 1
    public Sprite medalSilver; // Rank 2
    public Sprite medalBronze; // Rank 3
    public Sprite medalGeneric; // Rank 4+

    [Header("Avatars")]
    [Tooltip("List of avatars with CROWNS (index must match normal avatar ID)")]
    public List<Sprite> crownedAvatars;
    
    [Tooltip("List of normal avatars (for reference/fallback)")]
    public List<Sprite> normalAvatars;

    public Sprite GetAvatarSprite(int avatarId, int rank)
    {
        // Rank 1 gets Crowned avatar if available
        if (rank == 1 && avatarId >= 0 && avatarId < crownedAvatars.Count)
        {
            return crownedAvatars[avatarId];
        }

        // Fallback or normal rank
        if (avatarId >= 0 && avatarId < normalAvatars.Count)
        {
            return normalAvatars[avatarId];
        }

        return null;
    }
}
