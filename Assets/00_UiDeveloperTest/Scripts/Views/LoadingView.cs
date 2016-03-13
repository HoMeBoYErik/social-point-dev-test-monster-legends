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
    public CanvasGroup canvas;
    public GameObject progress_trail_fx;

    // On Awake
    protected override void Awake()
    {
        base.Awake();
        // Map view objects
        progress_bar = this.transform.FindChild("progress_bar").GetComponent<Slider>();
        loading_text = this.transform.FindChild("loading_text").GetComponent<Text>();
        canvas = this.GetComponent<CanvasGroup>();
        progress_trail_fx = progress_bar.gameObject.transform.FindChild("fill_area/fill/progress_trail_fx").gameObject;
        progress_trail_fx.SetActive(true);

       

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
        TweenFactory.Tween("FadeOutCanvas", 1.0f, 0.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    #endregion

}
