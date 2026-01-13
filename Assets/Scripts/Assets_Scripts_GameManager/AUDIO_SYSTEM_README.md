# Persistent Audio System Implementation Guide

## Overview
This guide explains how to implement a persistent audio system that works across both the 2D menu scene (MainScene) and the 3D game scene, ensuring background music continues playing uninterrupted when switching between scenes.

## What Has Been Implemented

### 1. **PersistentAudioManager** (NEW)
Location: `Assets/Scripts/Assets_Scripts_GameManager/PersistentAudioManager.cs`

**Key Features:**
- Singleton pattern with `DontDestroyOnLoad` to persist across scene changes
- Separate AudioSources for background music and sound effects
- Centralized control of all audio playback
- Automatic audio source creation if not assigned
- Methods:
  - `PlayBgMusic()` - Resume background music
  - `PlayBgMusic(string idx, bool isLoop)` - Play specific music track
  - `StopBgMusic()` - Stop music
  - `PauseBgMusic()` - Pause music
  - `ResumeBgMusic()` - Resume paused music
  - `PlayEffect(string idx)` - Play sound effect
  - `SetMusicSwitch(int value)` - Enable/disable music
  - `SetEffectSwitch(int value)` - Enable/disable effects
  - `SetBgMusicVolume(float volume)` - Control music volume
  - `SetEffectVolume(float volume)` - Control effect volume

---

## Remaining Implementation Steps

### Step 1: Update AudioManager
**File:** `Assets/Scripts/Assets_Scripts_GameManager/AudioManager.cs`

Replace the audio management methods to delegate to PersistentAudioManager:

```csharp
public void Init()
{
    // Sync with persistent audio manager
    PersistentAudioManager persistentAudio = PersistentAudioManager.GetInstance();
    this.switch_bg = persistentAudio.Switch_bg;
    this.switch_eff = persistentAudio.Switch_eff;
    
    // Use persistent background music source
    this.audio_bg = persistentAudio.audio_bg;
    this.PlayBgMusic();
}

public void PlayBgMusic()
{
    PersistentAudioManager.GetInstance().PlayBgMusic();
}

public void PlayBgMusic(string idx, bool isLoop = false)
{
    PersistentAudioManager.GetInstance().PlayBgMusic(idx, isLoop);
}

public void StopBgMusic()
{
    PersistentAudioManager.GetInstance().StopBgMusic();
}

public void PauseBgMusic()
{
    PersistentAudioManager.GetInstance().PauseBgMusic();
}

public void PlayEffect(string idx)
{
    if (!Configs.Configs.TSounds.ContainsKey(idx))
    {
        return;
    }
    if (this.Switch_eff == 1)
    {
        PersistentAudioManager.GetInstance().PlayEffect(idx);
    }
}

public void SetMusicSwitch(int _switch)
{
    this.Switch_bg = _switch;
    PersistentAudioManager.GetInstance().SetMusicSwitch(_switch);
}

public void SetEffectSwitch(int _switch)
{
    this.Switch_eff = _switch;
    PersistentAudioManager.GetInstance().SetEffectSwitch(_switch);
}
```

### Step 2: Update MainScene
**File:** `Assets/Scripts/MainScene.cs`

No changes needed! The `MainScene` already uses `AudioManager.GetInstance().PlayEffect()` which will now delegate to the persistent manager. Just ensure no direct `StopBgMusic()` calls when entering/leaving the scene.

### Step 3: Update Game2Manager (3D Scene)
**File:** `Assets/Scripts/Game2Manager.cs`

No changes needed! Like MainScene, it uses `AudioManager.GetInstance().PlayEffect()` which will work with the persistent system.

### Step 4: Update Setting Script
**File:** `Assets/Scripts/Setting.cs`

The Setting script is already correctly implemented with music and sound toggle logic. No changes needed.

### Step 5: Optional - Update UiManager
**File:** `Assets/Scripts/UiManager.cs`

If you want to pause music during scene transitions, add:

```csharp
public void OnMenuButton()
{
    GameObject.Find("ClickedSound").GetComponent<AudioSource>().Play();
    
    // Pause persistent music during scene transition
    PersistentAudioManager.GetInstance().PauseBgMusic();
    
    Reset3DGameState();
    PlayerPrefs.DeleteKey(GAMEOVER_STATE_KEY);
    PlayerPrefs.Save();
    
    SceneManager.LoadScene("MainScene 1");
}
```

Then resume music after scene load by calling `PersistentAudioManager.GetInstance().ResumeBgMusic()` in MainScene's Start.

---

## How It Works

### Audio Persistence Flow

1. **First Load (App Start)**
   - `Main.cs` initializes (game config)
   - `PersistentAudioManager` is created (singleton, DontDestroyOnLoad)
   - Loads audio settings from PlayerPrefs
   - Creates/initializes AudioSources

2. **MainScene Load**
   - `MainScene.Start()` initializes UI
   - Calls `AudioManager.GetInstance().PlayBgMusic()`
   - Which delegates to `PersistentAudioManager.PlayBgMusic()`
   - Music plays and continues playing

3. **Scene Transition to 3D Game**
   - Music continues playing (it's on persistent object)
   - `Game2Manager` can call effects via `AudioManager.GetInstance().PlayEffect()`
   - Music never stops

4. **Scene Transition Back to MainScene**
   - Same persistent music continues
   - MainScene UI resumes with music still playing

5. **Music/Effect Toggle in Settings**
   - Settings.OnClickMusicBtn() calls `SetMusicSwitch()`
   - Propagates through `AudioManager` to `PersistentAudioManager`
   - State saved in PlayerPrefs
   - Music stopped or resumed accordingly

---

## Key Design Decisions

1. **Why DontDestroyOnLoad?**
   - Ensures the audio manager persists across all scene loads
   - Music continues uninterrupted during transitions
   - Single instance prevents duplicates

2. **Separate AudioSources for Music vs Effects**
   - Music: Looping, persistent
   - Effects: One-shot, contextual
   - Prevents effects from interrupting music

3. **Delegation Pattern**
   - `AudioManager` remains as the public interface
   - Delegates to `PersistentAudioManager` internally
   - Maintains backward compatibility with existing code

4. **PlayerPrefs Integration**
   - Audio settings persist across app restarts
   - Toggle state saved automatically
   - Users' preferences respected

---

## Testing Checklist

- [ ] Background music plays on MainScene startup
- [ ] Music continues when transitioning to 3D game
- [ ] Music continues when returning to MainScene
- [ ] Sound effects play correctly in both scenes
- [ ] Music toggle in Settings works across scenes
- [ ] Sound effects toggle works correctly
- [ ] Volume controls work if implemented
- [ ] Music and effects respect user preferences after app restart
- [ ] No audio duplication or overlapping

---

## Troubleshooting

**Issue: Music not playing on startup**
- Check that PersistentAudioManager audio_bg source has a clip assigned
- Verify PlayerPrefs has "LocalData_Music" = 1
- Check that AudioManager is properly delegating

**Issue: Multiple instances of music**
- Ensure DontDestroyOnLoad is called in Awake
- Check singleton pattern implementation

**Issue: Scene-specific audio not working**
- Verify Setting script is calling correct methods
- Check that PlayEffect paths match your audio setup

---

## Future Enhancements

1. **Fade In/Out on Scene Transitions**
   ```csharp
   public void FadeMusicOut(float duration)
   {
       audio_bg.DOFade(0f, duration);
   }
   ```

2. **Scene-Specific Music**
   ```csharp
   // In MainScene.Start()
   persistentAudio.PlayBgMusic("menu_music", true);
   
   // In Game2Manager.Start()
   persistentAudio.PlayBgMusic("game_music", true);
   ```

3. **Music Playlist Queue**
   - Queue multiple tracks
   - Auto-advance to next track

4. **Volume Sliders in Settings**
   - Allow granular control over music/effect volume
   - Persist volume levels

---

## Summary

The persistent audio system is now implemented with:
? `PersistentAudioManager` created and ready to use
? Singleton pattern with DontDestroyOnLoad
? Separate audio sources for music and effects
? Automatic audio source initialization
? Full method coverage for all audio operations

**Next Steps:**
1. Update `AudioManager.cs` to delegate to `PersistentAudioManager`
2. Test audio persistence between scenes
3. Verify music and effect toggles work correctly
4. Test PlayerPrefs persistence across app restarts
