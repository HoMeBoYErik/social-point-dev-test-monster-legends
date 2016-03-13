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

public class BreedingView : View
{

    //Reference to view elements
    public Text header_title_text;
    public Button speedup_button;
    public Text speedup_button_text;
    public Text breeding_time_remaining_text;
    public RawImage right_monster_image;
    public Text right_monster_name_text;
    public RawImage left_monster_image;
    public Text left_monster_name_text;
    public CanvasGroup canvas;

    //Buttons delegates
    public delegate void SpeedUpClick();
    public SpeedUpClick OnSpeedUpClick;
  

 
    // On Awake
    protected override void Awake()
    {
        base.Awake();

        header_title_text = transform.FindChild("vertical_fit/header/header_title_text").GetComponent<Text>();
        speedup_button = transform.FindChild("vertical_fit/speedup_button").GetComponent<Button>();
        speedup_button_text = transform.FindChild("vertical_fit/speedup_button/speedup_button_text").GetComponent<Text>();
        breeding_time_remaining_text = transform.FindChild("vertical_fit/breeding_time_remaining/breeding_time_remaining_text").GetComponent<Text>();
        right_monster_image = transform.FindChild("horizontal_fit/right_breeding_panel/right_image_frame/right_monster_image").GetComponent<RawImage>();
        right_monster_name_text = transform.FindChild("horizontal_fit/right_breeding_panel/right_image_frame/right_monster_name_text").GetComponent<Text>();
        left_monster_image = transform.FindChild("horizontal_fit/left_breeding_panel/left_image_frame/left_monster_image").GetComponent<RawImage>();
        left_monster_name_text = transform.FindChild("horizontal_fit/left_breeding_panel/left_image_frame/left_monster_name_text").GetComponent<Text>();
        canvas = this.transform.GetComponent<CanvasGroup>();



    }

    // Reset view to default state
    internal void Reset()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MapEventListeners();
    }

    // OnDisable detact listeners and unmap delegates
    protected override void OnDisable()
    {
    }

    // Here map delegates between others monobehaviours and the view
    internal void MapEventListeners()
    {
        speedup_button.onClick.AddListener( () =>
        {
            OnSpeedUpClick();
        });      
    }

    internal void init()
    {
        header_title_text.text = "BREEDING";
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
