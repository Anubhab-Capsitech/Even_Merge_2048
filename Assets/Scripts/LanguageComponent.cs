using Assets.Scripts.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageComponent : MonoBehaviour
{
    private Text uiText;
    private TextMeshProUGUI tmpText;

    public string m_id = "TXT_NO_10001";
    public string m_suffix = "";
    public float m_linespace_ru;
    public float m_linespace_en;
    public float m_linespace_pt;      // Portuguese line spacing
    public float m_linespace_id;      // Indonesian line spacing
    public float m_linespace_default = 0.5f;

    private void Start()
    {
        // Try to find components on this GameObject first, then in children.
        uiText = GetComponent<Text>() ?? GetComponentInChildren<Text>();
        tmpText = GetComponent<TextMeshProUGUI>() ?? GetComponentInChildren<TextMeshProUGUI>();

        Set();
        Language.GetInstance().AddEvent(new Action(TransformLanguage));
    }

    private void OnDestroy()
    {
        Language.GetInstance().RemoveEvent(new Action(TransformLanguage));
    }

    public void SetText(string id)
    {
        this.m_id = id;
        this.Set();
    }

    private void Set()
    {
        string finalText = Language.GetText(this.m_id) + this.m_suffix;

        // Get current language
        SystemLanguage cur = Language.GetInstance().Id;
        bool isBrazilianPortuguese = Language.GetInstance().IsBrazilianPortuguese();

        if (tmpText != null)
        {
            tmpText.text = finalText;

            // Apply line spacing based on language
            ApplyLineSpacing(tmpText, cur, isBrazilianPortuguese);
            return;
        }

        if (uiText != null)
        {
            uiText.text = finalText;
            uiText.font = Language.GetFont();

            // Apply line spacing based on language
            ApplyLineSpacing(uiText, cur, isBrazilianPortuguese);
        }
    }

    private void ApplyLineSpacing(TextMeshProUGUI text, SystemLanguage lang, bool isBrazilianPT)
    {
        if (lang == SystemLanguage.Russian && m_linespace_ru != 0f)
        {
            text.lineSpacing = m_linespace_ru;
        }
        else if (lang == SystemLanguage.English && m_linespace_en != 0f)
        {
            text.lineSpacing = m_linespace_en;
        }
        else if ((lang == SystemLanguage.Portuguese || isBrazilianPT) && m_linespace_pt != 0f)
        {
            text.lineSpacing = m_linespace_pt;
        }
        else if (lang == SystemLanguage.Indonesian && m_linespace_id != 0f)
        {
            text.lineSpacing = m_linespace_id;
        }
        else if (m_linespace_default != 0f)
        {
            text.lineSpacing = m_linespace_default;
        }
    }

    private void ApplyLineSpacing(Text text, SystemLanguage lang, bool isBrazilianPT)
    {
        if (lang == SystemLanguage.Russian && m_linespace_ru != 0f)
        {
            text.lineSpacing = m_linespace_ru;
        }
        else if (lang == SystemLanguage.English && m_linespace_en != 0f)
        {
            text.lineSpacing = m_linespace_en;
        }
        else if ((lang == SystemLanguage.Portuguese || isBrazilianPT) && m_linespace_pt != 0f)
        {
            text.lineSpacing = m_linespace_pt;
        }
        else if (lang == SystemLanguage.Indonesian && m_linespace_id != 0f)
        {
            text.lineSpacing = m_linespace_id;
        }
        else if (m_linespace_default != 0f)
        {
            text.lineSpacing = m_linespace_default;
        }
    }

    private void TransformLanguage()
    {
        this.SetText(this.m_id);
    }
}