using UnityEngine;
using Assets.Scripts.Configs;

namespace Assets.Scripts.GameManager
{
	/// <summary>
	/// Persistent audio manager that persists across scene changes.
	/// Handles background music and sound effects globally.
	/// </summary>
	public class PersistentAudioManager : MonoBehaviour
	{
		private static PersistentAudioManager s_instance;

		public AudioSource audio_bg;
		public AudioSource audio_eff;

        [Header("Default Clips")]
        public AudioClip clickClip; // Assign in Inspector

        [Header("Default Clips")]
        public AudioClip cubeCollisionClip; // Assign in Inspector

        private int switch_bg = 1;
		private int switch_eff = 1;

		private float bgMusicTime = 0f; // Track music playback position

		private void Awake()
		{
			if (s_instance == null)
			{
				s_instance = this;
				DontDestroyOnLoad(gameObject);
				InitializeAudioSources();
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			LoadAudioSettings();
			PlayBgMusic();
		}

		public static PersistentAudioManager GetInstance()
		{
			if (s_instance == null)
			{
				GameObject audioObject = new GameObject("PersistentAudioManager");
				s_instance = audioObject.AddComponent<PersistentAudioManager>();
			}
			return s_instance;
		}

		public static PersistentAudioManager Instance => GetInstance(); // Shorthand

		private void InitializeAudioSources()
		{
			if (audio_bg == null)
			{
				audio_bg = gameObject.GetComponent<AudioSource>();
				if (audio_bg == null)
				{
					audio_bg = gameObject.AddComponent<AudioSource>();
					audio_bg.loop = true;
					audio_bg.playOnAwake = false;
				}
			}

			if (audio_eff == null)
			{
				// Create a separate audio source for effects
				GameObject effObject = new GameObject("EffectAudioSource");
				effObject.transform.SetParent(transform);
				audio_eff = effObject.AddComponent<AudioSource>();
				audio_eff.loop = false;
				audio_eff.playOnAwake = false;
			}
		}

		private void LoadAudioSettings()
		{
			switch_bg = PlayerPrefs.GetInt("LocalData_Music", 1);
			switch_eff = PlayerPrefs.GetInt("LocalData_Effect", 1);
		}

		public int Switch_bg
		{
			get { return switch_bg; }
			set { switch_bg = value; }
		}

		public int Switch_eff
		{
			get { return switch_eff; }
			set { switch_eff = value; }
		}

		public void PlayBgMusic()
		{
			if (switch_bg == 1)
			{
				if (!audio_bg.isPlaying)
				{
					audio_bg.Play();
				}
			}
			else
			{
				StopBgMusic();
			}
		}

		public void PlayBgMusic(string idx, bool isLoop = false)
		{
			if (!Configs.Configs.TSounds.ContainsKey(idx))
			{
				return;
			}

			if (switch_bg == 1)
			{
				AudioClip clip = Resources.Load(Configs.Configs.TSounds[idx].Path, typeof(AudioClip)) as AudioClip;
				if (clip != null)
				{
					audio_bg.clip = clip;
					audio_bg.loop = isLoop;
					audio_bg.Play();
				}
			}
		}

		public void StopBgMusic()
		{
			if (audio_bg != null)
			{
				audio_bg.Stop();
			}
		}

		public void PauseBgMusic()
		{
			if (audio_bg != null)
			{
				audio_bg.Pause();
			}
		}

		public void ResumeBgMusic()
		{
			if (audio_bg != null && switch_bg == 1)
			{
				audio_bg.Play();
			}
		}

		public void PlayEffect(string idx)
		{
			if (switch_eff == 0)
			{
				return;
			}

			if (!Configs.Configs.TSounds.ContainsKey(idx))
			{
				return;
			}

			AudioClip clip = Resources.Load(Configs.Configs.TSounds[idx].Path, typeof(AudioClip)) as AudioClip;
			if (clip != null)
			{
				audio_eff.PlayOneShot(clip);
			}
		}

        public void PlayClickSound()
        {
            if (switch_eff == 0) return;

            if (clickClip != null)
            {
                audio_eff.PlayOneShot(clickClip);
            }
            else
            {
               // Fallback if specific clip isn't assigned, try loading a common default
               // or just log warning.
               // For now, let's try to load "sound_eff_click" from resources if it exists in configs,
               // otherwise just return.
               if (Configs.Configs.TSounds.ContainsKey("sound_eff_click"))
               {
                   PlayEffect("sound_eff_click");
               }
            }
        }

        public void PlayCubeCollisionSound()
        {
            if (switch_eff == 0) return;

            if (clickClip != null)
            {
                audio_eff.PlayOneShot(cubeCollisionClip);
            }
            else
            {
                // Fallback if specific clip isn't assigned, try loading a common default
                // or just log warning.
                // For now, let's try to load "sound_eff_click" from resources if it exists in configs,
                // otherwise just return.
                if (Configs.Configs.TSounds.ContainsKey("sound_eff_click"))
                {
                    PlayEffect("sound_eff_click");
                }
            }
        }

        public void SetMusicSwitch(int value)
		{
			switch_bg = value;
			PlayerPrefs.SetInt("LocalData_Music", value);
			PlayerPrefs.Save();

			if (value == 0)
			{
				StopBgMusic();
			}
			else
			{
				PlayBgMusic();
			}
		}

		public void SetEffectSwitch(int value)
		{
			switch_eff = value;
			PlayerPrefs.SetInt("LocalData_Effect", value);
			PlayerPrefs.Save();
		}

		public bool IsBgMusicPlaying()
		{
			return audio_bg != null && audio_bg.isPlaying;
		}

		public void SetBgMusicVolume(float volume)
		{
			if (audio_bg != null)
			{
				audio_bg.volume = Mathf.Clamp01(volume);
			}
		}

		public void SetEffectVolume(float volume)
		{
			if (audio_eff != null)
			{
				audio_eff.volume = Mathf.Clamp01(volume);
			}
		}
	}
}
