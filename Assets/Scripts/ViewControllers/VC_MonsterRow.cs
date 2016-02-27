using UnityEngine;
using System.Collections;
using SocialPoint;

public class VC_MonsterRow : MonoBehaviour 
{
	public enum TableSide { LEFT, RIGHT }

	public GameDataManager gameDataManager;

	public VC_MonsterSelectionView monsterView;

	public int id { get; private set;} // id is also index in game data
	public TableSide tableSide;
	public bool isSelected { get; private set;} 
	public bool isEnabled {get; private set;}

	// Link to GUI elements
	public UITexture Thumb;

	public UILabel Name;
	public UILabel Type;
	public UILabel Level;

	public UITexture Element_0;
	public UITexture Element_1;
	public UITexture Element_2;

	public GameObject Selector;
	public GameObject DisabledState;

	public UISprite Box;


	// Font face for Monster Name
	public UIFont MonsterName_FontFace_Normal;
	public UIFont MonsterName_FontFace_Selected;
	// Font color for Monster Name
	public Color MonsterName_FontColor_Normal;
	public Color MonsterName_FontColor_Selected;
	// Font color for Monster Type
	public Color MonsterType_FontColor_Normal;
	public Color MonsterType_FontColor_Selected;

	// Box Background
	public Color Box_Color_Normal;
	public Color Box_Color_Selected;




	public void Init(int id, TableSide tableSide, VC_MonsterSelectionView monsterView)
	{
		// Assign unique id for this column
		this.id = id;
		// Set if it belongs to left list or right list
		this.tableSide = tableSide;
		// Copy reference to owner ViewController
		this.monsterView = monsterView;
		// Cache reference to GameDataManager
		gameDataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<GameDataManager>();
		// Get array of informations for this monster
		JSONObject monster = gameDataManager.GameData["monsters"][id];
		// Update gui info 
		Name.text = WebUtils.UnescapeString( monster["name"].str ); // monster name
		Type.text = WebUtils.UnescapeString( monster["type"].str ); // monster type
		Level.text = monster["level"].n.ToString();	// monster level
		// monster thumb
	 	StartCoroutine(  gameDataManager.GetTexture( WebUtils.UnescapeString( monster["thumb_img"].str), LoadMonsterThumb));
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

		isSelected = false;
		isEnabled = true;

	}

	public void LoadMonsterThumb(Texture2D tex)
	{
			Thumb.mainTexture = tex;
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


	// Use this for initialization
	void Start () 
	{
		gameDataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<GameDataManager>();
	}

	public void Select(float animFactor = 0.0f, bool forward = true)
	{
		isSelected = true;
		Selector.SetActive(true);

		Selector.GetComponent<TweenScale>().tweenFactor = animFactor;

		if( forward )
		{
			Selector.GetComponent<TweenScale>().PlayForward();
		}
		else
		{
			Selector.GetComponent<TweenScale>().PlayReverse();
		}

		// Set selected aspect (change fonts, colors,...)
		SetSelectedAspect();
	}
	
	public void Deselect()
	{
		isSelected = false;
		Selector.SetActive(false);
		// Set normal aspect (change fonts, colors,...)
		SetNormalAspect();

	}

	public void Enable()
	{
		if( !isSelected )
		{
			isEnabled = true;
			SetNormalAspect();
		}

	}

	public void Disable()
	{
		isEnabled = false;
		SetDisabledAspect();
	}

	// Change fonts and colors for NORMAL state
	private void SetNormalAspect()
	{
		DisabledState.SetActive(false);
		Name.bitmapFont = MonsterName_FontFace_Normal;
		Name.color = MonsterName_FontColor_Normal;
		Type.color = MonsterType_FontColor_Normal;
		Box.color = Box_Color_Normal;
	}

	// Change fonts and colors for SELECTED state
	private void SetSelectedAspect()
	{
		Name.bitmapFont = MonsterName_FontFace_Selected;
		Name.color = MonsterName_FontColor_Selected;
		Type.color = MonsterType_FontColor_Selected;
		Box.color = Box_Color_Selected;
	}

	// Change fonts and colors for DISABLED state
	private void SetDisabledAspect()
	{
		DisabledState.SetActive(true);
	}
	
	public void OnBoxClick()
	{

		// if button is disabled, do nothing
		if( !isEnabled ){return;}

		if( !isSelected )
		{
			float animFactor = 0.0f;
			bool forward = true;
			// match the animation of the other selector so they pulse together
			if( this.tableSide == TableSide.LEFT && monsterView.rightMonster != null )
			{
				TweenScale tween = monsterView.rightMonster.Selector.GetComponent<TweenScale>();
				animFactor = tween.tweenFactor;
				if( tween.direction == AnimationOrTween.Direction.Reverse )
				{
					forward = false;
				}

			}
			else if ( this.tableSide == TableSide.RIGHT && monsterView.leftMonster != null )
			{
				TweenScale tween = monsterView.leftMonster.Selector.GetComponent<TweenScale>();
				animFactor = tween.tweenFactor;
				if( tween.direction == AnimationOrTween.Direction.Reverse )
				{
					forward = false;
				}
			}

			// Select the row with parameters
			Select(animFactor, forward);
			// Send event to main view controller
			monsterView.OnRowSelect(this);
		}
		else
		{
			Deselect();
			monsterView.OnRowDeselect(this.id, this.tableSide);
		}

	}

	public void OnThumbClick()
	{
		// if button is disabled, do nothing
		if( !isEnabled ){return;}

		monsterView.monsterInfo.UpdateMonsterInfo(this.id);
		monsterView.monsterInfo.ShowInfoPanel();
	}


}
