# Loading Screen Setup Guide

This guide will help you set up a loading screen that appears when entering MainScene 1 while the system checks Firebase for user profile.

## Overview

The loading screen will:
- ✅ Show immediately when MainScene 1 loads
- ✅ Hide all menu elements during loading
- ✅ Display a loading indicator
- ✅ Automatically hide when profile check is complete
- ✅ Show menu elements again after loading

## Step 1: Create Loading Screen UI

### 1.1 Create the Loading Screen Panel

1. In `MainScene 1`, find or create your Canvas
2. Right-click on Canvas → **UI → Panel**
3. Name it: `LoadingScreenPanel`
4. Set RectTransform to **Stretch-Stretch** (full screen)
5. Add an Image component:
   - **Color**: Semi-transparent black (R:0, G:0, B:0, A:200) or your theme color
   - **Raycast Target**: Checked (to block clicks during loading)

### 1.2 Add Loading Indicator (Optional)

1. Right-click on `LoadingScreenPanel` → **UI → Image**
2. Name it: `LoadingIndicator`
3. Add a loading spinner image or use a simple rotating image
4. Position it in the center
5. **Optional**: Add a rotating animation script

**Alternative - Text Loading:**
- Right-click on `LoadingScreenPanel` → **UI → Text - TextMeshPro**
- Name it: `LoadingText`
- Text: "Loading..."
- Center it on screen
- Font Size: 48

### 1.3 Add CanvasGroup (for fade effects)

1. Select `LoadingScreenPanel`
2. Add Component → **Canvas Group**
3. This enables smooth fade in/out effects

## Step 2: Identify Menu Elements to Hide

You need to identify all menu UI elements that should be hidden during loading. Common elements include:

- Top panel (level, gems, diamonds)
- Bottom buttons (shop, skin, task, etc.)
- Game list/content panels
- Any other visible menu elements

**Tip**: You can hide the entire `panel_content` or `panel_top` if they contain all menu elements.

## Step 3: Setup LoadingScreen Component

1. Create an empty GameObject (or use an existing manager GameObject)
2. Name it: `LoadingScreenManager`
3. Add Component → `LoadingScreen`

### Configure LoadingScreen Component:

**Loading Screen UI:**
- **Loading Screen Panel**: Drag `LoadingScreenPanel` GameObject
- **Loading Indicator**: Drag `LoadingIndicator` GameObject (if you created one)

**Menu Elements to Hide:**
- **Size**: Set to the number of menu elements you want to hide
- **Element 0**: Drag first menu element (e.g., `panel_top`)
- **Element 1**: Drag second menu element (e.g., `panel_content`)
- **Element 2**: Drag third menu element (e.g., `gamelist`)
- Continue for all elements you want to hide

**Settings:**
- **Fade In Duration**: 0.3 (seconds)
- **Fade Out Duration**: 0.3 (seconds)

## Step 4: Update MainMenuProfileChecker

1. Find the GameObject with `MainMenuProfileChecker` component
2. In Inspector, you'll see a new field: **Loading Screen**
3. Drag the `LoadingScreenManager` GameObject (with LoadingScreen component) to this field

## Step 5: Test

1. **Clear PlayerPrefs** (to test first-time user)
2. Run the game from `StartingLanguageSelection` scene
3. Select language
4. **Expected**: 
   - Loading screen appears immediately
   - Menu elements are hidden
   - After profile check completes, loading screen fades out
   - Menu elements appear again

## Quick Setup Checklist

- [ ] Created `LoadingScreenPanel` (full screen, semi-transparent)
- [ ] Added `LoadingIndicator` (optional spinner/text)
- [ ] Added `CanvasGroup` to panel (for fade effects)
- [ ] Created `LoadingScreenManager` GameObject
- [ ] Added `LoadingScreen` component
- [ ] Assigned `LoadingScreenPanel` to component
- [ ] Assigned all menu elements to hide
- [ ] Assigned `LoadingScreen` to `MainMenuProfileChecker`

## Example Setup

### Simple Setup (Hide Everything):
```
Menu Elements to Hide:
- Element 0: panel_top (hides level, gems, etc.)
- Element 1: panel_content (hides game list)
- Element 2: m_img_buttons (hides bottom buttons)
```

### Detailed Setup (Hide Specific Elements):
```
Menu Elements to Hide:
- Element 0: panel_top
- Element 1: gamelist
- Element 2: m_btn_shop
- Element 3: m_btn_skin
- Element 4: m_btn_task
- Element 5: m_btn_start
- Element 6: m_btn_achiev
- Element 7: m_btn_activity
```

## Customization

### Change Loading Screen Appearance

1. **Background Color**: 
   - Select `LoadingScreenPanel`
   - Change Image color to match your theme

2. **Loading Animation**:
   - Create a rotating spinner sprite
   - Add a simple rotation script to `LoadingIndicator`
   - Or use DOTween for smooth rotation

3. **Loading Text**:
   - Add TextMeshPro text to panel
   - Text: "Loading..." or "Please wait..."
   - Style it to match your game

### Adjust Fade Speed

In `LoadingScreen` component:
- **Fade In Duration**: How fast it appears (0 = instant)
- **Fade Out Duration**: How fast it disappears (0 = instant)

## Troubleshooting

### Loading screen doesn't appear
- Check that `LoadingScreenPanel` is assigned in `LoadingScreen` component
- Verify `MainMenuProfileChecker` has `LoadingScreen` assigned
- Check Console for errors

### Menu elements still visible
- Verify all menu elements are added to "Menu Elements to Hide" array
- Check that elements are active in the scene
- Ensure element GameObjects are correct (not children)

### Loading screen doesn't hide
- Check that `ProfileSetupPopup` is being instantiated
- Verify Firebase is initializing correctly
- Check Console for Firebase errors

### Fade not working
- Ensure `CanvasGroup` component is on `LoadingScreenPanel`
- Check that fade durations are > 0

## Advanced: Custom Loading Animation

If you want a rotating spinner:

1. Create a spinner sprite/image
2. Add this script to `LoadingIndicator`:

```csharp
using UnityEngine;

public class RotateSpinner : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 200f;
    
    void Update()
    {
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
}
```

Or use DOTween for smooth rotation:

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

## Notes

- The loading screen automatically shows when `MainMenuProfileChecker` starts
- It automatically hides when profile check completes (whether profile exists or not)
- Menu elements are restored when loading screen hides
- The system works with or without a profile (first-time user or returning user)

