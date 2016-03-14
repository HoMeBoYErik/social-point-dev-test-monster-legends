using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace SocialPoint
{
    public class LocalizationService : MonoBehaviour, ILocalizationService
    {
        public string Language { get; set; }
        private string k_lang = "k_lang";

        [SerializeField]
        private Dictionary<string, StringReactiveProperty> strings;


        public LocalizationService()
        {           
        }
        
        public void LoadDictionary(Dictionary<string, string> dict)
        {
            strings = new Dictionary<string, StringReactiveProperty>();

            foreach( var elem in dict )
            {
                strings.Add(elem.Key, new StringReactiveProperty(elem.Value));
            }
        }

        /// Get a string by key based on current loaded locale
        /// it's reactive if we want to subscribe to its value
        public StringReactiveProperty GetString(string key)
        {
            if( strings.ContainsKey(key) )
            {
                return strings[key];
            }

            return null;
        }

        public string LoadUserLanguage()
        {
            // if user language not yet set
            if (!PlayerPrefs.HasKey(k_lang))
            {
                // try to determine it from system language
                switch (Application.systemLanguage)
                {
                    // English
                    case SystemLanguage.English:
                        Language = "en";
                        PlayerPrefs.SetString(k_lang, Language);
                        break;
                    // Spanish
                    case SystemLanguage.Spanish:
                        Language = "es";
                        PlayerPrefs.SetString(k_lang, Language);
                        break;
                    // French
                    case SystemLanguage.French:
                        Language = "fr";
                        PlayerPrefs.SetString(k_lang, Language);
                        break;
                    // Russian
                    case SystemLanguage.Russian:
                        Language = "ru";
                        PlayerPrefs.SetString(k_lang, Language);
                        break;
                    //...fall back to English if no match
                    default:
                        Language = "en";
                        PlayerPrefs.SetString(k_lang, Language);
                        break;
                }
                PlayerPrefs.Save();
            }
            //...already set, load it from prefs
            else
            {
                Language = PlayerPrefs.GetString(k_lang);
            }

            return Language;
        }

        // Set the language for the app
        // Reload current scene if language change
        public void SetUserLanguage(string lang)
        {
            bool langHasChanged = false;
            if (isValidLanguage(lang))
            {
                if (lang != Language)
                {
                    langHasChanged = true;
                    Language = lang;
                    PlayerPrefs.SetString(k_lang, Language);
                    PlayerPrefs.Save();
                }
            }

            if (langHasChanged)
            {
                // Notify scene controller

                // Reload app with new language
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel(Application.loadedLevel);
#else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#endif
            }

        }

        // Validate that user is trying to switch to a supported language
        public bool isValidLanguage(string lang)
        {
            if (lang == "en" || lang == "es" || lang == "fr" || lang == "ru") return true;
            return false;
        }

        #region string_utils_static_methods
        public string UnescapeString(string escapedString)
        {
            return System.Text.RegularExpressions.Regex.Unescape(escapedString);
        }

        public string DecodeUnicodeString(string inputString)
        {        
            inputString = System.Text.RegularExpressions.Regex.Unescape(inputString);
            return inputString;           
        }
        #endregion
    }

}

