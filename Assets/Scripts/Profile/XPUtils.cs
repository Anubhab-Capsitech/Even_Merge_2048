using UnityEngine;

public static class XPUtils
{
    // Tier Thresholds (Synced with GM.cs)
    public const float TIER_LIMIT_C = 40f;
    public const float TIER_LIMIT_B = 120f;
    public const float TIER_LIMIT_B_PLUS = 300f;
    public const float TIER_LIMIT_A = 650f;
    public const float TIER_LIMIT_A_PLUS = 1100f;

    /// <summary>
    /// Returns the Tier string (e.g., "A++", "B") based on XP value.
    /// </summary>
    public static string GetTierFromXP(float xp)
    {
        if (xp <= TIER_LIMIT_C) return "C";
        if (xp <= TIER_LIMIT_B) return "B";
        if (xp <= TIER_LIMIT_B_PLUS) return "B+";
        if (xp <= TIER_LIMIT_A) return "A";
        if (xp <= TIER_LIMIT_A_PLUS) return "A+";
        return "A++"; // 1101+
    }

    public static int GetTierIndex(float xp)
    {
        if (xp <= TIER_LIMIT_C) return 1;
        if (xp <= TIER_LIMIT_B) return 2;
        if (xp <= TIER_LIMIT_B_PLUS) return 3;
        if (xp <= TIER_LIMIT_A) return 4;
        if (xp <= TIER_LIMIT_A_PLUS) return 5;
        return 6;
    }

    public static float GetTierProgress(float xp)
    {
        if (xp <= TIER_LIMIT_C) 
            return Mathf.Clamp01(xp / TIER_LIMIT_C);
        
        if (xp <= TIER_LIMIT_B) 
            return Mathf.Clamp01((xp - TIER_LIMIT_C) / (TIER_LIMIT_B - TIER_LIMIT_C));
        
        if (xp <= TIER_LIMIT_B_PLUS) 
            return Mathf.Clamp01((xp - TIER_LIMIT_B) / (TIER_LIMIT_B_PLUS - TIER_LIMIT_B));
        
        if (xp <= TIER_LIMIT_A) 
            return Mathf.Clamp01((xp - TIER_LIMIT_B_PLUS) / (TIER_LIMIT_A - TIER_LIMIT_B_PLUS));

        if (xp <= TIER_LIMIT_A_PLUS)
            return Mathf.Clamp01((xp - TIER_LIMIT_A) / (TIER_LIMIT_A_PLUS - TIER_LIMIT_A));

        return 1.0f; 
    }
}
