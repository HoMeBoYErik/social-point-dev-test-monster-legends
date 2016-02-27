using UnityEngine;
using System.Collections;
using SocialPoint;

public class VC_MonsterSelectionView : MonoBehaviour, IEventListener {

	// Reference to data models
	public UserDataManager userDataManager;
	public GameDataManager gameDataManager;

	// Reference to others View Controllers
	public VC_BreedingView breedingView;
	public VC_MonsterInfoView monsterInfo;

	// Main Panel
	public GameObject MonsterSelectionPanel;

	// Table row prefab
	public GameObject MonsterRowPrefab;
	public GameObject MonsterRowPrefab_iPad; // if iPad use a tighter graphic element
	public GameObject MonsterRowPrefab_wide; // if very wide screen like iPhone 5 o iPhone 6
	// Left scroll list 
	public UIScrollView LeftScrollView;
	public UIGrid LeftGrid;
	public GameObject LeftScrollRoot;
	// Right scroll list
	public UIScrollView RightScrollView;
	public UIGrid RightGrid;
	public GameObject RightSCrollRoot;

	// Reference tu current selected monsters on both sides
	[HideInInspector]
	public VC_MonsterRow leftMonster = null;
	[HideInInspector]
	public VC_MonsterRow rightMonster = null;

	// Breeding button
	public UIButton BreedingButton;
	public string k_breeding_button;
	public string k_breeding_button_selected;
	public GameObject HearthButtonDecoration;



	
	
	// Use this for initialization
	void Start () 
	{
		gameDataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<GameDataManager>();
		EventManager.Instance.AddListener(this as IEventListener, "JSONEvent");
	}
	// Entry animation for the GUI panel
	public void ShowMonsterSelectionView()
	{
		TweenAlpha.Begin(MonsterSelectionPanel.gameObject, 0.5f, 1.0f).delay = 0.2f;
	}
	public void HideMonsterSelectionView()
	{
		TweenAlpha.Begin(MonsterSelectionPanel.gameObject, 0.2f, 0.0f).delay = 0.0f;
	}


	private VC_MonsterRow GetRow(int id, VC_MonsterRow.TableSide side)
	{
		if ( side == VC_MonsterRow.TableSide.LEFT )
		{
			return LeftGrid.transform.GetChild(id).GetComponent<VC_MonsterRow>();
		}
		else if ( side == VC_MonsterRow.TableSide.RIGHT )
		{
			return RightGrid.transform.GetChild(id).GetComponent<VC_MonsterRow>();
		}

		return null;
	}

	#region EVENTS
	// ON SELECT EVENT
	public void OnRowSelect(VC_MonsterRow selection)
	{
		// if selection is on the left side
		if( selection.tableSide == VC_MonsterRow.TableSide.LEFT )
		{
			if( leftMonster != null && leftMonster != selection )
			{
				// Reactivate corresponding right monster (same id on right side)
				GetRow (leftMonster.id, VC_MonsterRow.TableSide.RIGHT).Enable();
				leftMonster.Deselect();
			}
			// save current reference to selection
			leftMonster = selection;
			// Deactivate corresponding monster on right side
			GetRow (leftMonster.id, VC_MonsterRow.TableSide.RIGHT).Disable();
		}
		// ...else if selection is on the right side
		else if ( selection.tableSide == VC_MonsterRow.TableSide.RIGHT )
		{
			if( rightMonster != null && rightMonster != selection )
			{
				// Reactivate corresponding left monster (same id on left side)
				GetRow (rightMonster.id, VC_MonsterRow.TableSide.LEFT).Enable();
				rightMonster.Deselect();
			}
			// save current reference to selection
			rightMonster = selection;
			// Deactivate corresponding monster on left side
			GetRow (rightMonster.id, VC_MonsterRow.TableSide.LEFT).Disable();
		}

		// check if enable/disable breeding button
		this.RefreshBreedingButton();
	}

	// Receive an id and a table side instead of a direct reference
	public void OnRowSelect(int id, VC_MonsterRow.TableSide side)
	{
		// Obtain a reference to the new selected monster
		VC_MonsterRow selection = GetRow (id, side);
		this.OnRowSelect(selection);
	}

	// ON DESELECT EVENT	
	public void OnRowDeselect(VC_MonsterRow deselection)
	{
		// if deselection was on left side
		if( deselection.tableSide == VC_MonsterRow.TableSide.LEFT )
		{
			if( leftMonster != null && leftMonster == deselection )
			{
				// reactivate corresponding monster to the other side
				GetRow (leftMonster.id, VC_MonsterRow.TableSide.RIGHT).Enable();
				leftMonster = null;
			}
		}
		// ... else if deselection was on right side
		else if( deselection.tableSide == VC_MonsterRow.TableSide.RIGHT )
		{
			if( rightMonster != null && rightMonster == deselection )
			{
				// reactivate corresponding monster to the other side
				GetRow (rightMonster.id, VC_MonsterRow.TableSide.LEFT).Enable();
				rightMonster = null;
			}
		}
		// check if enable/disable breeding button
		this.RefreshBreedingButton();

	}		

	public void OnRowDeselect(int id, VC_MonsterRow.TableSide side)
	{
		// Obtain a reference to the deselected monster
		VC_MonsterRow deselection = GetRow (id, side);
		this.OnRowDeselect(deselection);
	}

	public void ResetAllSelection()
	{
		leftMonster.Deselect();
		rightMonster.Deselect();

		OnRowDeselect(leftMonster);
		OnRowDeselect(rightMonster);

		leftMonster = null;
		rightMonster = null;

		LeftScrollView.ResetPosition();
		RightScrollView.ResetPosition();
	}




	#endregion EVENTS

	#region BREEDING BUTTON

	public void OnStartBreeding()
	{
		breedingView.InitBreeding(leftMonster.id, rightMonster.id);
		this.HideMonsterSelectionView();
	}


	public void RefreshBreedingButton()
	{
		// if we have a valid selection on both sides
		// enable breeding button
		if( leftMonster != null && rightMonster != null )
		{
			this.ActivateBreedingButton();
		}
		else
		{
			this.DeactivateBreedingButton();
		}
	}

	public void ActivateBreedingButton()
	{
		BreedingButton.isEnabled = true;
		BreedingButton.GetComponentInChildren<UILabel>().text = WebUtils.DecodeUnicodeString( gameDataManager.GetString(k_breeding_button_selected) ).ToUpper(); 
		HearthButtonDecoration.SetActive(true);
	}
	public void DeactivateBreedingButton()
	{
		BreedingButton.isEnabled = false;
		BreedingButton.GetComponentInChildren<UILabel>().text = WebUtils.DecodeUnicodeString( gameDataManager.GetString(k_breeding_button) ).ToUpper(); 
		HearthButtonDecoration.SetActive(false);
	}
	#endregion BREEDING BUTTON



	//public void 

	bool IEventListener.HandleEvent(IEvent evt)
	{
		bool isEventConsumed = false;
		
		// Check if the type of Event is correct and it's what we are looking for
		// It's useful when we register for more than one type of event.
		if( evt.GetName() == "JSONEvent")
		{
			//JSONObject gameData = evt.GetData() as JSONObject;
			
			// Look for the Event Name and react properly. Use
			switch ( evt.GetCallName() )
			{
			case "LoadGameDataComplete":

				// Determine screen aspect ratio to select row element prefab
				float aspectRatio =  (float)Screen.width / (float)Screen.height;

				// Reference to the prefab that will populate the tables
				GameObject prefab = MonsterRowPrefab;
				// 4:3 or squared ( like iPad wide )
				if( aspectRatio <= 1.34f )
				{
					prefab = MonsterRowPrefab_iPad;
				}
				// iPhone 4 or normal wide
				else if ( aspectRatio <= 1.5f )
				{
					prefab = MonsterRowPrefab;
				}
				else
				{
					prefab = MonsterRowPrefab_wide;
				}

				// Fill monster lists
				GameObject row;
				for( int i = 0; i < gameDataManager.GameData["monsters"].Count; ++i)
				{
					// Populate left list
					row = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
					row.name = i.ToString() + "_MonsterRow";
					row.transform.parent = LeftScrollRoot.transform;
					row.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					row.transform.position = Vector3.zero;
					row.GetComponent<VC_MonsterRow>().Init(i, VC_MonsterRow.TableSide.LEFT, this);
					LeftGrid.Reposition();

					// Populate right list
					row = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
					row.name = i.ToString() + "_MonsterRow";
					row.transform.parent = RightSCrollRoot.transform;
					row.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					row.transform.position = Vector3.zero;
					row.GetComponent<VC_MonsterRow>().Init(i,VC_MonsterRow.TableSide.RIGHT, this);
					RightGrid.Reposition();
				}

				ShowMonsterSelectionView();
				break;
			}
		}
		
		return isEventConsumed;
	}
}
