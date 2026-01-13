# Manual Integration Instructions for Persistent Audio System

## Overview
Due to API rate limiting, the final AudioManager.cs update must be done manually. This document provides step-by-step instructions.

---

## MANUAL STEP: Update AudioManager.cs

### Location
`Assets/Scripts/Assets_Scripts_GameManager/AudioManager.cs`

### Instructions

1. **Open AudioManager.cs in your code editor** (Visual Studio, VS Code, etc.)

2. **Find the `Init()` method** (around line 51)

3. **Replace the Init() method with:**
```csharp
public void Init()
{
    // Get persistent audio manager for cross-scene persistence
    PersistentAudioManager persistentAudio = PersistentAudioManager.GetInstance();
    this.Switch_bg = persistentAudio.Switch_bg;
    this.Switch_eff = persistentAudio.Switch_eff;
    
    // Use persistent background audio source
    this.audio_bg = persistentAudio.audio_bg;
    this.PlayBgMusic();
}
```

4. **Find the `PlayBgMusic()` method** (around line 66)

5. **Replace it with:**
```csharp
public void PlayBgMusic()
{
    PersistentAudioManager.GetInstance().PlayBgMusic();
}
```

6. **Find the `PlayBgMusic(string idx, bool isLoop = false)` method** (around line 74)

7. **Replace it with:**
```csharp
public void PlayBgMusic(string idx, bool isLoop = false)
{
    PersistentAudioManager.GetInstance().PlayBgMusic(idx, isLoop);
}
```

8. **Find the `StopBgMusic()` method** (around line 85)

9. **Replace it with:**
```csharp
public void StopBgMusic()
{
    PersistentAudioManager.GetInstance().StopBgMusic();
}
```

10. **Find the `PauseBgMusic()` method** (around line 90)

11. **Replace it with:**
```csharp
public void PauseBgMusic()
{
    PersistentAudioManager.GetInstance().PauseBgMusic();
}
```

12. **Find the `SetMusicSwitch(int _switch)` method** (around line 115)

13. **Replace it with:**
```csharp
public void SetMusicSwitch(int _switch)
{
    this.Switch_bg = _switch;
    PersistentAudioManager.GetInstance().SetMusicSwitch(_switch);
}
```

14. **Find the `SetEffectSwitch(int _switch)` method** (around line 121)

15. **Replace it with:**
```csharp
public void SetEffectSwitch(int _switch)
{
    this.Switch_eff = _switch;
    PersistentAudioManager.GetInstance().SetEffectSwitch(_switch);
}
```

16. **Keep the `PlayEffect()` method as-is** - it already handles local effect sounds correctly

17. **Save the file** (Ctrl+S or Cmd+S)

---

## Quick Copy-Paste: Complete Updated AudioManager.cs

If you prefer, you can replace the entire AudioManager.cs content with this:

```csharp
using Assets.Scripts.Configs;
using System;
using UnityEngine;

namespace Assets.Scripts.GameManager
{
	internal class AudioManager : MonoBehaviour
	{
		private static AudioManager g_intance;

		public AudioSource audio_bg;
		public AudioClip[] audioClip;

		public AudioSource audio_eff;

		private int switch_bg;

		private int switch_eff;

		public int Switch_bg
		{
			get
			{
				return this.switch_bg;
			}
			set
			{
				this.switch_bg = value;
			}
		}

		public int Switch_eff
		{
			get
			{
				return this.switch_eff;
			}
			set
			{
				this.switch_eff = value;
			}
		}

		private void Awake()
		{
			AudioManager.g_intance = this;
		}

		private void Start()
		{
			this.Init();
		}

		public static AudioManager GetInstance()
		{
			return AudioManager.g_intance;
		}

		public void Init()
		{
			// Get persistent audio manager for cross-scene persistence
			PersistentAudioManager persistentAudio = PersistentAudioManager.GetInstance();
			this.Switch_bg = persistentAudio.Switch_bg;
			this.Switch_eff = persistentAudio.Switch_eff;
			
			// Use persistent background audio source
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
				AudioClip clip = Resources.Load(Configs.Configs.TSounds[idx].Path, typeof(AudioClip)) as AudioClip;
				if (idx == "Fit")
                {
                    clip = audioClip[1];
                }
				else
				{
                    clip = audioClip[0];
				}
				this.audio_eff.PlayOneShot(clip);
				this.audio_eff.loop = false;
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
	}
}
```

---

## Verify in Unity Editor

After updating AudioManager.cs:

1. **Save the file** and return to Unity
2. **Wait for compilation** (watch the bottom right of Unity Editor)
3. **Check Console** (Window > General > Console)
   - Should see no red errors
   - May see warnings but those are fine
4. **You're done!** The persistent audio system is now integrated

---

## Testing the System

### Test 1: Music Persistence on Scene Change
1. Play MainScene
2. Verify background music plays
3. Use Game Inspector to change to 3D game scene
4. **EXPECTED:** Music continues playing (doesn't stop/restart)

### Test 2: Music Toggle
1. Open MainScene
2. Open Settings panel
3. Click Music Toggle to turn OFF
4. **EXPECTED:** Music stops immediately
5. Click Music Toggle to turn ON
6. **EXPECTED:** Music resumes
7. Change to 3D scene
8. **EXPECTED:** Music continues

### Test 3: Settings Persistence
1. Open MainScene
2. Turn music OFF in Settings
3. Close app completely
4. Restart app
5. **EXPECTED:** Music is still OFF
6. Turn music ON in Settings
7. Close app
8. Restart app
9. **EXPECTED:** Music is still ON

### Test 4: Sound Effects
1. Play any sound effect in either scene
2. **EXPECTED:** Sound effect plays correctly
3. Toggle Effects OFF in Settings
4. Try to play a sound effect
5. **EXPECTED:** No sound plays

---

## Files Reference

### Core Files Created
- ? `Assets/Scripts/Assets_Scripts_GameManager/PersistentAudioManager.cs` - The persistent audio manager
- ? `Assets/Scripts/Assets_Scripts_GameManager/AudioManager_Updated.cs` - Reference for updated AudioManager

### Files to Manually Update
- ? `Assets/Scripts/Assets_Scripts_GameManager/AudioManager.cs` - Update following instructions above

### Documentation Files
- ?? `Assets/Scripts/Assets_Scripts_GameManager/AUDIO_SYSTEM_README.md` - Detailed documentation
- ?? `Assets/PERSISTENT_AUDIO_IMPLEMENTATION_STATUS.md` - Implementation summary
- ?? `Assets/MANUAL_INTEGRATION_INSTRUCTIONS.md` - **This file**

---

## Troubleshooting

### Error: "The name 'PersistentAudioManager' does not exist"
**Solution:** PersistentAudioManager.cs file is missing or in wrong location
- Check that `Assets/Scripts/Assets_Scripts_GameManager/PersistentAudioManager.cs` exists
- If not, copy it from project files
- Make sure namespace is correct: `Assets.Scripts.GameManager`

### Error: "Compilation errors in AudioManager"
**Solution:** Syntax error during update
- Check indentation (use tabs or consistent spaces)
- Ensure all curly braces { } are matched
- Verify method signatures match exactly

### Music not playing after update
**Solution:** AudioManager initialization issue
- Verify PersistentAudioManager is being created
- Check that audio clip is assigned
- Ensure PlayerPrefs has "LocalData_Music" = 1
- Check Console for any errors

### Duplicate AudioManager instances
**Solution:** This shouldn't happen with correct updates
- Search scene for multiple AudioManager components
- Keep only one AudioManager in MainScene
- PersistentAudioManager should only appear once (singleton)

---

## Next Steps

After successfully updating AudioManager.cs:

1. ? Build your project
2. ? Test music persistence across scenes
3. ? Test music/effect toggles
4. ? Test settings persistence after app restart
5. ?? You're done! Your audio system is now persistent

---

## Questions?

Refer to these documentation files:
- `Assets/Scripts/Assets_Scripts_GameManager/AUDIO_SYSTEM_README.md` - Comprehensive system docs
- `Assets/Scripts/Assets_Scripts_GameManager/PersistentAudioManager.cs` - Source code with comments
- `Assets/PERSISTENT_AUDIO_IMPLEMENTATION_STATUS.md` - Overall status and flow diagrams
