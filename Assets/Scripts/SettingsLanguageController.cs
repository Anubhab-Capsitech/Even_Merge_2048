using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
using Assets.Scripts.GameManager;

/// <summary>
/// Controls the language selection UI within the Settings panel.
/// Reads current language from the Language singleton (which persists to PlayerPrefs)
/// and updates the flag display accordingly.
/// </summary>
public class SettingsLanguageController : MonoBehaviour
{
    [Header("Main UI References")]
    [SerializeField] private Button arrowButton;
    [SerializeField] private GameObject languagePanel;
    [SerializeField] private Image flagImage;

    [Header("Language Selection Buttons")]
    [SerializeField] private Button englishButton;
    [SerializeField] private Button portugueseButton;
    [SerializeField] private Button indonesianButton;

    [Header("Flag Sources (used to pull sprites from)")]
    [SerializeField] private Image englishFlagSourceImage;
    [SerializeField] private Image portugueseFlagSourceImage;
    [SerializeField] private Image indonesianFlagSourceImage;

    [SerializeField] private Sprite englishFlagSprite;
    [SerializeField] private Sprite portugueseFlagSprite;
    [SerializeField] private Sprite indonesianFlagSprite;

    private void Start()
    {
        // Hide panel initially (safe check)
        if (languagePanel != null)
            languagePanel.SetActive(false);

        // Hook up UI listeners
        SetupButtonListeners();

        // Restore the previously selected language (or default) and update flag UI
        RestoreSavedLanguageToUI();
    }

    private void SetupButtonListeners()
    {
        if (arrowButton != null)
            arrowButton.onClick.AddListener(ToggleLanguagePanel);

        if (englishButton != null)
            englishButton.onClick.AddListener(OnSelectEnglish);

        if (portugueseButton != null)
            portugueseButton.onClick.AddListener(OnSelectPortuguese);

        if (indonesianButton != null)
            indonesianButton.onClick.AddListener(OnSelectIndonesian);
    }

    /// <summary>
    /// Toggle the visibility of the small language panel.
    /// </summary>
    public void ToggleLanguagePanel()
    {
        if (languagePanel != null)
        {
            languagePanel.SetActive(!languagePanel.activeSelf);
            PersistentAudioManager.Instance?.PlayEffect("sound_eff_button");
        }
    }

    // ---------- Selection handlers ---------- //

    public void OnSelectEnglish()
    {
        Debug.Log("Settings: Selecting English");
        SetLanguageAndUI(SystemLanguage.English);
    }

    public void OnSelectPortuguese()
    {
        Debug.Log("Settings: Selecting Portuguese");
        SetLanguageAndUI(SystemLanguage.Portuguese);
    }

    public void OnSelectIndonesian()
    {
        Debug.Log("Settings: Selecting Indonesian");
        SetLanguageAndUI(SystemLanguage.Indonesian);
    }

    /// <summary>
    /// Centralized method: set the language in your Language manager, update the left-side flag,
    /// save to PlayerPrefs, hide the panel and refresh other LanguageComponents.
    /// </summary>
    private void SetLanguageAndUI(SystemLanguage lang)
    {
        // 1) Set language in your Language manager (this saves to PlayerPrefs internally)
        Language.GetInstance().Set(lang);

        // 2) Update the left side flag image immediately
        SetFlagImageForLanguage(lang);

        // 3) Play click sound and hide panel
        PersistentAudioManager.Instance?.PlayEffect("sound_eff_button");
        
        if (languagePanel != null)
            languagePanel.SetActive(false);

        // 4) Refresh other language components (text, labels) in scene
        RefreshAllLanguageComponents();
    }

    /// <summary>
    /// Set the left flag Image sprite based on chosen language.
    /// </summary>
    private void SetFlagImageForLanguage(SystemLanguage lang)
    {
        if (flagImage == null)
        {
            Debug.LogWarning("SettingsLanguageController: flagImage (left-side) is not assigned in inspector!");
            return;
        }

        Sprite chosen = null;

        switch (lang)
        {
            case SystemLanguage.English:
                chosen = englishFlagSprite != null ? englishFlagSprite : englishFlagSourceImage?.sprite;
                break;

            case SystemLanguage.Portuguese:
                chosen = portugueseFlagSprite != null ? portugueseFlagSprite : portugueseFlagSourceImage?.sprite;
                break;

            case SystemLanguage.Indonesian:
                chosen = indonesianFlagSprite != null ? indonesianFlagSprite : indonesianFlagSourceImage?.sprite;
                break;

            default:
                chosen = englishFlagSprite != null ? englishFlagSprite : englishFlagSourceImage?.sprite;
                break;
        }

        if (chosen != null)
        {
            flagImage.sprite = chosen;
            flagImage.preserveAspect = true;
        }
        else
        {
            Debug.LogWarning("SettingsLanguageController: no sprite found for language " + lang);
        }
    }
    
    /// <summary>
    /// On startup, read the current language from the Language singleton and update the flag UI.
    /// The Language singleton already restores from PlayerPrefs in its Init(), so we just read from it.
    /// </summary>
    private void RestoreSavedLanguageToUI()
    {
        // Simply read from the Language singleton - it is already initialized from PlayerPrefs
        SystemLanguage current = Language.GetInstance().Id;
        
        Debug.Log("SettingsLanguageController: Restoring UI for language " + current);
        
        // Update the flag image to match
        SetFlagImageForLanguage(current);
    }

    /// <summary>
    /// Refresh all LanguageComponent instances in the scene so texts update to the new language.
    /// </summary>
    private void RefreshAllLanguageComponents()
    {
        LanguageComponent[] components = FindObjectsOfType<LanguageComponent>();

        foreach (LanguageComponent component in components)
        {
            component.SetText(component.m_id);
        }

        Debug.Log("Settings: Refreshed " + components.Length + " language components");
    }
}
