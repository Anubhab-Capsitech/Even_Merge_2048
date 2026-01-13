using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GameManager;

public class SettingsPanel3D : MonoBehaviour
{
    public static SettingsPanel3D Instance { get; private set; }

    [Header("Panel")]
    public GameObject settingsPanel;
    public GameObject backgroundOverlay;

    [Header("Audio Sprites")]
    public Sprite m_asset_switch_off;
    public Sprite m_asset_switch_on;

    [Header("Audio Images")]
    public Image m_img_music_switch;
    public Image m_img_effect_switch;

    [Header("Buttons")]
    public Button closeButton;

    private PersistentAudioManager audioManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);
    }

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        audioManager = PersistentAudioManager.Instance;
        InitUI();
    }

    public void InitUI()
    {
        if (audioManager == null) return;

        m_img_music_switch.sprite =
            audioManager.Switch_bg == 1 ? m_asset_switch_on : m_asset_switch_off;

        m_img_effect_switch.sprite =
            audioManager.Switch_eff == 1 ? m_asset_switch_on : m_asset_switch_off;
    }

    // ---------------- MUSIC ----------------

    public void OnClickMusicBtn()
    {
        if (audioManager == null) return;

        bool musicWasOn = audioManager.Switch_bg == 1;

        if (musicWasOn)
        {
            audioManager.SetMusicSwitch(0);
            audioManager.StopBgMusic();
            PlayerAnalytics.Instance?.MusicSwitched(false);
        }
        else
        {
            audioManager.SetMusicSwitch(1);
            audioManager.PlayBgMusic();
            PlayerAnalytics.Instance?.MusicSwitched(true);
        }

        InitUI();
    }

    // ---------------- EFFECT ----------------

    public void OnClickEffectBtn()
    {
        if (audioManager == null) return;

        bool effectWasOn = audioManager.Switch_eff == 1;

        if (effectWasOn)
        {
            audioManager.SetEffectSwitch(0);
            PlayerAnalytics.Instance?.SoundSwitched(false);
        }
        else
        {
            audioManager.SetEffectSwitch(1);
            PlayerAnalytics.Instance?.SoundSwitched(true);
            audioManager.PlayClickSound();
        }

        InitUI();
    }

    // ---------------- OPEN / CLOSE ----------------

    public void OpenSettings()
    {
        settingsPanel?.SetActive(true);
        backgroundOverlay?.SetActive(true);

        InitUI();

        if (audioManager.Switch_eff == 1)
            audioManager.PlayClickSound();

        Time.timeScale = 0f;
    }

    public void CloseSettings()
    {
        settingsPanel?.SetActive(false);
        backgroundOverlay?.SetActive(false);

        if (audioManager.Switch_eff == 1)
            audioManager.PlayClickSound();

        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        Time.timeScale = 1f;
    }
}
