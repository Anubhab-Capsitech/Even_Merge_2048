using Assets.Scripts.GameManager;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
	private PersistentAudioManager audioManager;

	public Sprite m_asset_switch_off;

	public Sprite m_asset_switch_on;

	public Image m_img_music_switch;

	public Image m_img_effect_switch;

	private void Start()
	{
		this.Init();
	}

	public void Init()
	{
		this.audioManager = PersistentAudioManager.Instance;
		this.InitUI();
	}

	public void InitUI()
	{
		this.m_img_music_switch.sprite = ((this.audioManager.Switch_bg == 1) ? this.m_asset_switch_on : this.m_asset_switch_off);
		this.m_img_effect_switch.sprite = ((this.audioManager.Switch_eff == 1) ? this.m_asset_switch_on : this.m_asset_switch_off);
	}

	public void OnClickMusicBtn()
	{
		if (audioManager == null)
		{
			Debug.LogWarning("PersistentAudioManager is not assigned!");
			return;
		}

        PersistentAudioManager.Instance?.PlayEffect("sound_eff_button");

        bool musicWasOn = audioManager.Switch_bg == 1;

		if (musicWasOn)
		{
			audioManager.SetMusicSwitch(0);
			audioManager.StopBgMusic();

			// Safe null check for analytics
			PlayerAnalytics.Instance?.MusicSwitched(false); // music off
		}
		else
		{
			audioManager.SetMusicSwitch(1);
			audioManager.PlayBgMusic();

			PlayerAnalytics.Instance?.MusicSwitched(true); // music on
		}

		// Always update UI
		InitUI();
	}


	public void OnClickEffectBtn()
	{
		if (audioManager == null)
		{
			Debug.LogWarning("PersistentAudioManager is not assigned!");
			return;
		}

        PersistentAudioManager.Instance.PlayEffect("sound_eff_button");

        bool effectWasOn = audioManager.Switch_eff == 1;

		if (effectWasOn)
		{
			audioManager.SetEffectSwitch(0);
			PlayerAnalytics.Instance?.SoundSwitched(false); // sound off
		}
		else
		{
			audioManager.SetEffectSwitch(1);
			PlayerAnalytics.Instance?.SoundSwitched(true); // sound on
		}

		InitUI();
	}

// privacy policy url function
	public void OnClickPrivacyPolicy()
{
    // Safely increment the click count only if FirebaseManager.Instance exists
    if (FirebaseManager.Instance == null)
    {
        Debug.LogWarning("FirebaseManager.Instance is null! Cannot log privacy policy click.");
        return;
    }

    PlayerAnalytics.Instance.PrivacyPolicyCount();  //firebase analytics - privacy policy click count

    // You can still open the privacy policy page here
    Application.OpenURL("https://your-privacy-policy-url.com");
}

	public void OnClickLanguage()
	{
		GameObject obj = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("Prefabs/setting_language") as GameObject);
		DialogManager.GetInstance().show(obj, false);
	}

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
}
