# Profile Button & Stats Popup Setup Guide

This guide will help you add a profile button to the main menu and create a popup that displays user statistics from Firebase.

## Overview

The profile system includes:
- ✅ **ProfileButton** component - Attach to any button to open profile popup
- ✅ **ProfileStatsPopup** - Displays user data fetched from Firebase
- ✅ **MainScene.OnClickProfile()** - Method to show profile popup (alternative to ProfileButton)

## Step 1: Create ProfileStatsPopup Prefab

### 1.1 Create the Popup UI Structure

1. In Unity, create a new GameObject:
   - Right-click in Hierarchy → Create Empty
   - Name it: `ProfileStatsPopup`

2. Add UI Panel as background:
   - Right-click on `ProfileStatsPopup` → **UI → Panel**
   - Name it: `BackgroundPanel`
   - Set RectTransform: Center-Middle, Width: 600, Height: 700
   - Add Image component with your theme color

3. Add UI Elements:

   **a) Title:**
   - Right-click on `BackgroundPanel` → **UI → Text - TextMeshPro**
   - Name: `TitleText`
   - Text: "Profile"
   - Font Size: 36
   - Alignment: Center
   - Position: Top, Y: 300

   **b) Avatar Display:**
   - Right-click on `BackgroundPanel` → **UI → Image**
   - Name: `AvatarImage`
   - Position: Center, Y: 200
   - Size: 150x150 (or your preferred size)
   - Set a default avatar sprite

   **c) Username:**
   - Right-click on `BackgroundPanel` → **UI → Text - TextMeshPro**
   - Name: `UsernameText`
   - Text: "Loading..."
   - Font Size: 32
   - Alignment: Center
   - Position: Center, Y: 100

   **d) Score:**
   - Right-click on `BackgroundPanel` → **UI → Text - TextMeshPro**
   - Name: `ScoreLabel`
   - Text: "Total Score:"
   - Font Size: 24
   - Position: Center, Y: 0
   
   - Right-click on `BackgroundPanel` → **UI → Text - TextMeshPro**
   - Name: `ScoreText`
   - Text: "0"
   - Font Size: 28
   - Alignment: Center
   - Position: Center, Y: -30

   **e) Level:**
   - Right-click on `BackgroundPanel` → **UI → Text - TextMeshPro**
   - Name: `LevelLabel`
   - Text: "Level:"
   - Font Size: 24
   - Position: Center, Y: -80
   
   - Right-click on `BackgroundPanel` → **UI → Text - TextMeshPro**
   - Name: `LevelText`
   - Text: "1"
   - Font Size: 28
   - Alignment: Center
   - Position: Center, Y: -110

   **f) Creation Date:**
   - Right-click on `BackgroundPanel` → **UI → Text - TextMeshPro**
   - Name: `CreationDateLabel`
   - Text: "Member Since:"
   - Font Size: 20
   - Position: Center, Y: -160
   
   - Right-click on `BackgroundPanel` → **UI → Text - TextMeshPro**
   - Name: `CreationDateText`
   - Text: "N/A"
   - Font Size: 22
   - Alignment: Center
   - Position: Center, Y: -190

   **g) Close Button:**
   - Right-click on `BackgroundPanel` → **UI → Button - TextMeshPro**
   - Name: `CloseButton`
   - Text: "Close"
   - Position: Center, Y: -250

   **h) Loading Indicator (Optional):**
   - Right-click on `BackgroundPanel` → **UI → Image**
   - Name: `LoadingIndicator`
   - Add a loading spinner or text
   - Initially set to active

   **i) Error Text (Optional):**
   - Right-click on `BackgroundPanel` → **UI → Text - TextMeshPro**
   - Name: `ErrorText`
   - Text: ""
   - Color: Red
   - Alignment: Center
   - Position: Center, Y: 0
   - Initially set to inactive

### 1.2 Add ProfileStatsPopup Component

1. Select the root `ProfileStatsPopup` GameObject
2. Add Component → `ProfileStatsPopup`

3. Assign references in Inspector:

   **UI References:**
   - **Username Text**: Drag `UsernameText` TextMeshProUGUI
   - **Avatar Image**: Drag `AvatarImage` Image
   - **Score Text**: Drag `ScoreText` TextMeshProUGUI
   - **Level Text**: Drag `LevelText` TextMeshProUGUI
   - **Creation Date Text**: Drag `CreationDateText` TextMeshProUGUI
   - **Close Button**: Drag `CloseButton` Button
   - **Loading Indicator**: Drag `LoadingIndicator` GameObject (if created)
   - **Error Text**: Drag `ErrorText` TextMeshProUGUI (if created)

   **Avatar Sprites:**
   - **Size**: 6 (for avatars 0-5)
   - **Element 0-5**: Drag your avatar sprites (matching avatarId from profile setup)

   **Default Values:**
   - **Default Avatar Sprite**: Drag a fallback avatar sprite
   - **Default Username**: "Guest" (or your preference)
   - **Loading Text**: "Loading..." (or your preference)
   - **Error Message**: "Failed to load profile data." (or your preference)

### 1.3 Make it a Prefab

1. Drag `ProfileStatsPopup` from Hierarchy to `Assets/Resources/Prefabs/` folder
2. **Important**: The path must be `Resources/Prefabs/ProfileStatsPopup` for the default setup
3. Delete the instance from the scene

## Step 2: Add Profile Button to Main Menu

You have two options:

### Option A: Use ProfileButton Component (Recommended)

1. Find or create a button in your main menu (e.g., near settings button)
2. Add Component → `ProfileButton`
3. Configure:
   - **Profile Popup Prefab Path**: "Prefabs/ProfileStatsPopup" (default, should work)
4. The button will automatically open the profile popup when clicked

### Option B: Use MainScene Method

1. Find a button in your main menu
2. In the Button's **OnClick** event:
   - Add a new event
   - Drag the GameObject with `MainScene` component
   - Select: `MainScene → OnClickProfile()`

## Step 3: Prepare Avatar Sprites

1. Import your avatar sprites (6 avatars for IDs 0-5)
2. Ensure they match the avatar IDs used in ProfileSetupPopup
3. Assign them to the ProfileStatsPopup prefab's **Avatar Sprites** array

## Step 4: Test

1. **Run the game** from MainScene 1
2. **Click the profile button**
3. **Expected behavior**:
   - Popup appears with loading indicator
   - Profile data loads from Firebase
   - Displays: username, avatar, score, level, creation date
   - Close button closes the popup

## Quick Setup Checklist

- [ ] Created `ProfileStatsPopup` prefab with all UI elements
- [ ] Added `ProfileStatsPopup` component and assigned all references
- [ ] Assigned avatar sprites (0-5)
- [ ] Saved prefab to `Resources/Prefabs/ProfileStatsPopup`
- [ ] Added profile button to main menu
- [ ] Added `ProfileButton` component OR connected to `MainScene.OnClickProfile()`
- [ ] Tested the popup

## UI Layout Example

```
ProfileStatsPopup
└── BackgroundPanel
    ├── TitleText ("Profile")
    ├── AvatarImage (150x150)
    ├── UsernameText
    ├── ScoreLabel + ScoreText
    ├── LevelLabel + LevelText
    ├── CreationDateLabel + CreationDateText
    ├── CloseButton
    ├── LoadingIndicator (optional)
    └── ErrorText (optional)
```

## Customization

### Change Popup Appearance

1. **Background**: Modify `BackgroundPanel` Image color/sprite
2. **Layout**: Adjust RectTransform positions and sizes
3. **Fonts**: Change TextMeshPro font assets
4. **Colors**: Customize text colors to match your theme

### Add More Stats

To display additional data:

1. Add new UI Text elements in the popup
2. Add corresponding fields to `ProfileStatsPopup.cs`:
   ```csharp
   [SerializeField] private TextMeshProUGUI newStatText;
   ```
3. Update `DisplayProfile()` method to show the new stat
4. Update `UserProfile.cs` if you need to store new data

### Custom Loading Animation

Add a rotating spinner to `LoadingIndicator`:

```csharp
using DG.Tweening;
using UnityEngine;

public class RotateSpinner : MonoBehaviour
{
    void Start()
    {
        transform.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}
```

## Troubleshooting

### Popup doesn't appear
- Check that prefab is in `Resources/Prefabs/ProfileStatsPopup`
- Verify button is connected correctly
- Check Console for errors

### Data not loading
- Verify Firebase is initialized
- Check Firebase rules allow read access
- Check Console for Firebase errors
- Verify user profile exists in Firebase

### Avatar not showing
- Check avatar sprites are assigned in Inspector
- Verify avatarId matches sprite array index (0-5)
- Ensure default avatar sprite is assigned

### Wrong data displayed
- Check Firebase database has correct data
- Verify UserProfile structure matches Firebase data
- Check Console logs for data being fetched

## Data Structure

The popup displays:
- **Username**: From `profile.username`
- **Avatar**: Sprite from array index `profile.avatarId`
- **Total Score**: From `profile.totalScore`
- **Current Level**: From `profile.currentLevel`
- **Creation Date**: From `profile.creationDate` (formatted)

## Notes

- The popup automatically fetches data from Firebase when opened
- Loading indicator shows while fetching
- Error message displays if fetch fails
- Close button uses DialogManager to close properly
- Profile data is fetched fresh each time (no caching)

