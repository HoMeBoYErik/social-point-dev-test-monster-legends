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

    // Signals we want to fire
    [Inject]
    public SpeedUpBreedingRequestSignal speedUpRequestSignal { get; set; }
    
    private StringReactiveProperty speedUpButtonText = new StringReactiveProperty("");
    private IntReactiveProperty speedUpGemsRequired = new IntReactiveProperty(0);
    private StringReactiveProperty breedingTimeLeftText = new StringReactiveProperty("");
    private IntReactiveProperty breedingTimeLeft = new IntReactiveProperty(30);

    private ReactiveProperty<Texture2D> leftMonsterTex = new ReactiveProperty<Texture2D>();
    private ReactiveProperty<Texture2D> rightMonsterTex = new ReactiveProperty<Texture2D>();
    private StringReactiveProperty leftMonsterName = new StringReactiveProperty("");
    private StringReactiveProperty rightMonsterName = new StringReactiveProperty("");

    public int MinBreedingTime = 10;
    public int MaxBreedingTime = 30;
    public int GemTimeFactor = 2;
    
    public override void OnRegister()
    {
        base.OnRegister();

        // Init view
        view.init();
        loadCompleteSignal.AddListener(OnLoadComplete);
        startBreedingSignal.AddListener(OnBreedingStart);

        //Bind RX properties to view
        leftMonsterTex.Subscribe(tex => view.left_monster_image.texture = tex);
        rightMonsterTex.Subscribe(tex => view.right_monster_image.texture = tex);
        leftMonsterName.SubscribeToText(view.left_monster_name_text);
        rightMonsterName.SubscribeToText(view.right_monster_name_text);

    }

    public override void OnRemove()
    {
        base.OnRemove();
        loadCompleteSignal.RemoveListener(OnLoadComplete);
        startBreedingSignal.RemoveListener(OnBreedingStart);
        view.OnSpeedUpClick -= this.OnSpeedUpClick;
    }

    private void OnLoadComplete(ReactiveCollection<MonsterDataModel> monsters,
                                 ReactiveDictionary<string, ElementDataModel> elements,
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
                .Subscribe(x => view.breeding_time_remaining_text.text = string.Format(x, "00:"+string.Format("{0:00}", t)).ToUpper())
                );

        view.OnSpeedUpClick += this.OnSpeedUpClick;
    }

    private void OnSpeedUpClick()
    {
        BreedingStatusModel status = new BreedingStatusModel();
        status.timeLeft = this.breedingTimeLeft;
        status.gemsRequired = this.speedUpGemsRequired;
        speedUpRequestSignal.Dispatch(status);
    }

    private void OnBreedingStart(BreedingCoupleDataModel couple)
    {
        rightMonsterTex.Value = couple.rightMonsterTex;
        rightMonsterName.Value = couple.rightMonsterName;

        leftMonsterTex.Value = couple.leftMonsterTex;
        leftMonsterName.Value = couple.leftMonsterName;

        // Generate a random breeding time
        int breedingTime = UnityEngine.Random.Range(MinBreedingTime, MaxBreedingTime + 1);
        StartCoroutine(BreedingProcess(breedingTime));

        view.ShowView();
    }

    // TODO convert to a observable stream throttled
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
	
    private void OnBreedingEnded()
    {
        Debug.Log("Breeding Ended");
        view.HideView();
        // play a sound
        // Dispatch signal that breeding has completed

       
    }
}
