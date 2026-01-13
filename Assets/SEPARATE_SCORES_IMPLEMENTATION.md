# Separate 2D and 3D High Scores Implementation

## Overview
This implementation separates high scores for 2D and 3D game versions, storing them independently in both PlayerPrefs and Firebase database. The ProfileStatsPopup now displays both scores separately.

## Changes Made

### 1. UserProfile.cs
- Added `highScore2D` field for 2D game high score
- Added `highScore3D` field for 3D game high score
- Kept `totalScore` for backward compatibility (automatically set to max of 2D/3D)
- Both new fields default to 0 in constructor

### 2. GM.cs (GameManager)
- **New Methods:**
  - `GetHighScore2D()` - Returns 2D game high score from PlayerPrefs
  - `GetHighScore3D()` - Returns 3D game high score from PlayerPrefs
  - `SaveHighScore2D(int score)` - Saves 2D high score to PlayerPrefs and Firebase
  - `SaveHighScore3D(int score)` - Saves 3D high score to PlayerPrefs and Firebase
  - `SaveHighScoresToFirebase()` - Saves both scores and level to Firebase

- **Updated Methods:**
  - `SaveScoreRecord()` - Now calls `SaveHighScore2D()` for gameID == 2
  - `SaveUnifiedHighScore()` - Deprecated but kept for compatibility, redirects to appropriate method based on GameId
  - `GetUnifiedHighScore()` - Still works, returns max of 2D and 3D scores

- **Firebase Integration:**
  - `SaveHighScoresToFirebase()` saves both `highScore2D` and `highScore3D` to Firebase
  - Also saves `currentLevel` from GM.Lv to Firebase
  - Updates `totalScore` for backward compatibility

### 3. HeightScore.cs (3D Game Score Manager)
- Updated `LoadProgress()` to load 3D-specific high score using `GetHighScore3D()`
- Updated `GameOver()` to save using `SaveHighScore3D()` instead of unified score
- Updated `OnApplicationQuit()` to save using `SaveHighScore3D()`

### 4. ProfileStatsPopup.cs
- Added new UI fields:
  - `highScore2DText` - TextMeshProUGUI for displaying 2D high score
  - `highScore3DText` - TextMeshProUGUI for displaying 3D high score
- Updated `DisplayProfile()` to show both scores separately
- Kept `scoreText` for backward compatibility (shows max of 2D/3D)
- Level display remains unchanged (fetches from Firebase `currentLevel`)

### 5. ProfileSetupPopup.cs
- Updated profile creation to initialize scores and level from current game state:
  - `highScore2D` from `GM.GetHighScore2D()`
  - `highScore3D` from `GM.GetHighScore3D()`
  - `currentLevel` from `GM.Lv`
  - `totalScore` as max of both scores

## PlayerPrefs Keys Used

### 2D Game:
- `LocalData_Record_Score` - Comma-separated string containing scores for games 1, 2, 3
  - Index 1 (gameID 2) contains 2D high score

### 3D Game:
- `heightScoreGet` - Integer containing 3D high score

## Firebase Database Structure

The user profile in Firebase now includes:
```json
{
  "userId": "...",
  "username": "...",
  "avatarId": 0,
  "creationDate": "...",
  "totalScore": 0,        // Max of highScore2D and highScore3D (backward compatibility)
  "currentLevel": 1,     // Player level from GM
  "highScore2D": 0,      // 2D game high score
  "highScore3D": 0       // 3D game high score
}
```

## Unity Editor Setup Required

### 1. ProfileStatsPopup Setup

In the Unity Editor, open the scene containing the ProfileStatsPopup:

1. **Locate ProfileStatsPopup GameObject**
   - Find the GameObject with `ProfileStatsPopup` component attached
   - This is typically in the "MainMenu 1" scene

2. **Assign New UI Text Fields**
   - Select the ProfileStatsPopup GameObject
   - In the Inspector, find the `ProfileStatsPopup` component
   - You'll see the following fields:
     - `Username Text` - Already assigned
     - `Avatar Image` - Already assigned
     - `Score Text` - Already assigned (shows max score, kept for compatibility)
     - **`High Score 2D Text`** - **NEW FIELD** - Assign a TextMeshProUGUI component for 2D high score display
     - **`High Score 3D Text`** - **NEW FIELD** - Assign a TextMeshProUGUI component for 3D high score display
     - `Level Text` - Already assigned
     - `Creation Date Text` - Already assigned
     - `Close Button` - Already assigned
     - `Loading Indicator` - Already assigned
     - `Error Text` - Already assigned

3. **Create UI Elements (if not already present)**
   - If you don't have separate text fields for 2D and 3D scores:
     - Create two new TextMeshProUGUI elements in your ProfileStatsPopup UI
     - Name them appropriately (e.g., "HighScore2DText", "HighScore3DText")
     - Position them where you want the scores to appear
     - Assign them to the `High Score 2D Text` and `High Score 3D Text` fields in the Inspector

4. **UI Layout Suggestions**
   - You might want to add labels like "2D High Score:" and "3D High Score:" next to the text fields
   - Consider arranging them vertically or horizontally based on your UI design
   - Example layout:
     ```
     Username: [username]
     Avatar: [avatar image]
     2D High Score: [highScore2DText]
     3D High Score: [highScore3DText]
     Level: [levelText]
     Created: [creationDateText]
     ```

### 2. Verify Scene Names

Ensure your scenes are named correctly:
- **Main Menu & 2D Game Scene:** `MainMenu 1`
- **3D Game Scene:** `scene`

These names are used by the game logic to determine which game mode is active.

### 3. Testing Checklist

After setup, test the following:

1. **2D Game Score Saving:**
   - Play the 2D game and achieve a high score
   - Verify the score is saved to PlayerPrefs key `LocalData_Record_Score` (index 1)
   - Check Firebase database - `highScore2D` should be updated
   - Open ProfileStatsPopup - 2D score should display correctly

2. **3D Game Score Saving:**
   - Play the 3D game and achieve a high score
   - Verify the score is saved to PlayerPrefs key `heightScoreGet`
   - Check Firebase database - `highScore3D` should be updated
   - Open ProfileStatsPopup - 3D score should display correctly

3. **ProfileStatsPopup Display:**
   - Open ProfileStatsPopup from main menu
   - Verify both 2D and 3D high scores are displayed separately
   - Verify level is displayed correctly
   - Verify scores are fetched from Firebase (not just PlayerPrefs)

4. **Level Data:**
   - Verify that player level (`GM.Lv`) is saved to Firebase `currentLevel`
   - Verify that level is displayed correctly in ProfileStatsPopup
   - Level should update when player levels up

5. **Backward Compatibility:**
   - Existing profiles should still work
   - `totalScore` field is maintained for backward compatibility
   - Old code using `SaveUnifiedHighScore()` still works but is deprecated

### 4. Firebase Database Rules

Ensure your Firebase Realtime Database rules allow reading and writing user profiles:

```json
{
  "rules": {
    "users": {
      "$userId": {
        ".read": "$userId === auth.uid",
        ".write": "$userId === auth.uid"
      }
    }
  }
}
```

### 5. Migration Notes

- **Existing Users:** When an existing user's profile is loaded, if `highScore2D` or `highScore3D` are 0 (default), the system will try to migrate from `totalScore` if available
- **New Users:** New profiles are created with both `highScore2D` and `highScore3D` initialized from current PlayerPrefs values

## Code Flow

### Saving 2D High Score:
1. `Game2Manager` or `G2BoardGenerator` calls `GM.SaveScoreRecord(2, score)`
2. `GM.SaveScoreRecord()` calls `GM.SaveHighScore2D(score)`
3. `GM.SaveHighScore2D()` saves to PlayerPrefs and calls `SaveHighScoresToFirebase()`
4. Firebase profile is updated with new `highScore2D` value

### Saving 3D High Score:
1. `HeightScore.GameOver()` or `HeightScore.OnApplicationQuit()` calls `GM.SaveHighScore3D(score)`
2. `GM.SaveHighScore3D()` saves to PlayerPrefs and calls `SaveHighScoresToFirebase()`
3. Firebase profile is updated with new `highScore3D` value

### Displaying Scores in ProfileStatsPopup:
1. `ProfileStatsPopup.LoadProfileData()` fetches profile from Firebase
2. `ProfileStatsPopup.DisplayProfile()` displays:
   - `profile.highScore2D` → `highScore2DText`
   - `profile.highScore3D` → `highScore3DText`
   - `profile.currentLevel` → `levelText`
   - Max of both scores → `scoreText` (backward compatibility)

## Troubleshooting

### Scores not showing in ProfileStatsPopup:
- Verify UI text fields are assigned in Inspector
- Check Firebase database to ensure scores are saved
- Check Unity console for Firebase errors
- Verify FirebaseManager is initialized before ProfileStatsPopup loads

### Scores not saving to Firebase:
- Check Firebase authentication status
- Verify Firebase database rules allow writes
- Check Unity console for Firebase save errors
- Ensure `FirebaseManager.Instance` is not null

### Level not updating:
- Verify `GM.Lv` is being updated when player levels up
- Check that `SaveHighScoresToFirebase()` is called after level changes
- Verify `currentLevel` field exists in Firebase profile

## Notes

- The `totalScore` field is maintained for backward compatibility but is automatically set to the maximum of `highScore2D` and `highScore3D`
- Old code using unified score methods will still work but should be updated to use the new separate methods
- Level data (`currentLevel`) is saved to Firebase whenever high scores are saved
- Both scores are saved independently and can have different values

