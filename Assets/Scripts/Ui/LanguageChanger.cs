using UnityEngine;
using Assets.Scripts.Utils;

public class LanguageChanger : MonoBehaviour
{
    /// <summary>
    /// Call this method to change to English
    /// </summary>
    public void SetEnglish()
    {
        Debug.Log("Setting language to English");
        Language.GetInstance().Set(SystemLanguage.English);

        // Force UI refresh
        RefreshAllLanguageComponents();
    }

    /// <summary>
    /// Call this method to change to Portuguese (Portugal)
    /// </summary>
    public void SetPortuguese()
    {
        Debug.Log("Setting language to Portuguese");
        Language.GetInstance().Set(SystemLanguage.Portuguese);

        // Force UI refresh
        RefreshAllLanguageComponents();
    }

    /// <summary>
    /// Call this method to change to Portuguese (Brazil)
    /// </summary>
    public void SetBrazilianPortuguese()
    {
        Debug.Log("Setting language to Brazilian Portuguese");
        Language.GetInstance().SetBrazilianPortuguese();

        // Force UI refresh
        RefreshAllLanguageComponents();
    }

    /// <summary>
    /// Call this method to change to Indonesian
    /// </summary>
    public void SetIndonesian()
    {
        Debug.Log("Setting language to Indonesian");
        Language.GetInstance().Set(SystemLanguage.Indonesian);

        // Force UI refresh
        RefreshAllLanguageComponents();
    }

    /// <summary>
    /// Call this method to change to Russian
    /// </summary>
    public void SetRussian()
    {
        Debug.Log("Setting language to Russian");
        Language.GetInstance().Set(SystemLanguage.Russian);

        // Force UI refresh
        RefreshAllLanguageComponents();
    }

    /// <summary>
    /// Call this method to change to Japanese
    /// </summary>
    public void SetJapanese()
    {
        Debug.Log("Setting language to Japanese");
        Language.GetInstance().Set(SystemLanguage.Japanese);

        // Force UI refresh
        RefreshAllLanguageComponents();
    }

    /// <summary>
    /// Call this method to change to Chinese
    /// </summary>
    public void SetChinese()
    {
        Debug.Log("Setting language to Chinese");
        Language.GetInstance().Set(SystemLanguage.ChineseSimplified);

        // Force UI refresh
        RefreshAllLanguageComponents();
    }

    /// <summary>
    /// Force refresh all LanguageComponent scripts in the scene
    /// </summary>
    private void RefreshAllLanguageComponents()
    {
        // Find all LanguageComponent instances and refresh them
        LanguageComponent[] components = FindObjectsOfType<LanguageComponent>();
        foreach (LanguageComponent component in components)
        {
            // Trigger the refresh by calling SetText with current ID
            string currentId = component.m_id;
            component.SetText(currentId);
        }

        Debug.Log($"Refreshed {components.Length} language components");
    }

    /// <summary>
    /// Debug method to check current language
    /// </summary>
    public void CheckCurrentLanguage()
    {
        SystemLanguage current = Language.GetInstance().Id;
        int savedLang = PlayerPrefs.GetInt("LocalData_LanguageId", -1);

        Debug.Log($"Current Language: {current}");
        Debug.Log($"Saved Language Code: {savedLang}");

        // Test if language data is loaded
        string testText = Language.GetText("TXT_PLAY_BUTTON");
        Debug.Log($"Test text for 'TXT_PLAY_BUTTON': {testText}");
    }
}