using Assets.Scripts.Configs;
using System;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    internal class Language
    {
        private SystemLanguage m_id = SystemLanguage.English;
        private Font m_fonts;
        private static Language m_instance;
        private Action m_transformHandle;

        public SystemLanguage Id
        {
            get { return this.m_id; }
            set { this.m_id = value; }
        }

        public Language()
        {
            this.Init();
        }

        public static Language GetInstance()
        {
            if (Language.m_instance == null)
            {
                Language.m_instance = new Language();
            }
            return Language.m_instance;
        }

        /// <summary>
        /// Check if Brazilian Portuguese is currently selected
        /// </summary>
        public bool IsBrazilianPortuguese()
        {
            int savedLang = PlayerPrefs.GetInt("LocalData_LanguageId", -1);
            return savedLang == 1000;
        }

        public static string GetText(string id)
        {
            if (!Configs.Configs.TLanguages.ContainsKey(id))
            {
                return "ERROR NO " + id;
            }

            TLanguage tLanguage = Configs.Configs.TLanguages[id];
            SystemLanguage currentLang = Language.GetInstance().Id;

            // Map languages to their text fields
            switch (currentLang)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    return tLanguage.L_ZH_CN;

                case SystemLanguage.Indonesian:
                    return !string.IsNullOrEmpty(tLanguage.L_ID) ? tLanguage.L_ID : tLanguage.L_EN;

                case SystemLanguage.Portuguese:
                    // Check if user specifically selected Brazilian Portuguese
                    if (Language.GetInstance().IsBrazilianPortuguese())
                    {
                        return !string.IsNullOrEmpty(tLanguage.L_PT_BR) ? tLanguage.L_PT_BR : tLanguage.L_PT;
                    }
                    return !string.IsNullOrEmpty(tLanguage.L_PT) ? tLanguage.L_PT : tLanguage.L_EN;

                case SystemLanguage.Japanese:
                    return !string.IsNullOrEmpty(tLanguage.L_JP) ? tLanguage.L_JP : tLanguage.L_EN;

                case SystemLanguage.Russian:
                    return !string.IsNullOrEmpty(tLanguage.L_RU) ? tLanguage.L_RU : tLanguage.L_EN;

                case SystemLanguage.Hindi:
                    return !string.IsNullOrEmpty(tLanguage.L_HI) ? tLanguage.L_HI : tLanguage.L_EN;

                default:
                    return tLanguage.L_EN;
            }
        }

        public static Font GetFont()
        {
            return Language.m_instance.m_fonts;
        }

        public void Set(SystemLanguage id)
        {
            this.Id = id;
            this.m_fonts = this.LoadFont();
            PlayerPrefs.SetInt("LocalData_LanguageId", (int)this.Id);

            Action expr_29 = this.m_transformHandle;
            if (expr_29 != null)
            {
                expr_29();
            }
        }

        /// <summary>
        /// Set Brazilian Portuguese specifically (uses custom code 1000)
        /// </summary>
        public void SetBrazilianPortuguese()
        {
            this.Id = SystemLanguage.Portuguese;
            this.m_fonts = this.LoadFont();
            PlayerPrefs.SetInt("LocalData_LanguageId", 1000); // Custom code for PT-BR

            Action expr_29 = this.m_transformHandle;
            if (expr_29 != null)
            {
                expr_29();
            }
        }

        public void AddEvent(Action e)
        {
            this.m_transformHandle += e;
        }

        public void RemoveEvent(Action e)
        {
            this.m_transformHandle -= e;
        }

        private void Init()
        {
            int savedLang = PlayerPrefs.GetInt("LocalData_LanguageId", (int)Application.systemLanguage);

            if (savedLang == 1000) // Brazilian Portuguese
            {
                this.Id = SystemLanguage.Portuguese;
            }
            else
            {
                this.Id = (SystemLanguage)savedLang;
            }

            // Validate and set default if needed
            switch (this.Id)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                case SystemLanguage.Japanese:
                case SystemLanguage.Russian:
                case SystemLanguage.Indonesian:
                case SystemLanguage.Portuguese:
                case SystemLanguage.Hindi:
                case SystemLanguage.English:
                    // Valid languages
                    break;
                default:
                    this.Id = SystemLanguage.English;
                    break;
            }

            this.m_fonts = this.LoadFont();
        }

        private Font LoadFont()
        {
            SystemLanguage id = this.Id;
            string path;

            if (id == SystemLanguage.Russian)
            {
                path = "font/font_ru";
            }
            else if (id == SystemLanguage.Japanese || id == SystemLanguage.Chinese ||
                     id == SystemLanguage.ChineseSimplified || id == SystemLanguage.ChineseTraditional)
            {
                path = "font/font_asian";
            }
            else if (id == SystemLanguage.Hindi)
            {
                path = "font/font_hindi";
            }
            else // Indonesian, Portuguese, English, etc.
            {
                path = "font/font_common";
            }

            Font font = Resources.Load(path) as Font;
            if (font == null)
            {
                Debug.LogWarning($"Font not found at {path}, using default");
                font = Resources.Load("font/font_common") as Font;
            }
            return font;
        }
    }
}