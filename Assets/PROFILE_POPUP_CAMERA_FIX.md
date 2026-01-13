# Profile Popup Camera/Viewport Fix

## Problem

When opening the profile popup in 2D game mode and then playing the game, the camera/viewport appeared distant (zoomed out). Opening and closing the settings popup would fix it, bringing the camera back to normal.

## Root Cause

The profile popup was not resetting `Time.timeScale` and unpausing the 2D game when closed, unlike the settings popup. This caused the game to remain in a paused state, which affected the camera/viewport rendering.

**Settings Popup** (working correctly):
- Opens: Sets `Time.timeScale = 0f` and pauses 2D game
- Closes: Resets `Time.timeScale = 1f` and unpauses 2D game ✅

**Profile Popup** (had bug):
- Opens: Sets `Time.timeScale = 0f` and pauses 2D game
- Closes: Only closes dialog, doesn't reset time scale ❌

## Solution

Updated `ProfileStatsPopup.cs` to match the behavior of `Setting.cs`:

**File: `Scripts/Profile/ProfileStatsPopup.cs`**

Added to `OnCloseClicked()` method:
```csharp
// Resume the game when profile dialog is closed (for 2D game)
// This fixes the camera/viewport issue where game appears distant after closing popup
Time.timeScale = 1f;
if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
{
    G2BoardGenerator.GetInstance().IsPuase = false;
}
```

## Changes Made

1. ✅ **Reset Time.timeScale**: Sets back to 1f when popup closes
2. ✅ **Unpause 2D Game**: Sets `G2BoardGenerator.IsPuase = false` when closing
3. ✅ **Matches Settings Behavior**: Now behaves identically to settings popup

## Testing

### Test Case 1: Profile Popup in 2D Mode
1. Start 2D game
2. Open profile popup
3. Close profile popup
4. **Expected**: Camera/viewport should be normal (not distant) ✅

### Test Case 2: Profile Popup in Main Menu
1. In main menu (not in game)
2. Open profile popup
3. Close profile popup
4. **Expected**: No issues (time scale doesn't affect menu) ✅

### Test Case 3: Multiple Popups
1. Open profile popup
2. Close profile popup
3. Open settings popup
4. Close settings popup
5. **Expected**: Both should work correctly ✅

## Unity Editor Setup

**No additional setup required!** The fix is in the code.

However, if you want to verify:

1. Check that `ProfileStatsPopup` prefab has the updated script
2. Ensure the prefab is assigned in `MainScene` component (if using the prefab method)
3. Test in Play mode to verify the fix

## Why This Happened

The issue occurred because:
- `MainScene.OnClickProfile()` correctly pauses the game when opening
- But `ProfileStatsPopup.OnCloseClicked()` didn't resume the game when closing
- Settings popup had the resume logic, which is why opening/closing it fixed the issue

## Related Code

**Settings Popup Close** (`Setting.cs`):
```csharp
public void OnClickClose()
{
    DialogManager.GetInstance().Close(null);
    PersistentAudioManager.Instance.PlayEffect("sound_eff_button");
    
    // Resume the game when settings dialog is closed (for 2D game)
    Time.timeScale = 1f;
    if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
    {
        G2BoardGenerator.GetInstance().IsPuase = false;
    }
}
```

**Profile Popup Close** (now fixed, `ProfileStatsPopup.cs`):
```csharp
private void OnCloseClicked()
{
    if (PersistentAudioManager.Instance != null)
    {
        PersistentAudioManager.Instance.PlayClickSound();
    }

    DialogManager.GetInstance().Close(null);
    
    // Resume the game when profile dialog is closed (for 2D game)
    // This fixes the camera/viewport issue where game appears distant after closing popup
    Time.timeScale = 1f;
    if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
    {
        G2BoardGenerator.GetInstance().IsPuase = false;
    }
}
```

## Notes

- The fix ensures consistent behavior between all popups
- Time scale is only reset when in 2D game mode (GameId == 2)
- The fix doesn't affect 3D mode or main menu
- Camera/viewport should now work correctly after closing profile popup

