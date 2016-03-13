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

public class SpeedUpViewMediator : Mediator {

    // Injecting the view
    [Inject]
    public SpeedUpView view { get; set; }

    // Inject the localization service to translate the view
    [Inject]
    public ILocalizationService localizationService { get; set; }

    // Signals we want to listen to
    [Inject]
    public LoadCompleteSignal loadCompleteSignal { get; set; }
    [Inject]
    public NewSpeedUpBreedingRequestSignal newSpeedUpRequestSignal { get; set; }


    private StringReactiveProperty speedUpButtonText = new StringReactiveProperty("");
    private StringReactiveProperty popupText = new StringReactiveProperty("");
    private StringReactiveProperty hearthText = new StringReactiveProperty("");

    public override void OnRegister()
    {
        base.OnRegister();

        // Init view
        view.init();
        loadCompleteSignal.AddListener(OnLoadComplete);
        newSpeedUpRequestSignal.AddListener(OnNewSpeedUpRequest);
    }

    public override void OnRemove()
    {
        base.OnRemove();
        loadCompleteSignal.RemoveListener(OnLoadComplete);
        newSpeedUpRequestSignal.RemoveListener(OnNewSpeedUpRequest);
    }

    private void OnLoadComplete(ReactiveCollection<MonsterDataModel> monsters,
                                ReactiveDictionary<string, ElementDataModel> elements,
                                Dictionary<string, Texture2D> images)
    {
        // Header of the view
        localizationService.GetString("complete_title").Subscribe(x => view.popup_title_text.text = x.ToUpper());
        speedUpButtonText = new StringReactiveProperty(localizationService.GetString("speedup_button").Value);
        hearthText = new StringReactiveProperty(localizationService.GetString("complete_heart").Value);
        popupText = new StringReactiveProperty(localizationService.GetString("complete_message").Value);

        view.OnSpendGemsClick += OnSpendGemsClick;
        view.OnClosePopupClick += OnClosePopupClick;
    }

    private void OnClosePopupClick()
    {
        view.HideView();
    }

    private void OnSpendGemsClick()
    {
        Debug.Log("Spend all the money");
    }

    private void OnNewSpeedUpRequest(BreedingStatusModel status)
    {
        status.timeLeft
            .Subscribe(t =>
                {
                    status.gemsRequired.Subscribe( gems =>
                        {
                            speedUpButtonText
                                .Subscribe(x => 
                                    {
                                        view.speedup_button_text.text = string.Format(x,gems).ToUpper();

                                    });
                            popupText
                                .Subscribe(p =>
                                {
                                    view.popup_text.text = string.Format(p, t+"s", gems);
                                });
                        });
                }
            );
       
        view.ShowView();
    }
}
