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

public class MonsterSelectionView : View {

    //Reference to view elements
    public Text header_title_text;
    public Button breeding_button;
    public Text breeding_button_text;
    public CanvasGroup canvas;
    public Transform right_list_root;
    public Transform left_list_root;


    // On Awake
    protected override void Awake()
    {
        base.Awake();

        header_title_text = this.transform
            .Find("vertical_fit/header/header_title_text")
            .GetComponent<Text>();

        breeding_button = this.transform
            .Find("vertical_fit/breeding_button").GetComponent<Button>();

        breeding_button_text = breeding_button.GetComponentInChildren<Text>();

        canvas = this.GetComponent<CanvasGroup>();

        // We use the tag here because the child is very deep down the hierarchy
        right_list_root = GameObject.FindGameObjectWithTag("RightListRoot").transform;
        left_list_root = GameObject.FindGameObjectWithTag("LeftListRoot").transform;
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

    // OnDisable detact listeners and unmap delegates
    protected override void OnDisable()
    {
    }

    // Here map delegates between others monobehaviours and the view
    internal void mapEventListeners()
    {
    }

    internal void init()
    {
        header_title_text.text = "BREEDING";
        breeding_button.interactable = false;
        breeding_button_text.text = "___";
    }

    #region VIEW ANIMATIONS CONTROLS
    public void HideView()
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        TweenFactory.Tween("FadeOutCanvas", 1.0f, 0.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    public void ShowView()
    {
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        TweenFactory.Tween("FadeInCanvas", 0.0f, 1.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    #endregion
}
