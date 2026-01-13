# Firebase Profile Setup - Unity Editor Configuration Guide

This guide will help you set up the Firebase profile system in your Unity project.

## Prerequisites

✅ Firebase SDK is already installed in your project  
✅ `google-services.json` is in `StreamingAssets/` folder  
✅ Firebase Realtime Database is set up in Firebase Console

## Step 1: Firebase Realtime Database Rules Setup

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select your project: **Even-Merge-2048**
3. Navigate to **Realtime Database** → **Rules** tab
4. Set the following rules (for initial testing, you can use these permissive rules):

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

**⚠️ Important:** For production, you should implement more secure rules. For now, this allows authenticated users to read/write their own data.

5. Click **Publish** to save the rules

## Step 2: Create FirebaseManager Prefab

1. In Unity, create a new GameObject:
   - Right-click in Hierarchy → Create Empty
   - Name it: `FirebaseManager`

2. Add the `FirebaseManager` component:
   - Select the GameObject
   - In Inspector, click **Add Component**
   - Search for `FirebaseManager` and add it

3. Make it a Prefab:
   - Drag the GameObject from Hierarchy to `Assets/Prefabs/` folder
   - Delete the instance from the scene (we'll instantiate it via script)

## Step 3: Setup StartingLanguageSelection Scene

1. Open the scene: `Scenes/StartingLanguageSelection.unity`

2. Create a GameObject for Firebase initialization:
   - Right-click in Hierarchy → Create Empty
   - Name it: `FirebaseInitializer`

3. Add the `FirebaseInitializer` component:
   - Select the GameObject
   - Add Component → `FirebaseInitializer`
   - In Inspector, drag the `FirebaseManager` prefab to the **Firebase Manager Prefab** field

## Step 4: Create ProfileSetupPopup Prefab

### 4.1 Create the Popup UI Structure

1. In `MainScene 1`, create a Canvas (if not exists):
   - Right-click in Hierarchy → UI → Canvas
   - Set Canvas Scaler to **Scale With Screen Size**

2. Create the Popup Panel:
   - Right-click on Canvas → UI → Panel
   - Name it: `ProfileSetupPopup`
   - Set RectTransform: Stretch-Stretch (full screen)
   - Add an Image component with semi-transparent background (e.g., Color: Black, Alpha: 200)

3. Create the Content Panel (inside ProfileSetupPopup):
   - Right-click on ProfileSetupPopup → UI → Panel
   - Name it: `PopupContent`
   - Set RectTransform: Center-Middle, Width: 600, Height: 500
   - Add a background Image (white or your theme color)

4. Add UI Elements inside PopupContent:

   **a) Title Text:**
   - Right-click on PopupContent → UI → Text - TextMeshPro
   - Name: `TitleText`
   - Text: "Create Your Profile"
   - Font Size: 32
   - Alignment: Center

   **b) Username Input:**
   - Right-click on PopupContent → UI → Input Field - TextMeshPro
   - Name: `UsernameInput`
   - Placeholder: "Enter your username (min 3 characters)"
   - Position: Center, Y: 100

   **c) Avatar Container:**
   - Right-click on PopupContent → UI → Empty GameObject
   - Name: `AvatarContainer`
   - Position: Center, Y: 0
   - Add Horizontal Layout Group component:
     - Spacing: 20
     - Child Alignment: Middle Center

   **d) Avatar Buttons (create 6 buttons):**
   For each avatar (0-5):
   - Right-click on AvatarContainer → UI → Button - TextMeshPro
   - Name: `AvatarButton_0`, `AvatarButton_1`, etc.
   - Remove the Text child (we'll use images)
   - Add an Image component to the button
   - Add the `AvatarSelectorButton` component
   - Set **Avatar Id** in Inspector (0, 1, 2, 3, 4, 5)
   - Optionally add a child GameObject for selected indicator (highlight border)

   **e) Save Button:**
   - Right-click on PopupContent → UI → Button - TextMeshPro
   - Name: `SaveButton`
   - Text: "Save"
   - Position: Center, Y: -150

   **f) Loading Indicator:**
   - Right-click on PopupContent → UI → Image
   - Name: `LoadingIndicator`
   - Add a loading spinner image or use a simple rotating image
   - Initially set to inactive

   **g) Status Text (optional):**
   - Right-click on PopupContent → UI → Text - TextMeshPro
   - Name: `StatusText`
   - Text: ""
   - Color: Red (for error messages)
   - Position: Below Save Button

### 4.2 Configure ProfileSetupPopup Script

1. Select the `ProfileSetupPopup` GameObject (root)

2. Add the `ProfileSetupPopup` component:
   - Add Component → `ProfileSetupPopup`

3. Assign references in Inspector:
   - **Popup Content**: Drag `PopupContent` GameObject
   - **Username Input**: Drag `UsernameInput` InputField
   - **Save Button**: Drag `SaveButton` Button
   - **Avatar Container**: Drag `AvatarContainer` Transform
   - **Loading Indicator**: Drag `LoadingIndicator` GameObject
   - **Status Text**: Drag `StatusText` TextMeshProUGUI (if created)
   - **Available Avatar Ids**: Keep default [0, 1, 2, 3, 4, 5]

### 4.3 Make it a Prefab

1. Drag `ProfileSetupPopup` from Hierarchy to `Assets/Prefabs/` folder
2. Delete the instance from `MainScene 1` (we'll instantiate it via script)

## Step 5: Setup MainScene 1

1. Open the scene: `Scenes/MainScene 1.unity`

2. Find or create the Canvas:
   - If Canvas doesn't exist, create one (UI → Canvas)

3. Create a GameObject for Profile Checker:
   - Right-click in Hierarchy → Create Empty
   - Name it: `MainMenuProfileChecker`
   - Make sure it's a child of Canvas (or at root level)

4. Add the `MainMenuProfileChecker` component:
   - Select the GameObject
   - Add Component → `MainMenuProfileChecker`
   - **Profile Setup Prefab**: Drag `ProfileSetupPopup` prefab from Prefabs folder
   - **Canvas Root**: Drag the Canvas GameObject (or leave null if at root)

## Step 6: Test the Setup

1. **Clear PlayerPrefs** (to test first-time setup):
   - In Unity Editor: Edit → PlayerPrefs → Delete All
   - Or add a test button that calls `PlayerPrefs.DeleteAll()`

2. **Build Settings**:
   - File → Build Settings
   - Ensure scenes are in this order:
     1. `StartingLanguageSelection` (index 0)
     2. `MainScene 1` (index 1)
     3. `scene` (index 2)

3. **Run the Game**:
   - Play the game from `StartingLanguageSelection` scene
   - Select a language
   - You should see the profile setup popup in `MainScene 1`
   - Enter a username and select an avatar
   - Click Save
   - The popup should disappear and show the main menu

## Step 7: Verify Firebase Data

1. Go to Firebase Console → Realtime Database → Data tab
2. You should see a structure like:
   ```
   users/
     └── [userId]/
         ├── userId: "[userId]"
         ├── username: "YourUsername"
         ├── avatarId: 0
         ├── creationDate: "2024-01-01 12:00:00"
         ├── totalScore: 0
         └── currentLevel: 1
   ```

## Troubleshooting

### Popup doesn't appear
- Check that `MainMenuProfileChecker` is in the scene
- Verify `ProfileSetupPopup` prefab is assigned
- Check Console for errors

### Firebase not initializing
- Verify `google-services.json` is in `StreamingAssets/` folder
- Check that `FirebaseInitializer` is in `StartingLanguageSelection` scene
- Look for Firebase errors in Console

### Avatar selection not working
- Ensure `AvatarSelectorButton` component is on each avatar button
- Verify `Avatar Id` is set correctly (0-5)
- Check that `AvatarContainer` is assigned in `ProfileSetupPopup`

### Save button not working
- Check username is at least 3 characters
- Verify Firebase rules allow write access
- Check Console for Firebase errors

## Next Steps

After this basic setup works:
1. Add avatar images/sprites to the avatar buttons
2. Style the popup UI to match your game's theme
3. Add validation for username (e.g., no special characters)
4. Implement leaderboard functionality
5. Add more user profile fields as needed

## Notes

- The system uses anonymous authentication, so each device gets a unique user ID
- Profile data is saved locally in PlayerPrefs as a flag to avoid repeated popups
- The popup only shows if no profile exists in Firebase
- FirebaseManager persists across scenes using `DontDestroyOnLoad`

