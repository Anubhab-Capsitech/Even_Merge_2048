using System;

[Serializable]
public class UserProfile
{
    public string userId;
    public string username;
    public int avatarId;
    public string creationDate;
    // public int totalScore; // Kept for backward compatibility, but deprecated - use highScore2D and highScore3D instead
    public int highScore2D; // High score for 2D game version
    public int highScore3D; // High score for 3D game version
    public float xp; // XP value for leaderboard ranking
    public string tier; // Current Tier (Bronze, Silver, Gold, Platinum, Ruby)

    public UserProfile() { }

    public UserProfile(string userId, string username, int avatarId)
    {
        this.userId = userId;
        this.username = username;
        this.avatarId = avatarId;
        this.creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        // this.totalScore = 0;
        this.highScore2D = 0;
        this.highScore3D = 0;
        this.xp = 0f;
        this.tier = "C";
    }
}
