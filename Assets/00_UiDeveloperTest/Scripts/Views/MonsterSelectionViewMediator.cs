using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using SocialPoint;
using SocialPoint.Signals;
using UniRx;


public class MonsterSelectionViewMediator : Mediator {

    // Injecting the view
    [Inject]
    public MonsterSelectionView view { get; set; }

    // Inject the localization service to translate the view
    [Inject]
    public ILocalizationService localizationService { get; set; }

    // Inject the gameobject prefab factory for monster row
    [Inject]
    public MonsterRowFactory monsterRowFactory { get; set; }

    // Signals we want to listen to
    [Inject]
    public LoadCompleteSignal loadCompleteSignal { get; set; }
    // Signal received when a breeding operation ended
    [Inject]
    public BreedingEndedReceivedSignal breedingEndedReceivedSignal { get; set; }

    // Signals we want to fire
    [Inject]
    public StartBreedingSignal startBreedingSignal { get; set; }
    [Inject]
    public OpenLanguagePanelSignal openLanguagePanelSignal { get; set; }

    private StringReactiveProperty breedingButtonText;
    private string breedingButtonNormalText;
    private string breedingButtonSelectedText;

    private List<MonsterDataModel> monsters;
    private Dictionary<string, ElementDataModel> elements;
    private Dictionary<string, Texture2D> images;

    public MonsterRowView leftSelectedMonster = null;
    public MonsterRowView rightSelectedMonster = null;

    private bool isBreedingEnabled = false;

    public override void OnRegister()
    {
        base.OnRegister();

        // Init view
        view.init();
        // Map string dictionary to label text
        loadCompleteSignal.AddListener(OnLoadComplete);
        breedingEndedReceivedSignal.AddListener(OnBreedingComplete);
        view.OnOpenLanguagePanelClick += OnOpenLanguagePanelClick;
        
    }

    public override void OnRemove()
    {
        base.OnRemove();
        loadCompleteSignal.RemoveListener(OnLoadComplete);
        breedingEndedReceivedSignal.RemoveListener(OnBreedingComplete);
        view.OnStartBreedingClick -= this.OnStartBreedingClick;
    }

    private void OnLoadComplete( List<MonsterDataModel> monsters,
                                 Dictionary<string, ElementDataModel> elements,
                                 Dictionary<string, Texture2D> images )
    {
        localizationService.GetString("title").Subscribe(x => view.header_title_text.text = x.ToUpper());

        // Init breeding button text values
        breedingButtonNormalText = localizationService.GetString("select_button").Value;
        breedingButtonSelectedText = localizationService.GetString("select_button_selected").Value;
        breedingButtonText = new StringReactiveProperty(breedingButtonNormalText);       
        breedingButtonText.Subscribe(x => view.breeding_button_text.text = x.ToUpper());
        view.OnStartBreedingClick += this.OnStartBreedingClick;
        this.DisableBreeding();

        this.monsters = monsters;
        this.elements = elements;
        this.images = images;
    
        // Load Monster Lists inside scrollers views
        for (int i = 0; i < monsters.Count; ++i )
        {
            // Fill RIGHT LIST
            GameObject goR = monsterRowFactory.Create();
            goR.name = "monster_right_0" + i.ToString();
            goR.transform.SetParent(view.right_list_root, false);
            // Init monster view with info from model
            MonsterDataModel m = monsters[i];
            MonsterRowView mView = goR.transform.GetComponent<MonsterRowView>();
            mView.init(i, MonsterRowView.TableSide.RIGHT); //assign to the view an id number
            
            // Assing click delegates
            mView.OnMonsterClick += OnMonsterClick_NotSelected;            
            mView.monster_thumb_image.texture = (Texture)images[m.thumb_img];
            mView.monster_level_text.text = m.level.ToString();
            mView.monster_name_text.text = m.name;
            mView.monster_type_text.text = m.type;

            int counter = 0;
            foreach( string elemName in m.elements)
            {
                mView.monster_elements[counter].enabled = true;
                mView.monster_elements[counter].texture = images[elements[elemName].img];
                ++counter;
            }

            // Fill LEFT LIST
            // Load Monster Lists inside scrollers     
            GameObject goL = monsterRowFactory.Create();
            goL.name = "monster_left_0" + i.ToString();
            goL.transform.SetParent(view.left_list_root, false);
            mView = goL.transform.GetComponent<MonsterRowView>();
            mView.init(i, MonsterRowView.TableSide.LEFT); //assign to the view an id number
            // Assing click delegates
            mView.OnMonsterClick += OnMonsterClick_NotSelected;            
            mView.monster_thumb_image.texture = (Texture)images[m.thumb_img];
            mView.monster_level_text.text = m.level.ToString();
            mView.monster_name_text.text = m.name;
            mView.monster_type_text.text = m.type;

            counter = 0;
            foreach( string elemName in m.elements)
            {
                mView.monster_elements[counter].enabled = true;
                mView.monster_elements[counter].texture = images[elements[elemName].img];
                ++counter;
            }           
            
        }

        view.ShowView();
    }

    private void OnBreedingComplete()
    {
        this.ResetAllSelections();
        view.ShowView();
    }

    public void ResetAllSelections()
    {
        OnMonsterClick_Selected(leftSelectedMonster.id, leftSelectedMonster.tableSide);
        OnMonsterClick_Selected(rightSelectedMonster.id, rightSelectedMonster.tableSide);
        leftSelectedMonster = null;
        rightSelectedMonster = null;
        view.right_list_root.localPosition = Vector3.zero;
        view.left_list_root.localPosition = Vector3.zero;
        RefreshBreedingButton();
    }

    IEnumerator EnableBreeding()
    {
        // Start hearth animation
        view.hearth_decoration_tweener.Stop();
        view.hearth_decoration_button_tweener.Stop();
        view.hearth_decoration_tweener.Play("FallDownFading");
        view.hearth_decoration_button_tweener.Play("Appear");

        // delay the button activation a bit
        yield return new WaitForSeconds(0.7f);

        view.breeding_button.interactable = true;
        breedingButtonText.Value = breedingButtonSelectedText;
        
        isBreedingEnabled = true;
        
    }
    IEnumerator DisableBreeding()
    {
        // if it was enabled play the appearing animation
        if( isBreedingEnabled )
        {
            view.hearth_decoration_tweener.Stop();
            view.hearth_decoration_tweener.Play("Appearing");
            view.hearth_decoration_tweener.Play("HeartBit");
            view.hearth_decoration_button_tweener.Stop();
            view.hearth_decoration_button_tweener.Play("Disappear");
        }        

        view.breeding_button.interactable = false;
        breedingButtonText.Value = breedingButtonNormalText;

        isBreedingEnabled = false;

        yield return null;
    }

    public void RefreshBreedingButton()
    {
        if( leftSelectedMonster != null && rightSelectedMonster != null )
        {
           StartCoroutine( "EnableBreeding" );            
        }
        else
        {
           StartCoroutine( "DisableBreeding" );
        }
    }

    public void OnStartBreedingClick()
    {
        BreedingCoupleIdModel breedingCouple = new BreedingCoupleIdModel(leftSelectedMonster.id, rightSelectedMonster.id);
        startBreedingSignal.Dispatch(breedingCouple);
        view.HideView();
    }

    public void OnMonsterClick_PlaySound(AudioSource sound)
    {
        sound.Play();
    }

    public void OnMonsterClick_NotSelected(int id, MonsterRowView.TableSide tableSide)
    {
        MonsterRowView mv = null;
        MonsterRowView mvMirror = null; // corresponding monster on the other side
        //Debug.Log("Mediator monster button clicked with id " + id + "  and side "  +tableSide.ToString());
        if( tableSide == MonsterRowView.TableSide.RIGHT )
        {
            mv = view.right_list_root.GetChild(id).GetComponent<MonsterRowView>();

            // if a current selection on the right
            if (this.rightSelectedMonster != null)
            {
                this.rightSelectedMonster.DeselectView();
                // set delegates to not selected
                SetDelegateToNotSelected(this.rightSelectedMonster);                

                // ...also unlock left side monster
                mvMirror = view.left_list_root.GetChild(this.rightSelectedMonster.id).GetComponent<MonsterRowView>(); 
                mvMirror.ActivateView();
            }

            // update current selection value
            this.rightSelectedMonster = mv;

            // ...also lock left side monster
            mvMirror = view.left_list_root.GetChild(id).GetComponent<MonsterRowView>();
            mvMirror.DeactivateView();
        }
        else if (tableSide == MonsterRowView.TableSide.LEFT )
        {
            mv = view.left_list_root.GetChild(id).GetComponent<MonsterRowView>();
            
            if (this.leftSelectedMonster != null)
            {
                this.leftSelectedMonster.DeselectView();
                // set delegates to not selected
                SetDelegateToNotSelected(this.leftSelectedMonster);              

                // ...also unlock right side monster
                mvMirror = view.right_list_root.GetChild(this.leftSelectedMonster.id).GetComponent<MonsterRowView>();
                mvMirror.ActivateView();
            }
            this.leftSelectedMonster = mv;
            //  ...also lock right side monster
            mvMirror = view.right_list_root.GetChild(id).GetComponent<MonsterRowView>();
            mvMirror.DeactivateView();
        }
        if (mv != null)
        {
            mv.SelectView();
            SetDelegateToSelected(mv);                   
        }

        RefreshBreedingButton();

    }

    public void OnMonsterClick_Selected(int id, MonsterRowView.TableSide tableSide)
    {
        MonsterRowView mv = null;
        MonsterRowView mvMirror = null; // corresponding monster on the other side

        if (tableSide == MonsterRowView.TableSide.RIGHT)
        {
            mv = view.right_list_root.GetChild(id).GetComponent<MonsterRowView>();
            this.rightSelectedMonster = null;
            // ...also unlock left side monster
            mvMirror = view.left_list_root.GetChild(id).GetComponent<MonsterRowView>();
            mvMirror.ActivateView();            
        }
        else if (tableSide == MonsterRowView.TableSide.LEFT)
        {
            mv = view.left_list_root.GetChild(id).GetComponent<MonsterRowView>();
            this.leftSelectedMonster = null;
            // TODO...also unlock right side monster
            mvMirror = view.right_list_root.GetChild(id).GetComponent<MonsterRowView>();
            mvMirror.ActivateView();         
        }

        if( mv != null)
        {            
            mv.DeselectView();
            SetDelegateToNotSelected(mv);            
        }

        RefreshBreedingButton();       
    }

    private void SetDelegateToNotSelected(MonsterRowView mv)
    {
        mv.OnMonsterClick -= OnMonsterClick_Selected;
        mv.OnMonsterClick += OnMonsterClick_NotSelected;
    }

    private void SetDelegateToSelected(MonsterRowView mv)
    {
        mv.OnMonsterClick -= OnMonsterClick_NotSelected;
        mv.OnMonsterClick += OnMonsterClick_Selected;        
    }

    private void OnOpenLanguagePanelClick()
    {
        openLanguagePanelSignal.Dispatch();
    }

    
}
