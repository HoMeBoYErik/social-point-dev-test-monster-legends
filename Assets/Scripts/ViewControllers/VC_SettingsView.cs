using UnityEngine;
using System.Collections;
using Deprecated;

public class VC_SettingsView : MonoBehaviour,  IEventListener 
{

	public UIPanel LanguageChangePanel;
	public UIPanel MenuPanel;


	void Start()
	{
		EventManager.Instance.AddListener(this as IEventListener, "JSONEvent");
	}


	public void OnSettingsButtonClick()
	{
		LanguageChangePanel.gameObject.SetActive(true);
	}

	public void OnCloseLanguageButtonClick()
	{
		LanguageChangePanel.gameObject.SetActive(false);
	}

	bool IEventListener.HandleEvent(IEvent evt)
	{
		bool isEventConsumed = false;
		
		// Check if the type of Event is correct and it's what we are looking for
		// It's useful when we register for more than one type of event.
		if( evt.GetName() == "JSONEvent")
		{
			switch ( evt.GetCallName() )
			{
			case "LoadGameDataComplete":
				this.MenuPanel.alpha = 1.0f;
				break;
			}
		}
		
		return isEventConsumed;
	}
}
