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

public class BreedingViewMediator : Mediator
{
    // Injecting the view
    [Inject]
    public BreedingView view { get; set; }

    // Inject the localization service to translate the view
    [Inject]
    public ILocalizationService localizationService { get; set; }

    // Signals we want to listen to
    [Inject]
    public LoadCompleteSignal loadCompleteSignal { get; set; }
    [Inject]
    public NewBreedingCoupleSignal startBreedingSignal { get; set; }
    [Inject]
    public BreedingSpeededUpSignal breedingSpeededUpSignal { get; set; }

    // Signals we want to fire
    [Inject]
    public SpeedUpBreedingRequestSignal speedUpRequestSignal { get; set; }
    [Inject]
    public BreedingEndedSignal breedingEndedSignal { get; set; }
    [Inject]
    public SpeedUpPopupDismissed popupDismissed { get; set; }
    
    private StringReactiveProperty speedUpButtonText = new StringReactiveProperty("");
    private IntReactiveProperty speedUpGemsRequired = new IntReactiveProperty(0);
    private StringReactiveProperty breedingTimeLeftText = new StringReactiveProperty("");
    private IntReactiveProperty breedingTimeLeft = new IntReactiveProperty(30);
    

    private ReactiveProperty<Texture2D> leftMonsterTex = new ReactiveProperty<Texture2D>();
    private ReactiveProperty<Texture2D> rightMonsterTex = new ReactiveProperty<Texture2D>();
    private StringReactiveProperty leftMonsterName = new StringReactiveProperty("");
    private StringReactiveProperty rightMonsterName = new StringReactiveProperty("");
    private StringReactiveProperty leftMonsterDescription = new StringReactiveProperty("");
    private StringReactiveProperty rightMonsterDescription = new StringReactiveProperty("");

    public int MinBreedingTime = 10;
    public int MaxBreedingTime = 30;
    public int GemTimeFactor = 2;

    private bool isLeftDescOpen = false;
    private bool isRightDescOpen = false;

    
    public override void OnRegister()
    {
        base.OnRegister();

        // Init view
        view.init();
        loadCompleteSignal.AddListener(OnLoadComplete);
        startBreedingSignal.AddListener(OnBreedingStart);
        breedingSpeededUpSignal.AddListener(OnBreedingSpeededUp);
        popupDismissed.AddListener(OnPopupDismissed);

        //Bind RX properties to view
        leftMonsterTex.Subscribe(tex => view.left_monster_image.texture = tex);
        rightMonsterTex.Subscribe(tex => view.right_monster_image.texture = tex);
        leftMonsterName.SubscribeToText(view.left_monster_name_text);
        rightMonsterName.SubscribeToText(view.right_monster_name_text);
        leftMonsterDescription.SubscribeToText(view.left_monster_description);
        rightMonsterDescription.SubscribeToText(view.right_monster_description);

    }

    public override void OnRemove()
    {
        base.OnRemove();
        loadCompleteSignal.RemoveListener(OnLoadComplete);
        startBreedingSignal.RemoveListener(OnBreedingStart);
        breedingSpeededUpSignal.RemoveListener(OnBreedingSpeededUp);
        popupDismissed.RemoveListener(OnPopupDismissed);
        view.OnSpeedUpClick -= this.OnSpeedUpClick;
        view.OnLeftMonsterDescriptionClick -= this.OnLeftInfoClick;
        view.OnRightMonsterDescriptionClick -= this.OnRightInfoClick;
    }

    private void OnLoadComplete(List<MonsterDataModel> monsters,
                                 Dictionary<string, ElementDataModel> elements,
                                 Dictionary<string, Texture2D> images)
    {
        // Header of the view
        localizationService.GetString("title").Subscribe(x => view.header_title_text.text = x.ToUpper());

        // Only changing the gems requires value the view will be updated with formatted text
        speedUpButtonText = new StringReactiveProperty(localizationService.GetString("speedup_button").Value);
        speedUpGemsRequired
            .Subscribe(gems => speedUpButtonText
                .Subscribe(x => view.speedup_button_text.text = string.Format(x, gems).ToUpper())
                );

        // When breeding time is updated the view will be notified
        breedingTimeLeftText = new StringReactiveProperty(localizationService.GetString("waiting").Value);
        breedingTimeLeft
            .Subscribe(t => breedingTimeLeftText
                .Subscribe(x => view.breeding_time_remaining_text.text = string.Format(x, "00:" + string.Format("{0:00}", t)).ToUpper())
                );

        view.OnSpeedUpClick += this.OnSpeedUpClick;
        view.OnLeftMonsterDescriptionClick += this.OnLeftInfoClick;
        view.OnRightMonsterDescriptionClick += this.OnRightInfoClick;
    }

    private void OnSpeedUpClick()
    {
        view.BreedingViewFX.SetActive(false);
        BreedingStatusModel status = new BreedingStatusModel();
        status.timeLeft = this.breedingTimeLeft;
        status.gemsRequired = this.speedUpGemsRequired;
        speedUpRequestSignal.Dispatch(status);
    }

    private void OnPopupDismissed()
    {
        view.BreedingViewFX.SetActive(true);
    }

    private void OnLeftInfoClick()
    {    

        if( isLeftDescOpen)
        {
            view.left_monster_tweener.Play("Exit");
            isLeftDescOpen = false;
        }
        else
        {
            view.left_monster_tweener.Play("Enter");
            isLeftDescOpen = true;
        }
    }

    private void OnRightInfoClick()
    {
        if (isRightDescOpen)
        {
            view.right_monster_tweener.Play("Exit");
            isRightDescOpen = false;
        }
        else
        {
            view.right_monster_tweener.Play("Enter");
            isRightDescOpen = true;
        }

       
    }

    private void OnBreedingStart(BreedingCoupleDataModel couple)
    {
        rightMonsterTex.Value = couple.rightMonsterTex;
        rightMonsterName.Value = couple.rightMonsterName;
        rightMonsterDescription.Value = couple.rightMonsterDescription;


        leftMonsterTex.Value = couple.leftMonsterTex;
        leftMonsterName.Value = couple.leftMonsterName;
        leftMonsterDescription.Value = couple.leftMonsterDescription;


        // Activate animations
        view.loving_fx_left.SetActive(true);
        view.loving_fx_right.SetActive(true);
        view.electro_fx.SetActive(true);
        view.gem_aura_fx.SetActive(true);

        // Activate animations based on monster type
        switch(couple.leftMonsterType)
        {
            case "Fire Lion":
                view.fire_lion_fx_left.SetActive(true);
                break;

            case "Lord of Atlantis":
                view.atlantis_fx_left.SetActive(true);
                break;

            case "Pandalf":
                view.pandalf_fx_left.SetActive(true);
                break;

            case "Rockilla":
                break;

            default:
                break;
        }

        switch (couple.rightMonsterType)
        {
            case "Fire Lion":
                view.fire_lion_fx_right.SetActive(true);
                break;

            case "Lord of Atlantis":
                view.atlantis_fx_right.SetActive(true);
                break;

            case "Pandalf":
                view.pandalf_fx_right.SetActive(true);
                break;

            case "Rockilla":
                break;

            default:
                break;
        }

        

       

        // Generate a random breeding time
        int breedingTime = UnityEngine.Random.Range(MinBreedingTime, MaxBreedingTime + 1);
        StartCoroutine("BreedingProcess", breedingTime);

        view.ShowView();
    }

    private void StopAllAnimations()
    {
        if( isRightDescOpen )
        {
            view.right_monster_tweener.Play("Exit");
            isRightDescOpen = false;
        }
        if( isLeftDescOpen )
        {
            view.left_monster_tweener.Play("Exit");
            isLeftDescOpen = false;
        }
        
        view.loving_fx_left.SetActive(false);
        view.loving_fx_right.SetActive(false);
        view.electro_fx.SetActive(false);
        view.gem_aura_fx.SetActive(false);
        view.fire_lion_fx_left.SetActive(false);
        view.fire_lion_fx_right.SetActive(false);
        view.atlantis_fx_left.SetActive(false);
        view.atlantis_fx_right.SetActive(false);
        view.pandalf_fx_left.SetActive(false);
        view.pandalf_fx_right.SetActive(false);
    }


   
    IEnumerator BreedingProcess(float breedTime)
    {
        //float startTime = Time.time;
        float timer = breedTime;
        breedingTimeLeft.Value = (int)breedTime;

        while (timer > 0)
        {

            breedingTimeLeft.Value = (int)timer;
            speedUpGemsRequired.Value = (int)timer * GemTimeFactor;           
            // countdown timer
            timer -= Time.deltaTime;

            yield return null;
        }

        this.OnBreedingEnded();
    }
	
    private void OnBreedingSpeededUp()
    {
        StopCoroutine("BreedingProcess");
        this.OnBreedingEnded();
    }

    private void OnBreedingEnded()
    {
        Debug.Log("Breeding Ended");
        StopAllAnimations();
        view.HideView();
        // play a sound
        // Dispatch signal that breeding has completed
        breedingEndedSignal.Dispatch();
       
    }
}
