using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GameManager;

public class SoundToggleButton : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private Button soundButton;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    
    [SerializeField] private Button backgroundSoundButton;
    [SerializeField] private Sprite backgroundSoundOnSprite;
    [SerializeField] private Sprite backgroundSoundOffSprite;

    [Header("Vibration Settings")]
    [SerializeField] private GameObject VibrationOn;
    [SerializeField] private Button VibrationButton;
    [SerializeField] private Sprite VibrationButtonOnSprite;
    [SerializeField] private Sprite VibrationButtonOffSprite;

    // We no longer need local AudioSource references as the Manager handles it.
    // Kept just in case there are legacy assignments, but they won't be used for logic.
    // [SerializeField] private AudioSource[] audioSources; 
    // [SerializeField] private AudioSource backgroundAudioSource;

    private void Start()
    {
        UpdateUI();

        if (soundButton != null)
            soundButton.onClick.AddListener(ToggleSFX);

        if (backgroundSoundButton != null)
            backgroundSoundButton.onClick.AddListener(ToggleBG);

        if (VibrationButton != null)
            VibrationButton.onClick.AddListener(ToggleVibration);
    }

    private void UpdateUI()
    {
        // Get state from PersistentAudioManager (1 = On, 0 = Off)
        int musicState = PersistentAudioManager.Instance.Switch_bg;
        int effectState = PersistentAudioManager.Instance.Switch_eff;
        bool isVibrationOn = PlayerPrefs.GetInt("VibrationOn", 1) == 1;

        // Update BG Button
        if (backgroundSoundButton != null)
        {
            Image btnImage = backgroundSoundButton.GetComponent<Image>();
            if (btnImage != null)
                btnImage.sprite = (musicState == 1) ? backgroundSoundOnSprite : backgroundSoundOffSprite;
        }

        // Update SFX Button
        if (soundButton != null)
        {
            Image btnImage = soundButton.GetComponent<Image>();
            if (btnImage != null)
                btnImage.sprite = (effectState == 1) ? soundOnSprite : soundOffSprite;
        }

        // Update Vibration Button
        if (VibrationButton != null)
        {
            Image btnImage = VibrationButton.GetComponent<Image>();
            if (btnImage != null)
                btnImage.sprite = isVibrationOn ? VibrationButtonOnSprite : VibrationButtonOffSprite;
        }
        
        if (VibrationOn != null)
             VibrationOn.SetActive(isVibrationOn);
    }

    private void ToggleSFX()
    {
        int currentState = PersistentAudioManager.Instance.Switch_eff;
        int newState = (currentState == 1) ? 0 : 1;
        
        PersistentAudioManager.Instance.SetEffectSwitch(newState);
        
        // Play click sound only if we are turning it ON or if it was already ON (but we just turned it off, so maybe no sound? usually UI generic click always plays, but here we respect the setting immediately or before?)
        // If we just turned it OFF, maybe we shouldn't play sound? 
        // Or we play it to acknowledge the press?
        // Let's play it if the NEW state is ON.
        if (newState == 1) 
            PersistentAudioManager.Instance.PlayClickSound();

        UpdateUI();
    }

    private void ToggleBG()
    {
        int currentState = PersistentAudioManager.Instance.Switch_bg;
        int newState = (currentState == 1) ? 0 : 1;

        PersistentAudioManager.Instance.SetMusicSwitch(newState);
        PersistentAudioManager.Instance.PlayClickSound(); // Click sound is SFX, so it should play regardless of BG setting (if SFX is on)

        UpdateUI();
    }

    private void ToggleVibration()
    {
        bool isVibrationOn = PlayerPrefs.GetInt("VibrationOn", 1) == 1;
        isVibrationOn = !isVibrationOn;
        
        PlayerPrefs.SetInt("VibrationOn", isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();
        
        PersistentAudioManager.Instance.PlayClickSound();

        if (isVibrationOn && FindObjectOfType<SimpleVibration>() != null)
        {
            FindObjectOfType<SimpleVibration>().VibrateSoft();
        }

        UpdateUI();
    }
}
