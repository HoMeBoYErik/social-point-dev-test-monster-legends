using UnityEngine;
using System.Collections;
using SocialPoint;
using Deprecated;

public class VC_BreedingView : MonoBehaviour, IEventListener  {

	// Reference to data models
	public GameDataManager gameDataManager;

	public UIPanel MonsterBreedingPanel;

	public VC_MonsterSelectionView monsterView;
	public VC_SpeedupPopupView speedPopupView;
	public VC_MonsterInfoView monsterInfoView;

	public string k_waiting_label;
	public string k_speedup_label;

	public UILabel Waiting_Label;
	public UILabel Speedup_Button_Label;

	// Breeding mechanics parameters
	public int MinBreedingTime = 10;
	public int MaxBreedingTime = 30;
	public int GemTimeFactor = 2;

	// we store string with parameters here so we can reuse it directly
	private string waiting_string;
	private string speedup_string;

	private int leftMonsterId;
	private int rightMonsterId;


	// Left monster params
	public UITexture LeftMonsterImage;
	public UILabel LeftMonsterLabel;

	// Right monster params
	public UITexture RightMonsterImage;
	public UILabel RightMonsterLabel;

	public UIPlaySound BreedingCompleteSound;


	// Use this for initialization
	void Start () 
	{
		gameDataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<GameDataManager>();
		EventManager.Instance.AddListener(this as IEventListener, "JSONEvent");
	}


	// Entry animation for the GUI panel
	public void ShowBreedingView()
	{
		TweenAlpha.Begin(MonsterBreedingPanel.gameObject, 0.2f, 1.0f).delay = 0.2f;
	}

	// Entry animation for the GUI panel
	public void HideBreedingView()
	{
		TweenAlpha.Begin(MonsterBreedingPanel.gameObject, 0.2f, 0.0f).delay = 0.0f;
	}

	public void InitBreeding(int leftMonsterId, int rightMonterId)
	{
		UpdateBreedingView(leftMonsterId, rightMonterId);
		int breedingTime = Random.Range(MinBreedingTime, MaxBreedingTime + 1);
		StartCoroutine("Breeding", breedingTime);
		ShowBreedingView();
	}

	private void UpdateBreedingView(int leftMonsterId, int rightMonsterId)
	{
		// Get array of informations for left monster
		JSONObject leftMonster = gameDataManager.GameData["monsters"][leftMonsterId];
		// Get array of informations for right monster
		JSONObject rightMonster = gameDataManager.GameData["monsters"][rightMonsterId];

		// Left monster name
		LeftMonsterLabel.text = WebUtils.UnescapeString( leftMonster["name"].str ); // monster name
		// Left monster full image
		StartCoroutine(  gameDataManager.GetTexture( WebUtils.UnescapeString( leftMonster["full_img"].str), LoadLeftMonsterImage));

		// Right monster name
		RightMonsterLabel.text = WebUtils.UnescapeString( rightMonster["name"].str ); // monster name
		// Right monster full image
		StartCoroutine(  gameDataManager.GetTexture( WebUtils.UnescapeString( rightMonster["full_img"].str), LoadRightMonsterImage));

		this.leftMonsterId = leftMonsterId;
		this.rightMonsterId = rightMonsterId;
	}

	private void LoadLeftMonsterImage(Texture2D tex)
	{
		LeftMonsterImage.mainTexture = tex;
	}

	private void LoadRightMonsterImage(Texture2D tex)
	{
		RightMonsterImage.mainTexture = tex;
	}

	public void StopBreeding()
	{
		StopCoroutine("Breeding");
		this.OnBreedingEnded();
	}

	private void UpdateWaitingLabel(int value)
	{
		string timeToWait = "00:" + string.Format("{0:00}", value);
		Waiting_Label.text = string.Format(waiting_string, timeToWait).ToUpper();
	}

	private void UpdateSpeedupLabel(int value)
	{
		Speedup_Button_Label.text = string.Format(speedup_string, value).ToUpper();
	}

	public void OnSpeedupButtonClick()
	{
		speedPopupView.ShowPopupPanel();
	}

	public void OnLeftMonsterClick()
	{
		monsterInfoView.UpdateMonsterInfo(leftMonsterId);
		monsterInfoView.ShowInfoPanel();
	}
	public void OnRightMonsterClick()
	{
		monsterInfoView.UpdateMonsterInfo(rightMonsterId);
		monsterInfoView.ShowInfoPanel();
	}


	IEnumerator Breeding(float breedTime)
	{
		//float startTime = Time.time;
		float timer = breedTime;

		while(timer > 0)
		{
			UpdateWaitingLabel((int) timer);
			UpdateSpeedupLabel((int) timer * GemTimeFactor);
			speedPopupView.UpdatePopup((int) timer, (int) timer * GemTimeFactor);
			// countdown timer
			timer -= Time.deltaTime;

			yield return null;
		}

		this.OnBreedingEnded();
	}

	public void OnBreedingEnded()
	{
		this.HideBreedingView();
		BreedingCompleteSound.Play();
		speedPopupView.HidePopupPanel();
		monsterView.ResetAllSelection();
		monsterView.ShowMonsterSelectionView();
	}

	#region EventListener
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
				// cache waiting string
				waiting_string = WebUtils.DecodeUnicodeString( gameDataManager.GetString(k_waiting_label) );
				// cache speedup string
				speedup_string = WebUtils.DecodeUnicodeString( gameDataManager.GetString(k_speedup_label) );

				string timeToWait = "00:" + string.Format("{0:00}", 30);
				Waiting_Label.text = string.Format(waiting_string, timeToWait).ToUpper();

				Speedup_Button_Label.text = string.Format(speedup_string, 15).ToUpper();

				//String s = String.Format("The current price is {0} per ounce.", pricePerOunce);

				break;
			}
		}
		
		return isEventConsumed;
	}
	#endregion EventListener
}
