using UnityEngine;
using System.Collections;
using SocialPoint;

public class VC_MonsterInfoView : MonoBehaviour {

	public GameDataManager gameDataManager;
	public UserDataManager userDataManager; 


	public UIPanel MonsterInfoPanel;

	public UILabel MonsterInfo;
	public UILabel MonsterName;
	public UILabel MonsterType;
	public UILabel MonsterLevel;

	public UITexture MonsterImage;
	public UITexture Element_0;
	public UITexture Element_1;
	public UITexture Element_2;


	public UILabel BreedingTime;
	public UILabel Love;
	public UILabel Info;

	public void ShowInfoPanel()
	{
		this.MonsterInfoPanel.alpha = 1.0f;
	}

	public void HideInfoPanel()
	{
		this.MonsterInfoPanel.alpha = 0.0f;
	}


	// Use this for initialization
	void Start () 
	{
		switch (userDataManager.Language )
		{
			case "en":
				BreedingTime.text = "BREEDING TIME";
				Love.text = "LOVE";
			Info.text = "Information";
				break;
			case "es":
				BreedingTime.text = "TIEMPO DE REPRODUCCIÓN";
				Love.text = "AMOR";
				Info.text = "Información";
				break;
			case "fr":
				BreedingTime.text = "TEMP D'ÉLEVAGE";
				Love.text = "AMOUR";
				Info.text = "Information";
				break;
			case "ru":
				BreedingTime.text = "ВРЕМЯ РАЗВЕДЕНИЯ";
				Love.text = "ЛЮБОВЬ";
				Info.text = "Информация";
				break;
		}
	}

	public void UpdateMonsterInfo(int monsterId)
	{
		// Get array of informations for left monster
		JSONObject monster = gameDataManager.GameData["monsters"][monsterId];
		MonsterName.text = WebUtils.UnescapeString( monster["name"].str ); // monster name
		MonsterType.text = WebUtils.UnescapeString( monster["type"].str ); // monster type
		MonsterInfo.text = WebUtils.UnescapeString( monster["description"].str ); // monster description
		MonsterLevel.text = WebUtils.UnescapeString( monster["level"].n.ToString() ); // monster level
		StartCoroutine(  gameDataManager.GetTexture( WebUtils.UnescapeString( monster["full_img"].str), LoadMonsterImage));

		Element_0.mainTexture = null;
		Element_1.mainTexture = null;
		Element_2.mainTexture = null;

		// Set element number 0
		string element = WebUtils.UnescapeString( monster["elements"][0].str) ;
		StartCoroutine(  gameDataManager.GetTexture( WebUtils.UnescapeString( gameDataManager.GameData["elements"].GetField(element).GetField("img").str), LoadMonsterElem));
		// Set element number 1 if exists
		if( monster["elements"][1] != null )
		{
			element = monster["elements"][1].str;
			
			StartCoroutine(  gameDataManager.GetTexture( WebUtils.UnescapeString( gameDataManager.GameData["elements"].GetField(element).GetField("img").str), LoadMonsterElem));
		}
		// Set element number 2 if exists
		if( monster["elements"][2] != null )
		{
			element = monster["elements"][2].str;
			StartCoroutine(  gameDataManager.GetTexture( WebUtils.UnescapeString( gameDataManager.GameData["elements"].GetField(element).GetField("img").str), LoadMonsterElem));
		}
	}

	public void LoadMonsterImage(Texture2D tex)
	{
		MonsterImage.mainTexture = tex;
	}

	public void LoadMonsterElem(Texture2D tex)
	{
		if( Element_0.mainTexture == null )
		{
			Element_0.mainTexture = tex;
		}
		else if ( Element_1.mainTexture == null )
		{
			Element_1.mainTexture = tex;
		}
		else if( Element_2.mainTexture == null)
		{
			Element_2.mainTexture = tex;
		}
		
		
	}
	
}
