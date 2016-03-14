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

public class LoadingView : View
{
    //Create delegates here

    //Create reference to view elements
    public Slider progress_bar;
    public Text loading_text;
    public Text random_phrase;
    public CanvasGroup canvas;

    public GameObject LoadingViewFx;
    public GameObject progress_trail_fx;
    public GameObject logo_trail_fx;

    // On Awake
    protected override void Awake()
    {
        base.Awake();
        // Map view objects
        progress_bar = this.transform.FindChild("progress_bar").GetComponent<Slider>();
        loading_text = this.transform.FindChild("loading_text").GetComponent<Text>();
        random_phrase = this.transform.FindChild("random_phrase").GetComponent<Text>();
        canvas = this.GetComponent<CanvasGroup>();
        progress_trail_fx = progress_bar.gameObject.transform.FindChild("fill_area/fill/progress_trail_fx").gameObject;
        progress_trail_fx.SetActive(true);
        logo_trail_fx = GameObject.Find("logo_trail_fx");
        LoadingViewFx = GameObject.Find("LoadingViewFX");

       

    }

    // Reset view to default state
    internal void reset()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.mapEventListeners();
    }

    // OnDisable detach listeners and unmap delegates
    protected override void OnDisable()
    {
    }

    // Here map delegates between others monobehaviours and the view
    internal void mapEventListeners()
    {
    }

    internal void init()
    {
        // map it to progress ref
        progress_bar.value = 0.0f;
    }

    #region VIEW ANIMATIONS CONTROLS
    public void HideView()
    {
        progress_trail_fx.SetActive(false);
        logo_trail_fx.SetActive(false);
        LoadingViewFx.SetActive(false);
        TweenFactory.Tween("FadeOutCanvas", 1.0f, 0.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    #endregion

}
