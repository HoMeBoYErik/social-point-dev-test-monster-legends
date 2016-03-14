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

public class LanguageViewMediator : Mediator
{
    // Injecting the view
    [Inject]
    public LanguageView view { get; set; }

    [Inject]
    public ILocalizationService localizationService { get; set; }

    // We listen for the signal when someone want to open it
    [Inject]
    public OpenLanguagePanelSignal openLanguagePanelSignal { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        // Init view
        view.init();

        view.OnCloseLanguageClick += OnLanguageCloseClick;
        view.OnSetLanguageClick += OnSetLanguageClick;

        openLanguagePanelSignal.AddListener(OnLanguagePanelOpen);
       
    }

    // Clean the view from listeners
    public override void OnRemove()
    {
        base.OnRemove();
        view.OnCloseLanguageClick -= OnLanguageCloseClick;
        view.OnSetLanguageClick -= OnSetLanguageClick;
        openLanguagePanelSignal.RemoveListener(OnLanguagePanelOpen);
    }

    private void OnLanguagePanelOpen()
    {
        view.ShowView();
    }

    private void OnLanguageCloseClick()
    {
        view.HideView();
    }

    private void OnSetLanguageClick(string lang)
    {
        // Just to be sure that it is a supported language
        if( localizationService.isValidLanguage ( lang ) )
        {
            localizationService.SetUserLanguage(lang);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload scene with new language
        }
    }
	
}
