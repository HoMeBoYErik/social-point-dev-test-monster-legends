using UnityEngine;
using System.Collections;
using System.Text;
using System;
using SocialPoint;

public class VC_LoadView : MonoBehaviour, IEventListener {

	public UserDataManager userDataManager;
	public GameDataManager gameDataManager;
	
	// GUI ELEMENTS - LOADING PANEL
	public UIPanel loadingPanel;
	public UILabel loadingLabel;
	public UISprite progressBar;


	void Start()
	{
		EventManager.Instance.AddListener(this as IEventListener, "JSONEvent");
		EventManager.Instance.AddListener(this as IEventListener, "ErrorEvent");

		// Show Loading screen in user language
		ShowLoadingScreen( userDataManager.Language );

		// 3 (Load JSON) + 4 (Load Images)
		//gameDataManager.Load( userDataManager.Language );

		// 5 show feedback of download progress
		//StartCoroutine("UpdateDownloadProgressBar");
	}

	IEnumerator UpdateDownloadProgressBar()
	{
		while( progressBar.fillAmount < 1.0f )
		{
			progressBar.fillAmount = gameDataManager.TotalDownloadProgress;
			yield return null;
		}
		// TODO improve fluidity of download bar
		// TODO remove subscription service maybe
	}

	public void ShowLoadingScreen( string lang )
	{
		loadingPanel.gameObject.SetActive(true);

		//TweenAlpha.Begin(loadingPanel.gameObject, 0.5f, 1.0f).delay = 2.3f;

		if( lang == "en" )
		{
			loadingLabel.text = "LOADING";
		}
		else if ( lang == "es" )
		{
			loadingLabel.text = "CARGANDO";
		}
		else if (lang == "fr" )
		{
			loadingLabel.text =  "CHARGEMENT";
		}
		else if ( lang == "ru" )
		{
			loadingLabel.text =  "ЗАГРУЗКА";
		}

		loadingLabel.gameObject.SetActive(true);
	}

	public void HideLoadingPanel()
	{
		TweenAlpha.Begin(loadingPanel.gameObject, 0.5f, 0.0f).delay = 0.0f;
	}

	public void ShowInternetConnectionError()
	{
		string lang = userDataManager.Language;

		if( lang == "en" )
		{
			loadingLabel.text = "NETWORK ERROR";
		}
		else if ( lang == "es" )
		{
			loadingLabel.text = "ERROR DE RED";
		}
		else if (lang == "fr" )
		{
			loadingLabel.text =  "ERREUR DE CONNEXION";
		}
		else if ( lang == "ru" )
		{
			loadingLabel.text =  "ОШИБКА СЕТИ";
		}

		StartCoroutine("ReloadGameInFiveSeconds");


	}

	IEnumerator ReloadGameInFiveSeconds()
	{
		yield return new WaitForSeconds(5.0f);

		Application.LoadLevel(Application.loadedLevel);
	}

	
	bool IEventListener.HandleEvent(IEvent evt)
	{
		bool isEventConsumed = false;

		if( evt.GetName() == "ErrorEvent" )
		{
			ErrorObject errorObj = evt.GetData() as ErrorObject;
			
			switch ( errorObj.errorCode )
			{
				case ERROR_CODES.NO_INTERNET_CONNECTION:
					ShowInternetConnectionError();				
					break;
				
				case ERROR_CODES.INTERNET_VERIFIED:		
					// 3 (Load JSON) + 4 (Load Images)
					gameDataManager.Load( userDataManager.Language );					
					// 5 show feedback of download progress
					StartCoroutine("UpdateDownloadProgressBar");				
					break;
				
			}
		}

		// Check if the type of Event is correct and it's what we are looking for
		// It's useful when we register for more than one type of event.
		else if( evt.GetName() == "JSONEvent")
		{
			//JSONObject gameData = evt.GetData() as JSONObject;
			
			// Look for the Event Name and react properly. Use
			switch ( evt.GetCallName() )
			{
				case "LoadGameDataComplete":
					this.HideLoadingPanel();					
					break;

				case "OnLanguageChange":
					ShowLoadingScreen(userDataManager.Language);
					gameDataManager.Load( userDataManager.Language );
					break;

			}
		}

		return isEventConsumed;
	}

	public void ChangeLang_EN()
	{
		userDataManager.SetUserLanguage("en");
	}
	public void ChangeLang_ES()
	{
		userDataManager.SetUserLanguage("es");
	}
	public void ChangeLang_FR()
	{
		userDataManager.SetUserLanguage("fr");
	}
	public void ChangeLang_RU()
	{
		userDataManager.SetUserLanguage("ru");
	}



}
