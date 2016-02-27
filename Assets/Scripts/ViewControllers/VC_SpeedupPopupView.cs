using UnityEngine;
using System.Collections;
using SocialPoint;

public class VC_SpeedupPopupView : MonoBehaviour, IEventListener {

	// Reference to data models
	public GameDataManager gameDataManager;

	public VC_BreedingView breedingView;

	public UIPanel SpeedPopupPanel;

	// Key to find content strings
	public string k_complete_label; // main popup text
	public string k_speedup_label; // speedup button text

	// we store string with parameters here so we can reuse it directly
	private string complete_string;
	private string speedup_string;

	// Popup contents
	public UILabel Complete_Label;
	public UILabel Speedup_Button_Label;

	public bool isOpen;

	public UILabel TotalGemCounter;
	private int currentGemCost;

	// Use this for initialization
	void Start () 
	{
		isOpen = false;
		gameDataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<GameDataManager>();
		EventManager.Instance.AddListener(this as IEventListener, "JSONEvent");
	}

	public void ShowPopupPanel()
	{
		SpeedPopupPanel.alpha = 1.0f;
		isOpen = true;
	}

	public void HidePopupPanel()
	{
		SpeedPopupPanel.alpha = 0.0f;
		isOpen = false;
	}

	public void UpdatePopup(int timeVal, int gemVal)
	{
		this.UpdateCompleteLabel(timeVal, gemVal);
		this.UpdateSpeedupLabel(gemVal);
		currentGemCost = gemVal;
	}


	private void UpdateCompleteLabel(int timeVal, int gemVal)
	{
		string timeToWait = timeVal.ToString() + "s";
		Complete_Label.text = string.Format(complete_string, timeToWait, gemVal);
	}
	
	private void UpdateSpeedupLabel(int value)
	{
		Speedup_Button_Label.text = string.Format(speedup_string, value).ToUpper();
	}

	public void OnSpeedupButtonClick()
	{
		gameDataManager.AvailableGems -= currentGemCost;
		if( gameDataManager.AvailableGems < 0 ){gameDataManager.AvailableGems = 100;}
		TotalGemCounter.text = "x" + gameDataManager.AvailableGems;
		breedingView.StopBreeding();
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
				// cache complete string
				complete_string = WebUtils.DecodeUnicodeString( gameDataManager.GetString(k_complete_label) );
				// cache speedup string
				speedup_string = WebUtils.DecodeUnicodeString( gameDataManager.GetString(k_speedup_label) );				
				break;
			}
		}
		
		return isEventConsumed;
	}
	

}
