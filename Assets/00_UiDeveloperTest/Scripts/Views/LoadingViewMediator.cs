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

public class LoadingViewMediator : Mediator{

    // Injecting the view
    [Inject]
    public LoadingView view { get; set; }

    // Inject Localization service
    [Inject]
    public ILocalizationService localizationService { get; set; }

    // Injecting this one because we want to fire it
    [Inject]
    public LoadGameDataSignal loadGameDataSignal { get; set; }

    // Signals we want to listen to
    [Inject]
    public LoadCompleteSignal loadCompleteSignal { get; set; }

    [SerializeField]
    public StringReactiveProperty loadingRandomPhrase = new StringReactiveProperty();
    public FloatReactiveProperty loadProgress = new FloatReactiveProperty(0);

    public bool isDataLoaded = false;
    private string[] randomPhrases = { "monsters", "levels", "gems", "coins", "love", "colours", "sprites", "textures", "skill" };
    private string loadingTranslated;


    public override void OnRegister()
    {
        base.OnRegister();

        // Init view
        view.init();
        loadingRandomPhrase.SubscribeToText(view.random_phrase);
        loadProgress.Subscribe(x => view.progress_bar.value = x);
        StartCoroutine("GenerateRandomPhrase");
        // We are ready to load game data
        // Listen for when all the data will be ready
        loadCompleteSignal.AddListener(OnLoadComplete);
        // The signal carry the handle where we want to be notified on progress
        loadGameDataSignal.Dispatch(loadProgress);
    }

    IEnumerator GenerateRandomPhrase()
    {
        string currentLanguage = localizationService.LoadUserLanguage();

        if (currentLanguage == "en")
        {
            loadingTranslated = "LOADING";
        }
        else if (currentLanguage == "es")
        {
            loadingTranslated = "CARGANDO";
        }
        else if (currentLanguage == "fr")
        {
            loadingTranslated = "CHARGEMENT";
        }
        else if (currentLanguage == "ru")
        {
            loadingTranslated = "ЗАГРУЗКА";
        }
        view.loading_text.text = loadingTranslated;

        while( !isDataLoaded )
        {
            loadingRandomPhrase.Value = (loadingTranslated + "..." + randomPhrases[UnityEngine.Random.Range(0, randomPhrases.Length)]).ToLower();
            
            yield return new WaitForSeconds(0.5f);
        }
        
    }

    public override void OnRemove()
    {
        base.OnRemove();
        loadCompleteSignal.RemoveAllListeners();
    }

    #region Signals Received Listeners
    private void OnLoadComplete(List<MonsterDataModel> monsters,
                                 Dictionary<string, ElementDataModel> elements,
                                 Dictionary<string, Texture2D> images)
    {
        StopCoroutine("GenerateRandomPhrase");
        view.HideView();
    }
    #endregion
}
