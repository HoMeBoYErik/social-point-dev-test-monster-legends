using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using SocialPoint;

public class LanguageView : View {


    public Button english_button;
    public Button spanish_button;
    public Button russian_button;
    public Button french_button;
    public Button language_close_button;

    public CanvasGroup canvas;

    //Buttons delegates
    public delegate void SetLanguageClick(string lang);
    public SetLanguageClick OnSetLanguageClick;

    public delegate void CloseLanguageClick();
    public CloseLanguageClick OnCloseLanguageClick;


    // On Awake
    protected override void Awake()
    {
        base.Awake();

        canvas = this.transform.GetComponent<CanvasGroup>();
        english_button = this.transform.FindChild("button_container/english_button").GetComponent<Button>();
        spanish_button = this.transform.FindChild("button_container/spanish_button").GetComponent<Button>();
        french_button = this.transform.FindChild("button_container/french_button").GetComponent<Button>();
        russian_button = this.transform.FindChild("button_container/russian_button").GetComponent<Button>();
        language_close_button = this.transform.FindChild("button_container/button_close").GetComponent<Button>();

    }

    // Reset view to default state
    internal void Reset()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.mapEventListeners();
    }

    // OnDisable detact listeners and unmap delegates
    protected override void OnDisable()
    {
    }

    // Here map delegates between others monobehaviours and the view
    internal void mapEventListeners()
    {
        english_button.onClick.AddListener(() =>
            {
                OnSetLanguageClick("en");
            });
        
        spanish_button.onClick.AddListener(() =>
        {
            OnSetLanguageClick("es");
        });
        
        french_button.onClick.AddListener(() =>
        {
            OnSetLanguageClick("fr");
        });
        
        russian_button.onClick.AddListener(() =>
        {
            OnSetLanguageClick("ru");
        });

        language_close_button.onClick.AddListener(() =>
            {
                OnCloseLanguageClick();
            });

    }

    internal void init()
    {
    }

    #region VIEW ANIMATIONS CONTROLS
    public void HideView()
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        TweenFactory.Tween("FadeOutLanguageView", 1.0f, 0.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    public void ShowView()
    {
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        TweenFactory.Tween("FadeInLanguageView", 0.0f, 1.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    #endregion
}
