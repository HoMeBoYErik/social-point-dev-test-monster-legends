using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace SocialPoint
{
    public class LocalizationService : MonoBehaviour
    {
        // Static singleton reference
        public static LocalizationService Instance { get; private set; }
        // whether or not this object should persist accross scenes (DontDestroyOnLoad)
        public bool makePersistent = true;
        
        public string Language { get; private set; }
        private string k_lang = "k_lang";

        [SerializeField]
        private Dictionary<string, string> strings;

        void Awake()
        {
            // First we check if there are any other instances conflicting
            if (Instance != null && Instance != this)
            {
                // If that is the case, we destroy other instances
                Destroy(gameObject);
            }

            // Here we save our singleton instance
            Instance = this;

            // Furthermore we make sure that we don't destroy between scenes (this is optional)
            if (makePersistent)
            {
                DontDestroyOnLoad(this.gameObject);
            }

            // Others "one time" initializations...

            // Load saved language prefs
            LoadUserLanguage();
        }

        public void LoadDictionary(Dictionary<string, string> dict)
        {
            strings = dict;           
        }

        /// Get a string by key based on current loaded locale
        public string GetString(string key)
        {
            if( strings.ContainsKey(key) )
            {
                return strings[key];
            }

            return null;
        }        
       

        private void LoadUserLanguage()
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
        private bool isValidLanguage(string lang)
        {
            if (lang == "en" || lang == "es" || lang == "fr" || lang == "ru") return true;
            return false;
        }

        #region string_utils_static_methods
        public static string UnescapeString(string escapedString)
        {
            return System.Text.RegularExpressions.Regex.Unescape(escapedString);
        }

        public static string DecodeUnicodeString(string inputString)
        {        
            inputString = System.Text.RegularExpressions.Regex.Unescape(inputString);
            return inputString;           
        }
        #endregion
    }

}

