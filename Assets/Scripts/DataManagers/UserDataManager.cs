using UnityEngine;
using System.Collections;

public class UserDataManager : MonoBehaviour {

	public string Language {get; private set;}
	private string k_lang = "k_lang";

	void Awake()
	{
		// 2 Load user language
		LoadUserLanguage();
	}


	private void LoadUserLanguage()
	{
		// if user language not yet set
		if( !PlayerPrefs.HasKey(k_lang) )
		{
			// try to determine it from system language
			switch( Application.systemLanguage )
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

	public void SetUserLanguage(string lang)
	{
		bool langHasChanged = false;
		if( isValidLanguage(lang) )
		{
			if( lang != Language)
			{
				langHasChanged = true;
				Language = lang;
				PlayerPrefs.SetString(k_lang, Language);
				PlayerPrefs.Save();			
			}
		}

		if ( langHasChanged )
		{
			// TODO notify that lang has changed
			// TODO maybe rename JSONEvent to generic event with payload
			//EventManager.Instance.QueueEvent(new JSONEvent("OnLanguageChange", null));

			// Reload app with new language
			Application.LoadLevel(Application.loadedLevel);
		}

	}

	private bool isValidLanguage(string lang)
	{
		if( lang == "en" || lang == "es" || lang == "fr" || lang == "ru") return true;
		return false;
	}
	
}
