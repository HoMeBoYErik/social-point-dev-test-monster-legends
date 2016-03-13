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
    private string[] randomPhrases = { "monsters", "gems", "pizzas", "coins", "trees", "colours", "ice creams" };

    public override void OnRegister()
    {
        base.OnRegister();

        // Init view
        view.init();
        loadingRandomPhrase.SubscribeToText(view.loading_text);
        loadProgress.Subscribe(x => view.progress_bar.value = x);
        StartCoroutine(GenerateRandomPhrase());
        // We are ready to load game data
        // Listen for when all the data will be ready
        loadCompleteSignal.AddListener(OnLoadComplete);
        // The signal carry the handle where we want to be notified on progress
        loadGameDataSignal.Dispatch(loadProgress);
    }

    IEnumerator GenerateRandomPhrase()
    {
        while( !isDataLoaded )
        {
            loadingRandomPhrase.Value = "LOADING..." + randomPhrases[UnityEngine.Random.Range(0, randomPhrases.Length)];
            //loadProgress.Value = UnityEngine.Random.Range(0.1f, 0.95f);
            yield return new WaitForSeconds(1.0f);
        }
        
    }

    public override void OnRemove()
    {
        base.OnRemove();
        loadCompleteSignal.RemoveAllListeners();
    }

    #region Signals Received Listeners
    private void OnLoadComplete(ReactiveCollection<MonsterDataModel> monsters,
                                 ReactiveDictionary<string, ElementDataModel> elements,
                                 Dictionary<string, Texture2D> images)
    {
        view.HideView();
    }
    #endregion
}
