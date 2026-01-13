# Leaderboard Feature Setup Guide

This guide will help you set up the leaderboard feature in your Unity project. The leaderboard displays rankings for both 2D and 3D game modes, fetched from Firebase Realtime Database.

## Overview

The leaderboard system includes:
- ✅ **LeaderboardPopup** - Main popup with tabs for 2D and 3D leaderboards
- ✅ **LeaderboardEntry** - Individual entry component for each player
- ✅ **FirebaseManager.GetLeaderboard()** - Method to fetch sorted leaderboard data
- ✅ **MainScene.OnClickLeaderboard()** - Method to show leaderboard popup
- ✅ Current user profile display at the bottom

## Prerequisites

✅ Firebase SDK is installed  
✅ Firebase Realtime Database is set up  
✅ `google-services.json` is in `StreamingAssets/` folder  
✅ FirebaseManager prefab exists and is initialized  
✅ User profiles are being saved with `highScore2D` and `highScore3D` fields

## Step 1: Create LeaderboardEntry Prefab

### 1.1 Create the Entry UI Structure

1. In Unity, create a new GameObject:
   - Right-click in Hierarchy → Create Empty
   - Name it: `LeaderboardEntry`

2. Add UI Panel as background:
   - Right-click on `LeaderboardEntry` → **UI → Panel**
   - Name it: `BackgroundPanel`
   - Set RectTransform: Stretch-Stretch (full width)
   - Set Height: 80 (or your preferred height)
   - Add Image component with your theme color

3. Add UI Elements inside BackgroundPanel:

   **a) Rank Text:**
   - Right-click on BackgroundPanel → **UI → Text - TextMeshPro**
   - Name: `RankText`
   - Text: "1"
   - Font Size: 24
   - Alignment: Center
   - Position: Left, X: 20, Width: 50

   **b) Avatar Image:**
   - Right-click on BackgroundPanel → **UI → Image**
   - Name: `AvatarImage`
   - Position: Left, X: 80
   - Size: 60x60 (or your preferred size)
   - Set a default avatar sprite

   **c) Username Text:**
   - Right-click on BackgroundPanel → **UI → Text - TextMeshPro**
   - Name: `UsernameText`
   - Text: "PlayerName"
   - Font Size: 22
   - Alignment: Left
   - Position: Left, X: 150, Width: 200

   **d) Score Text:**
   - Right-click on BackgroundPanel → **UI → Text - TextMeshPro**
   - Name: `ScoreText`
   - Text: "0"
   - Font Size: 24
   - Alignment: Right
   - Position: Right, X: -20, Width: 150

4. Add the `LeaderboardEntry` component:
   - Select `LeaderboardEntry` GameObject
   - In Inspector, click **Add Component**
   - Search for `LeaderboardEntry` and add it

5. Assign references in Inspector:
   - **Rank Text**: Drag `RankText` TextMeshProUGUI
   - **Avatar Image**: Drag `AvatarImage` Image
   - **Username Text**: Drag `UsernameText` TextMeshProUGUI
   - **Score Text**: Drag `ScoreText` TextMeshProUGUI
   - **Background Image**: Drag `BackgroundPanel` Image
   - **Avatar Sprites**: Create an array of 6 sprites (indices 0-5) matching your avatar system
   - **Default Avatar Sprite**: Assign a default sprite

6. Make it a Prefab:
   - Drag `LeaderboardEntry` from Hierarchy to `Assets/Prefabs/` folder
   - Delete the instance from the scene

## Step 2: Create LeaderboardPopup Prefab

### 2.1 Create the Popup UI Structure

1. In Unity, create a new GameObject:
   - Right-click in Hierarchy → Create Empty
   - Name it: `LeaderboardPopup`

2. Add UI Panel as background:
   - Right-click on `LeaderboardPopup` → **UI → Panel**
   - Name it: `BackgroundPanel`
   - Set RectTransform: Center-Middle, Width: 700, Height: 900
   - Add Image component with your theme color

3. Add Title:
   - Right-click on BackgroundPanel → **UI → Text - TextMeshPro**
   - Name: `TitleText`
   - Text: "Leaderboard"
   - Font Size: 36
   - Alignment: Center
   - Position: Top, Y: -30

4. Add Close Button:
   - Right-click on BackgroundPanel → **UI → Button - TextMeshPro**
   - Name: `CloseButton`
   - Position: Top-Right, X: -20, Y: -20
   - Size: 40x40
   - Add an "X" text or icon sprite
   - Set button color/style

5. Create Tab System:

   **a) Tab Container:**
   - Right-click on BackgroundPanel → **UI → Empty GameObject**
   - Name: `TabContainer`
   - Position: Top, Y: -80
   - Add Horizontal Layout Group:
     - Spacing: 10
     - Child Alignment: Middle Center
     - Child Force Expand: Width = true

   **b) Tab 2D Button:**
   - Right-click on TabContainer → **UI → Button - TextMeshPro**
   - Name: `Tab2DButton`
   - Text: "2D"
   - Font Size: 24
   - Add Image component for indicator
   - Create child GameObject: `Tab2DIndicator` (Image) for active state

   **c) Tab 3D Button:**
   - Right-click on TabContainer → **UI → Button - TextMeshPro**
   - Name: `Tab3DButton`
   - Text: "3D"
   - Font Size: 24
   - Add Image component for indicator
   - Create child GameObject: `Tab3DIndicator` (Image) for active state

6. Create Scroll Views:

   **a) 2D Tab Content:**
   - Right-click on BackgroundPanel → **UI → Scroll View**
   - Name: `Tab2DContent`
   - Set RectTransform: Stretch-Stretch
   - Set Top: 120, Bottom: 200 (leaving space for tabs and current user panel)
   - In ScrollRect component:
     - **Content**: Create child GameObject `Content2D` (RectTransform)
     - **Vertical Scrollbar**: Create or assign
   - Add Vertical Layout Group to `Content2D`:
     - Spacing: 5
     - Child Alignment: Upper Center
     - Child Force Expand: Width = true

   **b) 3D Tab Content:**
   - Right-click on BackgroundPanel → **UI → Scroll View**
   - Name: `Tab3DContent`
   - Set RectTransform: Stretch-Stretch
   - Set Top: 120, Bottom: 200
   - Initially set **Active** = false (hidden)
   - In ScrollRect component:
     - **Content**: Create child GameObject `Content3D` (RectTransform)
     - **Vertical Scrollbar**: Create or assign
   - Add Vertical Layout Group to `Content3D`:
     - Spacing: 5
     - Child Alignment: Upper Center
     - Child Force Expand: Width = true

7. Create Current User Panel:

   - Right-click on BackgroundPanel → **UI → Panel**
   - Name: `CurrentUserPanel`
   - Set RectTransform: Bottom, Height: 180
   - Add Image component with distinct color (e.g., slightly darker)

   **Inside CurrentUserPanel:**

   **a) Avatar:**
   - Right-click on CurrentUserPanel → **UI → Image**
   - Name: `CurrentUserAvatar`
   - Position: Left, X: 20
   - Size: 80x80

   **b) Username:**
   - Right-click on CurrentUserPanel → **UI → Text - TextMeshPro**
   - Name: `CurrentUserUsername`
   - Text: "Your Name"
   - Font Size: 24
   - Position: Left, X: 120, Y: 40

   **c) 2D Rank & Score:**
   - Right-click on CurrentUserPanel → **UI → Text - TextMeshPro**
   - Name: `CurrentUserRank2D`
   - Text: "#1"
   - Font Size: 20
   - Position: Left, X: 120, Y: 0
   
   - Right-click on CurrentUserPanel → **UI → Text - TextMeshPro**
   - Name: `CurrentUserScore2D`
   - Text: "Score: 0"
   - Font Size: 20
   - Position: Left, X: 200, Y: 0

   **d) 3D Rank & Score:**
   - Right-click on CurrentUserPanel → **UI → Text - TextMeshPro**
   - Name: `CurrentUserRank3D`
   - Text: "#1"
   - Font Size: 20
   - Position: Right, X: -200, Y: 0
   
   - Right-click on CurrentUserPanel → **UI → Text - TextMeshPro**
   - Name: `CurrentUserScore3D`
   - Text: "Score: 0"
   - Font Size: 20
   - Position: Right, X: -120, Y: 0

8. Add Loading & Error Indicators:

   **a) Loading Indicator:**
   - Right-click on BackgroundPanel → **UI → Text - TextMeshPro**
   - Name: `LoadingIndicator`
   - Text: "Loading..."
   - Font Size: 24
   - Alignment: Center
   - Position: Center

   **b) Error Text:**
   - Right-click on BackgroundPanel → **UI → Text - TextMeshPro**
   - Name: `ErrorText`
   - Text: "Failed to load leaderboard"
   - Font Size: 20
   - Alignment: Center
   - Color: Red
   - Initially set **Active** = false

   **c) Empty Text:**
   - Right-click on BackgroundPanel → **UI → Text - TextMeshPro**
   - Name: `EmptyText`
   - Text: "No players yet"
   - Font Size: 20
   - Alignment: Center
   - Position: Center
   - Initially set **Active** = false

9. Add the `LeaderboardPopup` component:
   - Select `LeaderboardPopup` GameObject
   - In Inspector, click **Add Component**
   - Search for `LeaderboardPopup` and add it

10. Assign all references in Inspector:

    **Tab System:**
    - **Tab 2D Button**: Drag `Tab2DButton`
    - **Tab 3D Button**: Drag `Tab3DButton`
    - **Tab 2D Content**: Drag `Tab2DContent` GameObject
    - **Tab 3D Content**: Drag `Tab3DContent` GameObject
    - **Tab 2D Indicator**: Drag `Tab2DIndicator` Image
    - **Tab 3D Indicator**: Drag `Tab3DIndicator` Image

    **Scroll Views:**
    - **Scroll View 2D**: Drag `Tab2DContent` ScrollRect component
    - **Scroll View 3D**: Drag `Tab3DContent` ScrollRect component
    - **Content 2D**: Drag `Content2D` Transform
    - **Content 3D**: Drag `Content3D` Transform

    **Leaderboard Entry:**
    - **Leaderboard Entry Prefab**: Drag `LeaderboardEntry` prefab from Prefabs folder

    **Current User Display:**
    - **Current User Panel**: Drag `CurrentUserPanel` GameObject
    - **Current User Avatar**: Drag `CurrentUserAvatar` Image
    - **Current User Username**: Drag `CurrentUserUsername` TextMeshProUGUI
    - **Current User Rank 2D**: Drag `CurrentUserRank2D` TextMeshProUGUI
    - **Current User Rank 3D**: Drag `CurrentUserRank3D` TextMeshProUGUI
    - **Current User Score 2D**: Drag `CurrentUserScore2D` TextMeshProUGUI
    - **Current User Score 3D**: Drag `CurrentUserScore3D` TextMeshProUGUI

    **Close Button:**
    - **Close Button**: Drag `CloseButton` Button

    **Loading & Error:**
    - **Loading Indicator**: Drag `LoadingIndicator` GameObject
    - **Error Text**: Drag `ErrorText` TextMeshProUGUI
    - **Empty Text**: Drag `EmptyText` TextMeshProUGUI

    **Avatar Sprites:**
    - **Avatar Sprites**: Create array of 6 sprites (indices 0-5) matching your avatar system
    - **Default Avatar Sprite**: Assign a default sprite

    **Settings:**
    - **Max Leaderboard Entries**: 100 (or your preferred limit)
    - **Active Tab Color**: Yellow/Orange (e.g., R:255, G:204, B:0)
    - **Inactive Tab Color**: Gray (e.g., R:180, G:180, B:180)

11. Make it a Prefab:
    - Drag `LeaderboardPopup` from Hierarchy to `Assets/Prefabs/` folder
    - Delete the instance from the scene

## Step 3: Add Leaderboard Button to MainScene

### 3.1 Create Leaderboard Button

1. Open `MainScene 1` scene

2. Find the main menu UI (usually in Canvas or panel_content)

3. Create Leaderboard Button:
   - Right-click on your button container → **UI → Button - TextMeshPro**
   - Name: `LeaderboardButton`
   - Text: "Leaderboard"
   - Position it where you want (e.g., next to Profile button)
   - Style it to match your UI theme

4. Assign the button click event:
   - Select `LeaderboardButton`
   - In Inspector, find **Button** component
   - Under **OnClick()**, click **+** to add event
   - Drag `MainScene` GameObject (the one with MainScene script) to the object field
   - Select function: **MainScene → OnClickLeaderboard()**

### 3.2 Assign LeaderboardPopup Prefab to MainScene

1. Select the GameObject with `MainScene` script (usually named `MainScene` or similar)

2. In Inspector, find **MainScene** component

3. Under **Popup Prefabs**:
   - **Leaderboard Popup Prefab**: Drag `LeaderboardPopup` prefab from Prefabs folder

## Step 4: Verify Firebase Database Rules

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select your project: **Even-Merge-2048**
3. Navigate to **Realtime Database** → **Rules** tab
4. Ensure rules allow reading user data:

```json
{
  "rules": {
    "users": {
      "$userId": {
        ".read": "auth != null",
        ".write": "auth != null"
      }
    }
  }
}
```

**Note:** For leaderboard, users need to read ALL user profiles (not just their own). For production, consider:

```json
{
  "rules": {
    "users": {
      "$userId": {
        ".read": "auth != null",
        ".write": "$userId === auth.uid"
      }
    }
  }
}
```

This allows authenticated users to read all profiles (for leaderboard) but only write their own.

5. Click **Publish** to save the rules

## Step 5: Test the Leaderboard

1. **Build Settings**:
   - File → Build Settings
   - Ensure scenes are in correct order

2. **Run the Game**:
   - Play from `MainScene 1` scene
   - Click the Leaderboard button
   - The popup should appear with loading indicator
   - After loading, you should see:
     - Two tabs: "2D" and "3D"
     - List of players ranked by score
     - Current user profile at the bottom
     - Close button works

3. **Test Tab Switching**:
   - Click "2D" tab → Should show 2D leaderboard
   - Click "3D" tab → Should show 3D leaderboard
   - Verify scrolling works

4. **Test Current User Display**:
   - Verify your profile appears at the bottom
   - Check that ranks are correct for both 2D and 3D

## Step 6: Customization (Optional)

### Change Colors

1. **Tab Active Color**: In LeaderboardPopup component, adjust `Active Tab Color`
2. **Tab Inactive Color**: Adjust `Inactive Tab Color`
3. **Current User Highlight**: In LeaderboardEntry component, adjust `Current User Color`

### Change Layout

1. **Entry Height**: Adjust BackgroundPanel height in LeaderboardEntry prefab
2. **Popup Size**: Adjust BackgroundPanel size in LeaderboardPopup prefab
3. **Font Sizes**: Adjust TextMeshProUGUI font sizes in both prefabs

### Limit Entries

- In LeaderboardPopup component, adjust `Max Leaderboard Entries` (default: 100)

## Troubleshooting

### Leaderboard doesn't load
- ✅ Check Firebase is initialized (FirebaseManager.Instance != null)
- ✅ Check Firebase Realtime Database rules allow reading
- ✅ Check Console for Firebase errors
- ✅ Verify user profiles exist in Firebase with `highScore2D` and `highScore3D` fields

### Empty leaderboard
- ✅ Verify users have scores saved in Firebase
- ✅ Check that `username` field is not empty (entries without username are filtered out)
- ✅ Check Firebase Console → Realtime Database → Data tab

### Current user not showing
- ✅ Verify FirebaseManager.CurrentUserId is set
- ✅ Check that current user profile exists in Firebase
- ✅ Verify user has a username set

### Tabs not switching
- ✅ Check Tab2DContent and Tab3DContent GameObjects are assigned
- ✅ Verify tab buttons have onClick listeners assigned
- ✅ Check Console for errors

### Scrolling not working
- ✅ Verify ScrollRect components are assigned
- ✅ Check Content transforms have Vertical Layout Group
- ✅ Ensure Content height is larger than ScrollRect viewport

### Entries not displaying
- ✅ Verify LeaderboardEntry prefab is assigned
- ✅ Check LeaderboardEntry prefab has LeaderboardEntry component
- ✅ Verify all UI references are assigned in LeaderboardEntry component

## File Structure

```
Assets/
├── Scripts/
│   ├── Analytics_Scripts/
│   │   └── FirebaseManager.cs (updated with GetLeaderboard method)
│   ├── Leaderboard/
│   │   ├── LeaderboardEntry.cs (new)
│   │   └── LeaderboardPopup.cs (new)
│   └── MainScene.cs (updated with OnClickLeaderboard method)
└── Prefabs/
    ├── LeaderboardEntry.prefab (new)
    └── LeaderboardPopup.prefab (new)
```

## Summary

✅ Created LeaderboardEntry prefab with rank, avatar, username, and score  
✅ Created LeaderboardPopup prefab with 2D/3D tabs and scrolling  
✅ Added GetLeaderboard method to FirebaseManager  
✅ Added OnClickLeaderboard method to MainScene  
✅ Added leaderboard button to main menu  
✅ Configured Firebase database rules  
✅ Tested leaderboard functionality  

The leaderboard feature is now ready to use! Players can view rankings for both 2D and 3D game modes, and see their own profile at the bottom of the list.

