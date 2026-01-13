using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.Utils;
using Assets.Scripts.GameManager;
using UnityEngine.EventSystems;

/// <summary>
/// Controls language selection in the StartingLanguageSelection scene.
/// Sets language via the Language singleton which persists to PlayerPrefs
/// and automatically updates all LanguageComponent instances across scenes.
/// </summary>
public class StartingLanguageSelectionController : MonoBehaviour
{
    [Header("Language Buttons")]
    [SerializeField] private Button englishButton;
    [SerializeField] private Button portugueseButton;
    [SerializeField] private Button indonesianButton;
    [SerializeField] private Button continueButton;

    [Header("Visual Feedback - Selected State Highlights")]
    [Tooltip("Optional highlight images that show which language is selected (e.g., border glow)")]
    [SerializeField] private GameObject englishHighlight;
    [SerializeField] private GameObject portugueseHighlight;
    [SerializeField] private GameObject indonesianHighlight;

    private SystemLanguage selectedLanguage = SystemLanguage.English;

    /// <summary>
    /// Callback triggered when the language selection is finalized.
    /// </summary>
    public System.Action OnComplete;

    private void Start()
    {
        // Restore previously saved language (if any) from Language singleton
        selectedLanguage = Language.GetInstance().Id;
        UpdateHighlights();

        // Setup button listeners
        if (englishButton != null)
            englishButton.onClick.AddListener(OnSelectEnglish);
        
        if (portugueseButton != null)
            portugueseButton.onClick.AddListener(OnSelectPortuguese);
        
        if (indonesianButton != null)
            indonesianButton.onClick.AddListener(OnSelectIndonesian);
        
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);

        // Set initial button selection state
        Button initialButton = GetButtonForLanguage(selectedLanguage);
        if (initialButton != null)
        {
            initialButton.Select();
        }
    }

    private void Update()
    {
        // Sticky Selection: If the user clicks nothing (background), restore selection to the current language button
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
        {
            Button currentButton = GetButtonForLanguage(selectedLanguage);
            if (currentButton != null)
            {
                currentButton.Select();
            }
        }
    }

    private Button GetButtonForLanguage(SystemLanguage lang)
    {
        switch (lang)
        {
            case SystemLanguage.English: return englishButton;
            case SystemLanguage.Portuguese: return portugueseButton;
            case SystemLanguage.Indonesian: return indonesianButton;
            default: return null;
        }
    }

    private void OnSelectEnglish()
    {
        Debug.Log("StartingLanguageSelection: Selected English");
        SelectLanguage(SystemLanguage.English);
    }

    private void OnSelectPortuguese()
    {
        Debug.Log("StartingLanguageSelection: Selected Portuguese");
        SelectLanguage(SystemLanguage.Portuguese);
    }

    private void OnSelectIndonesian()
    {
        Debug.Log("StartingLanguageSelection: Selected Indonesian");
        SelectLanguage(SystemLanguage.Indonesian);
    }

    /// <summary>
    /// Sets the language in the Language singleton (which saves to PlayerPrefs)
    /// and updates visual highlights.
    /// </summary>
    private void SelectLanguage(SystemLanguage lang)
    {
        selectedLanguage = lang;
        
        // This saves to PlayerPrefs and broadcasts to all LanguageComponents
        Language.GetInstance().Set(lang);
        
        UpdateHighlights();

        // Ensure the button stays selected in the EventSystem
        Button btn = GetButtonForLanguage(lang);
        if (btn != null)
        {
            btn.Select();
        }
        
        // Play button sound if PersistentAudioManager exists
        if (PersistentAudioManager.Instance != null)
        {
            PersistentAudioManager.Instance.PlayClickSound();
        }
    }

    /// <summary>
    /// Updates visual highlights to show which language is currently selected.
    /// </summary>
    private void UpdateHighlights()
    {
        if (englishHighlight != null)
            englishHighlight.SetActive(selectedLanguage == SystemLanguage.English);
        
        if (portugueseHighlight != null)
            portugueseHighlight.SetActive(selectedLanguage == SystemLanguage.Portuguese);
        
        if (indonesianHighlight != null)
            indonesianHighlight.SetActive(selectedLanguage == SystemLanguage.Indonesian);
    }

    /// <summary>
    /// Called when Continue button is pressed. Finalizes selection and notifies owner.
    /// </summary>
    private void OnContinue()
    {
        Debug.Log($"StartingLanguageSelection: Continuing with language {selectedLanguage}");
        
        if (PersistentAudioManager.Instance != null)
        {
            PersistentAudioManager.Instance.PlayClickSound();
        }
        
        // Mark language as configured
        PlayerPrefs.SetInt("LanguageConfigured", 1);
        PlayerPrefs.Save();

        // Notify and clean up
        OnComplete?.Invoke();
        Destroy(gameObject);
    }
}
